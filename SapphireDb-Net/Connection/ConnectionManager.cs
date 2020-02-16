using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using SapphireDb_Net.Command;
using SapphireDb_Net.Command.Connection;
using SapphireDb_Net.Command.Subscribe;
using SapphireDb_Net.Command.Unsubscribe;
using SapphireDb_Net.Models;
using SapphireDb_Net.Options;

namespace SapphireDb_Net.Connection
{
    public class ConnectionManager
    {
        private string _authToken = null;
        
        private readonly SapphireDbOptions _options;
        private readonly ConnectionBase _connection;

        private readonly ConcurrentDictionary<Guid, CommandReference> _commandReferences = new ConcurrentDictionary<Guid, CommandReference>();
        private ConcurrentBag<SubscribeCommandInfo> _storedCommandStorage = new ConcurrentBag<SubscribeCommandInfo>();

        public ConnectionManager(SapphireDbOptions options)
        {
            _options = options;
            
            _connection = new WebsocketConnection();

            _connection.ConnectionInformation
                .Where(information => information.ReadyState == ConnectionState.Connected)
                .Subscribe((information) =>
                {
                    Array.ForEach(_storedCommandStorage.ToArray(), (commandInfo) =>
                    {
                        if (!commandInfo.SendWithAuthToken || !string.IsNullOrEmpty(_authToken))
                        {
                            _connection.Send(commandInfo.Command, true);
                        }
                    });
                });

            _connection.MessageHandler = HandleResponse;
            _connection.SetData(_options);
        }

        private bool StoreSubscribeCommand(CommandBase command)
        {
            if (command is UnsubscribeCommand)
            {
                _storedCommandStorage = new ConcurrentBag<SubscribeCommandInfo>(
                    _storedCommandStorage.Where(v => v.Command.ReferenceId != command.ReferenceId));
                return true;
            }
            
            if (command is SubscribeCommand)
            {
                if (_storedCommandStorage.All(v => v.Command.ReferenceId != command.ReferenceId))
                {
                    _storedCommandStorage.Add(new SubscribeCommandInfo()
                    {
                        SendWithAuthToken = !string.IsNullOrEmpty(_authToken),
                        Command = command
                    });
                    return true;
                }
            }

            return false;
        }
        
        public IObservable<ResponseBase> SendCommand(CommandBase command, bool keep = false, bool onlySend = false)
        {
            bool storedCommand = StoreSubscribeCommand(command);
            
            if (!storedCommand || _connection.ConnectionInformation.Value.ReadyState == ConnectionState.Connected) {
                _connection.Send(command, storedCommand);
            }

            if (onlySend)
            {
                return Observable.Return<ResponseBase>(null);
            }

            ReplaySubject<ResponseBase> referenceSubject = new ReplaySubject<ResponseBase>(1);
            CommandReference reference = new CommandReference()
            {
                Subject = referenceSubject,
                Keep = keep
            };

            _commandReferences.TryAdd(command.ReferenceId, reference);

            return referenceSubject.AsObservable().Finally(() =>
            {
                referenceSubject.OnCompleted();
                _commandReferences.TryRemove(command.ReferenceId, out CommandReference _);
            });
        }

        private void HandleResponse(ResponseBase response)
        {
            if (response is WrongApiResponse)
            {
                throw new Exception("Wrong API key or secret");
            }
            
            if (response.ResponseType == "MessageResponse")
            {
                
            }
            else
            {
                if (response.ReferenceId.HasValue && _commandReferences.TryGetValue(response.ReferenceId.Value, out CommandReference reference))
                {
                    if (response.Error != null)
                    {
                        reference.Subject.OnError(new Exception(response.Error.ToString()));
                        reference.Subject.OnCompleted();
                        _commandReferences.TryRemove(response.ReferenceId.Value, out CommandReference _);
                    }
                    else
                    {
                        reference.Subject.OnNext(response);

                        if (!reference.Keep)
                        {
                            reference.Subject.OnCompleted();
                            _commandReferences.TryRemove(response.ReferenceId.Value, out CommandReference _);
                        }
                    }
                }
            }
        }
    }
}
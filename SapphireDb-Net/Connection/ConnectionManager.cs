using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using SapphireDb_Net.Command;
using SapphireDb_Net.Models;
using SapphireDb_Net.Options;

namespace SapphireDb_Net.Connection
{
    public class ConnectionManager
    {
        private string _authToken = null;
        
        private readonly SapphireDbOptions _options;
        private readonly ConnectionBase _connection;
        
        private readonly List<SubscribeCommandInfo> _storedCommandStorage = new List<SubscribeCommandInfo>();
        
        public ConnectionManager(SapphireDbOptions options)
        {
            _options = options;
            
            _connection = new WebsocketConnection();

            _connection.ConnectionInformation
                .Where(information => information.ReadyState == ConnectionState.Connected)
                .Subscribe((information) =>
                {
                    _storedCommandStorage.ForEach(commandInfo =>
                    {
                        if (!commandInfo.SendWithAuthToken || !string.IsNullOrEmpty(_authToken))
                        {
                            _connection.Send(commandInfo.Command, true);
                        }
                    });
                });

            _connection.MessageHandler = (message) =>
            {
                
            };
            
            _connection.SetData(_options);
        }

        public IObservable<ResponseBase> SendCommand(CommandBase command, bool keep = false, bool onlySend = false)
        {
            bool storedCommand = false; // TODO
            
            if (!storedCommand || _connection.ConnectionInformation.Value.ReadyState == ConnectionState.Connected) {
                _connection.Send(command, storedCommand);
            }

            if (onlySend)
            {
                return Observable.Return<ResponseBase>(null);
            }

            ReplaySubject<ResponseBase> referenceSubject = new ReplaySubject<ResponseBase>(1);

            return referenceSubject.AsObservable();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SapphireDb_Net.Collection.Prefilter;
using SapphireDb_Net.Command;
using SapphireDb_Net.Command.Create;
using SapphireDb_Net.Command.CreateRange;
using SapphireDb_Net.Command.Info;
using SapphireDb_Net.Command.Query;
using SapphireDb_Net.Command.Subscribe;
using SapphireDb_Net.Command.Unsubscribe;
using SapphireDb_Net.Command.Update;
using SapphireDb_Net.Command.UpdateRange;
using SapphireDb_Net.Connection;
using SapphireDb_Net.Helper;
using SapphireDb_Net.Models;

namespace SapphireDb_Net.Collection
{
    public class CollectionBase<TModel, TValue>
    {
        private readonly ConnectionManager _connectionManager;
        private readonly CollectionManager _collectionManager;
        private readonly IObservable<InfoResponse> _collectionInformation;
        private readonly string _contextName;
        private readonly string _collectionName;

        protected readonly List<IPrefilter> _prefilters;


        public CollectionBase(string collectionNameRaw, ConnectionManager connectionManager,
            CollectionManager collectionManager, List<IPrefilter> prefilters,
            IObservable<InfoResponse> collectionInformation)
        {
            _connectionManager = connectionManager;
            _collectionManager = collectionManager;
            _collectionInformation = collectionInformation;
            
            _prefilters = prefilters;

            Tuple<string, string> collectionNameParsed = collectionNameRaw.ParseCollectionName();
            _collectionName = collectionNameParsed.Item1;
            _contextName = collectionNameParsed.Item2;
        }

        public IObservable<TValue> Snapshot()
        {
            QueryCommand queryCommand = new QueryCommand(_collectionName, _contextName, _prefilters);

            return _connectionManager.SendCommand(queryCommand)
                .Select(response =>
                {
                    if (response is QueryResponse queryResponse)
                    {
                        return queryResponse.Result.ToObject<TValue>();
                    }

                    return default;
                });
        }

        public IObservable<TValue> Values()
        {
            CollectionValue collectionValue = CreateValuesSubscription<TValue>();
            return CreateCollectionObservable(collectionValue);
        }

        public IObservable<ResponseBase> Changes()
        {
            SubscribeCommand subscribeCommand = new SubscribeCommand(_collectionName, _contextName, _prefilters);
            return _connectionManager.SendCommand(subscribeCommand, true)
                .Finally(() =>
                    {
                        _connectionManager.SendCommand(
                            new UnsubscribeCommand(_collectionName, _contextName, subscribeCommand.ReferenceId), false,
                            true);
                    });
        }

        public IObservable<CommandResult<TModel>> Add(params TModel[] values)
        {
            CommandBase command;
            
            if (values.Length == 1)
            {
                command = new CreateCommand(_collectionName, _contextName, values[0]);
            }
            else
            {
                command = new CreateRangeCommand<TModel>(_collectionName, _contextName, values);
            }

            return CreateCommandResult(_connectionManager.SendCommand(command));
        }
        
        public IObservable<CommandResult<TModel>> Update(params TModel[] values)
        {
            CommandBase command;
            
            if (values.Length == 1)
            {
                command = new UpdateCommand(_collectionName, _contextName, values[0]);
            }
            else
            {
                command = new UpdateRangeCommand<TModel>(_collectionName, _contextName, values);
            }

            return CreateCommandResult(_connectionManager.SendCommand(command));
        }
        
        private CollectionValue CreateValuesSubscription<T>()
        {
            SubscribeCommand subscribeCommand = new SubscribeCommand(_collectionName, _contextName, _prefilters);
            CollectionValue collectionValue = new CollectionValue(subscribeCommand.ReferenceId);

            Type modelType = null;
            
            if (typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                modelType = typeof(T).GetGenericArguments().FirstOrDefault();
            }
            
            collectionValue.SocketSubscription = _connectionManager.SendCommand(subscribeCommand, true)
                .Subscribe((response) =>
                {
                    if (response is QueryResponse queryResponse)
                    {
                        collectionValue.Subject.OnNext(queryResponse.Result.ToObject<TValue>());
                    }
                    else if (response is ChangeResponse changeResponse)
                    {
                        CollectionHelper.CallUpdateCollection(modelType, collectionValue.Subject, _collectionInformation, changeResponse);
                    }
                    else if (response is ChangesResponse changesResponse)
                    {
                        changesResponse.Changes.ForEach(change =>
                        {
                            CollectionHelper.CallUpdateCollection(modelType, collectionValue.Subject, _collectionInformation, change);
                        });
                    }
                }, (error) =>
                {
                    collectionValue.Subject.OnError(error);
                });

            return collectionValue;
        }

        private IObservable<TValue> CreateCollectionObservable(CollectionValue collectionValue)
        {
            return collectionValue.Subject
                .Select((values) => (TValue) values)
                .Finally(() =>
                {
                    _connectionManager.SendCommand(new UnsubscribeCommand(_collectionName, _contextName,
                        collectionValue.ReferenceId));
                    collectionValue.SocketSubscription.Dispose();
                });
                // TODO: Check if necessary
                // .Publish()
                // .Replay(1)
                // .RefCount();
        }

        private IObservable<CommandResult<TModel>> CreateCommandResult(IObservable<ResponseBase> observable)
        {
            return observable
                .Select(response =>
                {
                    if (response is CreateResponse createResponse)
                    {
                        return new CommandResult<TModel>(createResponse.Error, createResponse.ValidationResults, createResponse.NewObject.ToObject<TModel>());
                    }
                    // TODO: Handle Update, UpdateRange and CreateRange
                    // else if (response is UpdateResponse)

                    return null;
                });
        }
    }
}
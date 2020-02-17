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
using SapphireDb_Net.Command.Delete;
using SapphireDb_Net.Command.DeleteRange;
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
        protected readonly CollectionManager _collectionManager;
        private readonly IObservable<InfoResponse> _collectionInformation;
        protected readonly string _contextName;
        protected readonly string _collectionName;

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

        
        /// <summary>
        /// Get a snapshot of the values of the collection
        /// </summary>
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
        
        /// <summary>
        /// Get the values of the collection and also get updates if the collection has changed
        /// </summary>
        public IObservable<TValue> Values()
        {
            CollectionValue collectionValue = CreateValuesSubscription<TValue>();
            return CreateCollectionObservable(collectionValue);
        }
        
        /// <summary>
        /// Get all changes of a collection
        /// </summary>
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

        
        /// <summary>
        /// Add a value to the collection
        /// </summary>
        /// <param name="values">The object(s) to add to the collection</param>
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
        
        /// <summary>
        /// Update a value of the collection
        /// </summary>
        /// <param name="values">The object(s) to update in the collection</param>
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
        
        /// <summary>
        /// Remove a value from the collection
        /// </summary>
        /// <param name="values">The object(s) to remove from the collection</param>
        public IObservable<CommandResult<TModel>> Remove(params TModel[] values)
        {
            CommandBase command;
            
            if (values.Length == 1)
            {
                command = new DeleteCommand(_collectionName, _contextName, values[0]);
            }
            else
            {
                command = new DeleteRangeCommand<TModel>(_collectionName, _contextName, values);
            }

            return CreateCommandResult(_connectionManager.SendCommand(command));
        }
        
        private CollectionValue CreateValuesSubscription<T>()
        {
            SubscribeCommand subscribeCommand = new SubscribeCommand(_collectionName, _contextName, _prefilters);
            CollectionValue collectionValue = new CollectionValue(subscribeCommand.ReferenceId);

            Type modelType = null;

            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
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
                .Select((values) =>
                {
                    dynamic result = values;
                    
                    foreach (dynamic prefilter in _prefilters.Where(p => !(p is IAfterQueryPrefilter)))
                    {
                        result = prefilter.Execute(result);
                    }

                    if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(List<>))
                    {
                        return (TValue)(object)(new List<TModel>(result));
                    }

                    return (TValue)result;
                })
                .Finally(() =>
                {
                    _connectionManager.SendCommand(new UnsubscribeCommand(_collectionName, _contextName,
                        collectionValue.ReferenceId));
                    collectionValue.SocketSubscription.Dispose();
                })
                .Replay(1)
                .RefCount();
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

                    if (response is UpdateResponse updateResponse)
                    {
                        return new CommandResult<TModel>(updateResponse.Error, updateResponse.ValidationResults, updateResponse.UpdatedObject.ToObject<TModel>());
                    }
                    
                    // TODO: Handle Update, UpdateRange and CreateRange

                    return null;
                });
        }
    }
}
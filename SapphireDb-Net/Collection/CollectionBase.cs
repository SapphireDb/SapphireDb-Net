using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SapphireDb_Net.Collection.Prefilter;
using SapphireDb_Net.Command;
using SapphireDb_Net.Command.Info;
using SapphireDb_Net.Command.Query;
using SapphireDb_Net.Command.Subscribe;
using SapphireDb_Net.Command.Unsubscribe;
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

                    return default(TValue);
                });
        }

        public IObservable<TValue> Values()
        {
            CollectionValue collectionValue = CreateValuesSubscription();
            return CreateCollectionObservable(collectionValue);
        }

        private CollectionValue CreateValuesSubscription()
        {
            SubscribeCommand subscribeCommand = new SubscribeCommand(_collectionName, _contextName, _prefilters);
            CollectionValue collectionValue = new CollectionValue(subscribeCommand.ReferenceId);

            collectionValue.SocketSubscription = _connectionManager.SendCommand(subscribeCommand, true)
                .Subscribe((response) =>
                {
                    if (response is QueryResponse queryResponse)
                    {
                        collectionValue.Subject.OnNext(queryResponse.Result.ToObject<TValue>());
                    }
                    else if (response is ChangeResponse changeResponse)
                    {
                        // TODO: Handle update
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
                .Finally(() =>
                {
                    _connectionManager.SendCommand(new UnsubscribeCommand(_collectionName, _contextName,
                        collectionValue.ReferenceId));
                    collectionValue.SocketSubscription.Dispose();
                })
                .Select((values) =>
                {
                    return (TValue)values;
                })
                .Publish()
                .Replay()
                .RefCount();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Newtonsoft.Json.Linq;
using SapphireDb_Net.Collection.Prefilter;
using SapphireDb_Net.Command;
using SapphireDb_Net.Command.Query;
using SapphireDb_Net.Connection;

namespace SapphireDb_Net.Collection
{
    public class CollectionBase<TModel, TValue>
    {
        private readonly ConnectionManager _connectionManager;
        private readonly CollectionManager _collectionManager;
        
        public List<IPrefilter> Prefilters = new List<IPrefilter>();
        public string CollectionName;
        public string ContextName;

        public CollectionBase(string collectionNameRaw, ConnectionManager connectionManager, CollectionManager collectionManager)
        {
            _connectionManager = connectionManager;
            _collectionManager = collectionManager;
            string[] collectionNameParts = collectionNameRaw.Split('.');

            CollectionName = collectionNameParts.Length == 1 ? collectionNameParts[0] : collectionNameParts[1];
            ContextName = collectionNameParts.Length == 2 ? collectionNameParts[0] : "default";
        }
        
        public IObservable<TValue> Snapshot() {
            QueryCommand queryCommand = new QueryCommand(CollectionName, ContextName, Prefilters);

            return _connectionManager.SendCommand(queryCommand)
                .Select(response =>
                {
                    if (response is QueryResponse queryResponse)
                    {
                        Type valueType = typeof(TValue);
                        
                        if (valueType.IsArray)
                        {
                            valueType = valueType.GetElementType();
                            return (TValue)queryResponse.Result.Values(valueType);
                        }
                        else
                        {
                            return queryResponse.Result.Value<TValue>();
                        }
                    }

                    return default(TValue);
                });
            
        }
    }
}
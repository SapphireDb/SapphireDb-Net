using System;
using System.Collections.Generic;
using SapphireDb_Net.Collection.Prefilter;
using SapphireDb_Net.Command.Info;
using SapphireDb_Net.Connection;

namespace SapphireDb_Net.Collection
{
    public class ReducedCollection<TModel, TValue> : CollectionBase<TModel, TValue>
    {
        public ReducedCollection(string collectionNameRaw, ConnectionManager connectionManager,
            CollectionManager collectionManager, List<IPrefilter> prefilters,
            IObservable<InfoResponse> collectionInformation) : base(collectionNameRaw, connectionManager,
            collectionManager, prefilters, collectionInformation)
        {
        }
    }
}
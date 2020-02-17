using System;
using System.Collections.Generic;
using SapphireDb_Net.Collection.Prefilter;
using SapphireDb_Net.Command.Info;
using SapphireDb_Net.Connection;

namespace SapphireDb_Net.Collection
{
    public class OrderedCollection<T> : DefaultCollection<T>
    {
        public OrderedCollection(string collectionNameRaw, ConnectionManager connectionManager,
            CollectionManager collectionManager, List<IPrefilter> prefilters,
            IObservable<InfoResponse> collectionInformation) : base(collectionNameRaw, connectionManager,
            collectionManager, prefilters, collectionInformation)
        {
        }
        
        /// <summary>
        /// Apply additional ordering to the collection without effecting previous order
        /// </summary>
        /// <param name="property">The name of the property to order by</param>
        /// <param name="direction">The direction of ordering</param>
        /// <returns></returns>
        public OrderedCollection<T> ThenOrderBy(string property, SortDirection direction = SortDirection.Ascending)
        {
            return _collectionManager.GetCollection<T, List<T>>($"{_contextName}.{_collectionName}", _prefilters,
                new ThenOrderByPrefilter<T>(property, direction));
        }
    }
}
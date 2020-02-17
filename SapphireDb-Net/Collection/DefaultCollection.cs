using System;
using System.Collections.Generic;
using SapphireDb_Net.Collection.Prefilter;
using SapphireDb_Net.Command.Info;
using SapphireDb_Net.Connection;

namespace SapphireDb_Net.Collection
{
    public class DefaultCollection<T> : CollectionBase<T, List<T>>
    {
        public DefaultCollection(string collectionNameRaw, ConnectionManager connectionManager,
            CollectionManager collectionManager, List<IPrefilter> prefilters,
            IObservable<InfoResponse> collectionInformation) : base(collectionNameRaw, connectionManager,
            collectionManager, prefilters, collectionInformation)
        {
        }
        
        /// <summary>
        /// Skip a number of entries
        /// </summary>
        /// <param name="number">Number of entries to skip</param>
        public DefaultCollection<T> Skip(int number)
        {
            return _collectionManager.GetCollection<T>($"{_contextName}.{_collectionName}", _prefilters,
                new SkipPrefilter<T>(number));
        }
        
        /// <summary>
        /// Take a number of entries
        /// </summary>
        /// <param name="number">Number of entries to take</param>
        public DefaultCollection<T> Take(int number)
        {
            return _collectionManager.GetCollection<T>($"{_contextName}.{_collectionName}", _prefilters,
                new TakePrefilter<T>(number));
        }
        
        /// <summary>
        /// Filter the data to query
        /// </summary>
        /// <param name="conditions">The array of conditions for the filter operation</param>
        public DefaultCollection<T> Where(object[] conditions)
        {
            return _collectionManager.GetCollection<T>($"{_contextName}.{_collectionName}", _prefilters,
                new WherePrefilter<T>(conditions));
        }
        
        /// <summary>
        /// Apply ordering to the collection
        /// </summary>
        /// <param name="property">The name of the property to order by</param>
        /// <param name="direction">The direction of ordering</param>
        /// <returns></returns>
        public OrderedCollection<T> OrderBy(string property, SortDirection direction = SortDirection.Ascending)
        {
            return _collectionManager.GetCollection<T>($"{_contextName}.{_collectionName}", _prefilters,
                new OrderByPrefilter<T>(property, direction));
        }
    }
}
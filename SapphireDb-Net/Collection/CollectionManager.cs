using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SapphireDb_Net.Collection.Prefilter;
using SapphireDb_Net.Connection;

namespace SapphireDb_Net.Collection
{
    public class CollectionManager
    {
        private readonly ConnectionManager _connectionManager;
        private readonly CollectionInformationManager _collectionInformationManager;

        public CollectionManager(
            ConnectionManager connectionManager,
            CollectionInformationManager collectionInformationManager)
        {
            _connectionManager = connectionManager;
            _collectionInformationManager = collectionInformationManager;
        }

        public dynamic GetCollection<TModel, TValue>(string collectionNameRaw, List<IPrefilter> prefilters,
            IPrefilter newPrefilter = null)
        {
            List<IPrefilter> newPrefilters = prefilters.ToList();

            if (newPrefilter != null)
            {
                newPrefilters.Add(newPrefilter);
            }

            if (newPrefilter is IAfterQueryPrefilter)
            {
                return new ReducedCollection<TModel, TValue>(collectionNameRaw, _connectionManager, this, newPrefilters,
                    _collectionInformationManager.GetCollectionInformation(collectionNameRaw));
            }

            if (newPrefilter is OrderByPrefilter<TModel>)
            {
                return new OrderedCollection<TModel>(collectionNameRaw, _connectionManager, this, newPrefilters,
                    _collectionInformationManager.GetCollectionInformation(collectionNameRaw));
            }

            return new DefaultCollection<TModel>(collectionNameRaw, _connectionManager, this, newPrefilters,
                _collectionInformationManager.GetCollectionInformation(collectionNameRaw));
        }
    }
}
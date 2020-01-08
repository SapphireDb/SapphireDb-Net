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

        public dynamic GetCollection<T>(string collectionNameRaw, List<IPrefilter> prefilters,
            IPrefilter newPrefilter = null)
        {
            List<IPrefilter> newPrefilters = prefilters.ToList();

            if (newPrefilter != null)
            {
                newPrefilters.Add(newPrefilter);
            }

            // if (newPrefilter is IAfterQueryPrefilter)
            // {

            // }

            // if (newPrefilter is OrderByPrefilter)
            // {

            // }

            return new DefaultCollection<T>(collectionNameRaw, _connectionManager, this, prefilters,
                _collectionInformationManager.GetCollectionInformation(collectionNameRaw));
        }
    }
}
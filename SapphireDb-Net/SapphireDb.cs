using System.Collections.Generic;
using System.Threading.Tasks;
using SapphireDb_Net.Collection;
using SapphireDb_Net.Collection.Prefilter;
using SapphireDb_Net.Connection;
using SapphireDb_Net.Options;

namespace SapphireDb_Net
{
    public class SapphireDb
    {
        private readonly ConnectionManager _connectionManager;
        private readonly CollectionInformationManager _collectionInformationManager;
        private readonly CollectionManager _collectionManager;
        
        public SapphireDb(SapphireDbOptions options)
        {
            _connectionManager = new ConnectionManager(options);
            _collectionInformationManager = new CollectionInformationManager(_connectionManager);
            _collectionManager = new CollectionManager(_connectionManager, _collectionInformationManager);
        }

        public DefaultCollection<T> Collection<T>(string collectionName)
        {
            return (DefaultCollection<T>)_collectionManager.GetCollection<T>(collectionName, new List<IPrefilter>(), null);
        }
    }
}
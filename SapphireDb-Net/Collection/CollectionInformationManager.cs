using System;
using System.Collections.Concurrent;
using SapphireDb_Net.Command.Info;
using SapphireDb_Net.Connection;

namespace SapphireDb_Net.Collection
{
    public class CollectionInformationManager
    {
        private readonly ConnectionManager _connectionManager;
        private readonly ConcurrentDictionary<string, IObservable<InfoResponse>> _collectionInformation = new ConcurrentDictionary<string, IObservable<InfoResponse>>();

        public CollectionInformationManager(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }
        
        
    }
}
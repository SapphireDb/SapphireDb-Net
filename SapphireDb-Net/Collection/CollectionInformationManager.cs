using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using SapphireDb_Net.Command.Info;
using SapphireDb_Net.Connection;
using SapphireDb_Net.Helper;

namespace SapphireDb_Net.Collection
{
    public class CollectionInformationManager
    {
        private readonly ConnectionManager _connectionManager;
        private readonly ConcurrentDictionary<string, IObservable<InfoResponse>> _collectionInformation
            = new ConcurrentDictionary<string, IObservable<InfoResponse>>();

        public CollectionInformationManager(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public IObservable<InfoResponse> GetCollectionInformation(string collectionNameRaw)
        {
            Tuple<string, string> collectionNameParsed = collectionNameRaw.ParseCollectionName();
            string collectionName = collectionNameParsed.Item1;
            string contextName = collectionNameParsed.Item2;

            if (_collectionInformation.TryGetValue($"{contextName}.{collectionName}",
                out IObservable<InfoResponse> infoResponse))
            {
                return infoResponse.Take(1);
            }

            ReplaySubject<InfoResponse> subject = new ReplaySubject<InfoResponse>(1);
            _collectionInformation.TryAdd($"{contextName}.{collectionName}", subject);

            _connectionManager.SendCommand(new InfoCommand(collectionName, contextName))
                .Subscribe(response =>
                {
                    subject.OnNext((InfoResponse)response);
                }, error =>
                {
                    subject.OnError(error);
                });

            return subject.Take(1);
        }
    }
}
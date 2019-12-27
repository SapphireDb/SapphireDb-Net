using System;
using System.Reactive.Subjects;
using SapphireDb_Net.Options;

namespace SapphireDb_Net.Connection
{
    public abstract class ConnectionBase
    {
        protected ConnectionBase(SapphireDbOptions options,  String authToken)
        {
            Options = options;
            AuthToken = authToken;
        }
        
        public Action<object> ConnectionResponseHandler;
        public Action OpenHandler;
        public Action<object> MessageHandler;
        
        public BehaviorSubject<ConnectionState> ReadyState = new BehaviorSubject<ConnectionState>(ConnectionState.Disconnected);

        public SapphireDbOptions Options;
        public string AuthToken;

        public abstract void Send(object command, bool storedCommand);
        
        public abstract void DataUpdated();
    }

    public enum ConnectionState
    {
        Disconnected, Connecting, Connected
    }
}
using System;
using System.Reactive.Subjects;
using SapphireDb_Net.Command;
using SapphireDb_Net.Command.Connection;
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
        
        public Action<ConnectionResponse> ConnectionResponseHandler;
        public Action OpenHandler;
        public Action<ResponseBase> MessageHandler;
        
        public BehaviorSubject<ConnectionState> ReadyState = new BehaviorSubject<ConnectionState>(ConnectionState.Disconnected);

        public SapphireDbOptions Options;
        public string AuthToken;

        public abstract void Send(CommandBase command, bool storedCommand);
        
        public abstract void DataUpdated();
    }

    public enum ConnectionState
    {
        Disconnected, Connecting, Connected
    }
}
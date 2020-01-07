using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using SapphireDb_Net.Command;
using SapphireDb_Net.Command.Connection;
using SapphireDb_Net.Options;

namespace SapphireDb_Net.Connection
{
    public abstract class ConnectionBase
    {
        public Action<ResponseBase> MessageHandler;
        
        public readonly BehaviorSubject<ConnectionInformation> ConnectionInformation =
            new BehaviorSubject<ConnectionInformation>(new ConnectionInformation());

        public abstract void Send(CommandBase command, bool storedCommand);
        public abstract void SetData(SapphireDbOptions options, string authToken = null);

        public void UpdateConnectionInformation(ConnectionState readyState, Guid connectionId)
        {
            ConnectionInformation connectionInformation = ConnectionInformation.Value;
            connectionInformation.ReadyState = readyState;
            connectionInformation.ConnectionId = connectionId;
            ConnectionInformation.OnNext(connectionInformation);
        }
    }

    public enum ConnectionState
    {
        Disconnected, Connecting, Connected
    }
    
    public class ConnectionInformation
    {
        public ConnectionState ReadyState { get; set; }

        public Guid ConnectionId { get; set; }
    }
}
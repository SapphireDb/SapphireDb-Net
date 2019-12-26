using System;
using System.Reactive;
using System.Reactive.Linq;
using SapphireDb_Net.Options;

namespace SapphireDb_Net.Connection
{
    public class WebsocketConnection : ConnectionBase
    {
        public WebsocketConnection(SapphireDbOptions options, string bearer) : base(options, bearer)
        {
        }

        private IObservable<ConnectionState> Connect()
        {
            ReadyState.Take(1).Subscribe((state) =>
            {
                if (state == ConnectionState.Disconnected)
                {
                    ReadyState.Next();
                }
            });

            return ReadyState.AsObservable();
        }
        
        public override void Send(object command, bool storedCommand)
        {
            throw new System.NotImplementedException();
        }

        public override void DataUpdated()
        {
            throw new System.NotImplementedException();
        }
    }
}
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using SapphireDb_Net.Helper;
using SapphireDb_Net.Options;
using Websocket.Client;

namespace SapphireDb_Net.Connection
{
    public class WebsocketConnection : ConnectionBase
    {
        private ManualResetEvent exitEvent = new ManualResetEvent(false);
        private WebsocketClient _websocketClient;
        // private ClientWebSocket _webSocket;
        
        public WebsocketConnection(SapphireDbOptions options, string authToken) : base(options, authToken)
        {
        }

        private IObservable<ConnectionState> Connect()
        {
            Task.Run(() =>
            {
                ReadyState.Take(1).Subscribe(async (state) =>
                {
                    if (state == ConnectionState.Disconnected)
                    {
                        ReadyState.OnNext(ConnectionState.Connecting);
                        Uri url = new Uri(CreateConnectionString());

                        using (_websocketClient = new WebsocketClient(url))
                        {
                            _websocketClient.ReconnectTimeout = TimeSpan.FromSeconds(30);

                            _websocketClient.DisconnectionHappened.Subscribe((info) =>
                            {
                                ReadyState.OnNext(ConnectionState.Disconnected);
                            });
                        
                            _websocketClient.ReconnectionHappened.Subscribe((info) =>
                            {
                                ReadyState.OnNext(ConnectionState.Connected);
                            });
                        
                            _websocketClient.MessageReceived.Subscribe(message =>
                            {
                                Console.WriteLine(message);
                            });

                            _ = _websocketClient.Start();
                            exitEvent.WaitOne();
                        }
                    }
                });
            });

            return ReadyState.AsObservable();
        }
        
        public override void Send(object command, bool storedCommand)
        {
            if (storedCommand && ReadyState.Value != ConnectionState.Connected) {
                return;
            }

            Connect()
                .TakeWhile(state => state != ConnectionState.Disconnected || !storedCommand)
                .Where(state => state == ConnectionState.Connected && _websocketClient.IsRunning)
                .Take(1)
                .Subscribe((state) =>
                {
                    _websocketClient.Send(command.ToString());
                });
        }

        public override void DataUpdated()
        {
            exitEvent.Set();
            // if (_webSocket != null && (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.Connecting))
            // {
            //     _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Data updated", CancellationToken.None);
            // }
        }
        
        private string CreateConnectionString()
        {
            string urlPrefix = Options.UseSsl ? "wss" : "ws";
            string url = $"{urlPrefix}://{Options.ServerBaseUrl}/sapphire/socket?";

            if (!string.IsNullOrEmpty(Options.ApiSecret) && !string.IsNullOrEmpty(Options.ApiKey)) {
                url += $"key={Options.ApiKey}&secret={Options.ApiSecret}&";
            }
            
            if (!string.IsNullOrEmpty(AuthToken)) {
                url += $"bearer={AuthToken}";
            }

            return url;
        }
    }
}
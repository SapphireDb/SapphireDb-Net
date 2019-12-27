using System;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using SapphireDb_Net.Helper;
using SapphireDb_Net.Options;

namespace SapphireDb_Net.Connection
{
    public class WebsocketConnection : ConnectionBase
    {
        private ClientWebSocket _webSocket;
        
        public WebsocketConnection(SapphireDbOptions options, string authToken) : base(options, authToken)
        {
        }

        private IObservable<ConnectionState> Connect()
        {
            ReadyState.Take(1).Subscribe(async (state) =>
            {
                if (state == ConnectionState.Disconnected)
                {
                    ReadyState.OnNext(ConnectionState.Connecting);
                    string connectionString = CreateConnectionString();
                    
                    _webSocket = new ClientWebSocket();

                    _ = Task.Run(async () =>
                    {
                        while (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.Connecting)
                        {
                            try
                            {
                                string message = await _webSocket.Receive();

                                if (!string.IsNullOrEmpty(message))
                                {
                                    Console.WriteLine(message);
                                }
                            }
                            catch (Exception ex)
                            {
                                ReadyState.OnNext(ConnectionState.Disconnected);
                                
                                Thread.Sleep(1000);
                                Connect();
                            }
                        }

                    });
                    
                    await _webSocket.ConnectAsync(new Uri(connectionString), CancellationToken.None);

                }
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
                .Where(state => state == ConnectionState.Connected && _webSocket.State == WebSocketState.Open)
                .Take(1)
                .Subscribe((state) =>
                {
                    _ = _webSocket.Send(command.ToString());
                });
        }

        public override void DataUpdated()
        {
            if (_webSocket != null && (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.Connecting))
            {
                _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Data updated", CancellationToken.None);
            }
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
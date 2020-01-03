using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using SapphireDb_Net.Command;
using SapphireDb_Net.Command.Connection;
using SapphireDb_Net.Helper;
using SapphireDb_Net.Options;
using Websocket.Client;

namespace SapphireDb_Net.Connection
{
    public class WebsocketConnection : ConnectionBase
    {
        private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);
        private WebsocketClient _websocketClient;

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
                            _websocketClient.ReconnectTimeout = null;

                            _websocketClient.DisconnectionHappened.Subscribe((info) =>
                            {
                                ReadyState.OnNext(ConnectionState.Disconnected);
                                Thread.Sleep(1000);
                                Connect();
                            });

                            _websocketClient.MessageReceived.Subscribe(message =>
                            {
                                ResponseBase response = JsonHelper.DeserializeResponse(message.Text);

                                if (response is ConnectionResponse connectionResponse)
                                {
                                    // ConnectionResponseHandler(connectionResponse);
                                    ReadyState.OnNext(ConnectionState.Connected);
                                    // OpenHandler();
                                }
                                else
                                {
                                    // MessageHandler(response);
                                }
                                Console.WriteLine(message);
                            });

                            await _websocketClient.Start();
                            _exitEvent.WaitOne();
                        }
                    }
                });
            });

            return ReadyState.AsObservable();
        }
        
        public override void Send(CommandBase command, bool storedCommand)
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
                    _websocketClient.Send(JsonHelper.Serialize(command));
                });
        }

        public override void DataUpdated()
        {
            Task.Run(async () =>
            {
                if (_websocketClient != null)
                {
                    _websocketClient.Url = new Uri(CreateConnectionString());
                    await _websocketClient.Reconnect();   
                }
            });
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
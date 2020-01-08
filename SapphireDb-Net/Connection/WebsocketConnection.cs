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
        private string _socketConnectionString;
        
        private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);
        private WebsocketClient _websocketClient;

        private void Connect()
        {
            Task.Run(() =>
            {
                ConnectionInformation.Take(1).Subscribe(async (connectionInfo) =>
                {
                    if (connectionInfo.ReadyState == ConnectionState.Disconnected)
                    {
                        UpdateConnectionInformation(ConnectionState.Connecting, Guid.Empty);

                        Uri url = new Uri(_socketConnectionString);

                        using (_websocketClient = new WebsocketClient(url))
                        {
                            _websocketClient.ReconnectTimeout = null;

                            _websocketClient.DisconnectionHappened.Subscribe((info) =>
                            {
                                UpdateConnectionInformation(ConnectionState.Disconnected, Guid.Empty);
                                Thread.Sleep(1000);
                                Connect();
                            });

                            _websocketClient.MessageReceived.Subscribe(message =>
                            {
                                ResponseBase response = JsonHelper.DeserializeResponse(message.Text);

                                if (response is ConnectionResponse connectionResponse)
                                {
                                    UpdateConnectionInformation(ConnectionState.Connected, Guid.Empty);
                                }
                                else
                                {
                                    MessageHandler(response);
                                }
                                Console.WriteLine(message);
                            });

                            await _websocketClient.Start();
                            _exitEvent.WaitOne();
                        }
                    }
                });
            });
        }
        
        public override void Send(CommandBase command, bool storedCommand)
        {
            ConnectionInformation
                .TakeWhile(connectionInformation => connectionInformation.ReadyState != ConnectionState.Disconnected || !storedCommand)
                .Where(connectionInformation => connectionInformation.ReadyState == ConnectionState.Connected)
                .Take(1)
                .Subscribe((state) =>
                {
                    _websocketClient.Send(JsonHelper.Serialize(command));
                });
        }

        public override void SetData(SapphireDbOptions options, string authToken = null)
        {
            Task.Run(async () =>
            {
                CreateConnectionString(options, authToken);
                
                if (_websocketClient == null)
                {
                    Connect();
                }
                else
                {
                    _websocketClient.Url = new Uri(_socketConnectionString);
                    await _websocketClient.Reconnect();   
                }
            });
        }
        
        private void CreateConnectionString(SapphireDbOptions options, string authToken = null)
        {
            string urlPrefix = options.UseSsl ? "wss" : "ws";
            string url = $"{urlPrefix}://{options.ServerBaseUrl}/sapphire/socket?";

            if (!string.IsNullOrEmpty(options.ApiSecret) && !string.IsNullOrEmpty(options.ApiKey)) {
                url += $"key={options.ApiKey}&secret={options.ApiSecret}&";
            }
            
            if (!string.IsNullOrEmpty(authToken)) {
                url += $"bearer={authToken}";
            }

            _socketConnectionString = url;
        }
    }
}
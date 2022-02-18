using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace mqtt
{
	public class MessageClient
	{
        MqttClientOptionsBuilder _builder;
        ManagedMqttClientOptions _options;
        IManagedMqttClient _mqttClient;


        Logger _logger;
        int _port = 707;
        int _reconnectDelay = 60;
        string _server;


        int MessageCounter { get; set; }

        // the id , name of this specifc client
        public string ClientId { get; set; }


        public MessageClient(string clientId, string tcpServer = "localhost", int port = 707,bool connect = true)
		{
            // get logger to work 
            _logger = Logger.GetInstance;

            ClientId = clientId;
            _server = tcpServer;
            _port = port;

            _logger.Debug($"Start builder with clientID {ClientId} server {_server} port {_port}");
            _builder = new MqttClientOptionsBuilder()
                                                    .WithClientId(ClientId)
                                                    .WithTcpServer(_server, _port);

            _logger.Debug($"Setup options with reconnectDelay {_reconnectDelay} sec");
            _options = new ManagedMqttClientOptionsBuilder()
                                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(_reconnectDelay))
                                    .WithClientOptions(_builder.Build())
                                    .Build();
            _mqttClient = new MqttFactory().CreateManagedMqttClient();
            _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);
            _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnMessageReceived);
       
            
            //_mqttClient.ApplicationMessageReceivedHandler = new ApplicationMessage
            // start connection if requested
            if (connect)
                Connect();

        }

        public void Connect()
        {
            _logger.Debug($"Start connection...");
            _mqttClient.StartAsync(_options).GetAwaiter().GetResult();
        }

        public void OnMessageReceived(MqttApplicationMessageReceivedEventArgs obj)
        {
            _logger.Information("Message received.");
        }

        public void OnConnected(MqttClientConnectedEventArgs obj)
        {
            _logger.Information("Successfully connected.");
        }

        public void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {
            _logger.Warning("Couldn't connect to broker.");
        }

        public void OnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            _logger.Information("Successfully disconnected.");
        }

        public void Publish(string topic, string message, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.ExactlyOnce)
        {
            _logger.Information($"Publish message to topic {topic}");
            _mqttClient.PublishAsync(topic, message, qos);
        }

        public void Subscribe(string topic)
        {
            MqttTopicFilter filter = new MqttTopicFilterBuilder()
                        .WithTopic(topic)
                        .Build();

            _mqttClient.SubscribeAsync(new MqttTopicFilter[] { filter });

        } 
    }
}


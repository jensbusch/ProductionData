using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

// other pdcore components
using PDCore.Logger;

namespace PDCore.MqTT
{


    public class MessageClient
	{
        MqttClientOptionsBuilder _builder;
        ManagedMqttClientOptions _options;
        IManagedMqttClient _mqttClient;


        ILogger _logger;
        int _port = 707;
        int _reconnectDelay = 60;
        string _server;


        int MessageCounter { get; set; }

        // the id , name of this specifc client
        public string ClientId { get; set; }

        // delegates
        public delegate void MqttClientConnectedDelegate(MqttClientConnectedEventArgs obj);
        public delegate void MqttClientDisconnectedDelegate(MqttClientDisconnectedEventArgs obj);
        public delegate void MqttClientConnectingFailedDelegate(ManagedProcessFailedEventArgs obj);
        public delegate void MqttApplicationMessageReceivedDelegate(MqttApplicationMessageReceivedEventArgs obj);

        // events
        public event MqttClientConnectedDelegate? OnConnected;
        public event MqttClientDisconnectedDelegate? OnDisconnected;
        public event MqttClientConnectingFailedDelegate? OnConnectingFailed;
        public event MqttApplicationMessageReceivedDelegate? OnMessageReceived;

        public MessageClient(string clientId, string tcpServer = "localhost", int port = 707,bool connect = true)
		{
            // get logger to work 
            _logger = ConsoleLogger.GetInstance;

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
            _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(DoOnConnected);
            _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(DoOnDisconnected);
            _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(DoOnConnectingFailed);
            _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(DoOnMessageReceived);
       
            
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

        private void DoOnMessageReceived(MqttApplicationMessageReceivedEventArgs obj)
        {
            if (OnMessageReceived != null)
                OnMessageReceived(obj);
            else
                _logger.Information("Message received.");
        }

        private void DoOnConnected(MqttClientConnectedEventArgs obj)
        {
            if (OnConnected != null)
                OnConnected(obj);
            else
                _logger.Information("Successfully connected.");
        }

        private void DoOnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {

            if (OnConnectingFailed != null)
                OnConnectingFailed(obj);
            else
                _logger.Warning("Couldn't connect to broker.");
        }

        private void DoOnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            if (OnDisconnected != null)
                OnDisconnected(obj);
            else
                _logger.Information("Successfully disconnected.");
        }

        // overloaded publish messages for more convenient programming
        public void Publish(string topic, string message, string responseTopic)
        {
            Publish(topic, message, false, responseTopic);
        }

        public void Publish(string topic, string message, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.ExactlyOnce)
        {
            Publish(topic, message, false, "", qos);
        }

        // sends a messages and stores the last version for any new conecting client.
        public void Publish(string topic, string message,
            bool retainFlag = true, string responseTopic = "", MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.ExactlyOnce)
        {
            _logger.Information($"Publish message to topic {topic}");
            var mqttMsg = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(message)
            .WithQualityOfServiceLevel(qos)
            .WithResponseTopic(responseTopic)
            .WithRetainFlag(retainFlag)
            .Build();
            _mqttClient.PublishAsync(mqttMsg);

        }


        public void Publish(params MqttApplicationMessage[] mqttApplicationMessages)
        {
            //var message = new MqttApplicationMessageBuilder()
            //            .WithTopic(context.ApplicationMessage?.ResponseTopic)
            //            .WithPayload("published message received")
            //            .WithExactlyOnceQoS()
            //            .WithRetainFlag()
            //            .Build();
            _mqttClient.PublishAsync(mqttApplicationMessages);

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


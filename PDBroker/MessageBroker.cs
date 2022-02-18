using System;
using System.Text;
using MQTTnet;
using MQTTnet.Server;

namespace mqtt
{
	public class MessageBroker
	{
        readonly MqttServerOptionsBuilder _options;
        Logger _logger;
		int _brokerPort = 707;
        IMqttServer _mqttServer;

        int MessageCounter { get; set; }


        public MessageBroker(int brokerPort = 707, bool start = true)
		{
            _logger = Logger.GetInstance;
            _brokerPort = brokerPort;
			_options = new MqttServerOptionsBuilder()
                            // set endpoint to localhost
							.WithDefaultEndpoint()
                            // set used port
							.WithDefaultEndpointPort(_brokerPort)
                            // new connection handler
							.WithConnectionValidator(OnNewConnection)
                            // new message handler
                            .WithApplicationMessageInterceptor(OnNewMessage);
            // creates a new mqtt server     
             _mqttServer = new MqttFactory().CreateMqttServer();
            if (start)
                Start();
        }

        public void Start()
        {
            _logger.Information("Start the broker...");
            // start the server with options  
            _mqttServer?.StartAsync(_options.Build()).GetAwaiter().GetResult();
        }

        public void OnNewConnection(MqttConnectionValidatorContext context)
        {
            _logger.Information(
                    "New connection: ClientId = {0}, Endpoint = {1}",
                    context.ClientId,
                    context.Endpoint);
        }

        public void OnNewMessage(MqttApplicationMessageInterceptorContext context)
        {
            string? payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage.Payload);

            MessageCounter++;

            _logger.Information(
                "MessageId: {0} - TimeStamp: {1} -- Message: ClientId = {2}, Topic = {3}, Payload = {4}, QoS = {5}, Retain-Flag = {6}",
                MessageCounter,
                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);

            var message = new MqttApplicationMessageBuilder()
                    .WithTopic("response/"+context.ApplicationMessage?.Topic)
                    .WithPayload("published message received")
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();
            _mqttServer.PublishAsync(message);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Newtonsoft;
using Newtonsoft.Json;

// referenced pdcore lib
using PDCore.Logger;
using PDCore.MqTT;

namespace PDClient // Note: actual namespace depends on the project name.
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to ProductionData (MQTT) Client");
            ILogger _logger = ConsoleLogger.GetInstance;

            string _testTopic = "topic/1";
            string _testResponseTopic = "response/topic/1";
            string _clientId = string.Format($"client{new Random().Next(1000,9999)}");
           
            
            

            _logger.Information($"Create {_clientId}");
            MessageClient _client = new MessageClient(_clientId);

            _logger.Information("subscribe for the response of the topic");
            _client.Subscribe(_testTopic);

            Stopwatch stopWatch = new Stopwatch();
            bool exit = false;

            Console.WriteLine("Press 'quit' to terminat the broker");
            while (!exit)
            {
                string? text = Console.ReadLine();
                exit = text?.ToLower() == "quit";
                if (!exit)
                {
                    int x = 100000;
                    _logger.Information("disable logging for send cycle");
                    _logger.LogLevel = Level.silence;
                    Random random = new Random();
                    stopWatch.Start();
                    for(int i = 0; i < x; i++)
                    {
                        Thread.Sleep(random.Next(0, 100));
                        string payload = JsonConvert.SerializeObject(
                            new {
                                sender = _client.ClientId,
                                message = text,
                                msgNumber = i,
                                sent = DateTimeOffset.UtcNow }
                                ); ;
                        _client.Publish(_testTopic, payload, _testResponseTopic);

                    }
                    stopWatch.Stop();
                    _logger.Information($"Enable logging. {x} messages have been sent");
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;
                    _logger.LogLevel = Level.info;
                    _logger.Information($"it took for {ts} for {x} executions. That is {ts / x} for one");
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using mqtt;
using Newtonsoft;
using Newtonsoft.Json;

namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Logger _logger = Logger.GetInstance;

            string _testTopic = "topic/1";
            _logger.Information("Create messagebroker");
            MessageBroker _broker = new MessageBroker();

            _logger.Information("Create client1");
            MessageClient _client = new MessageClient("Client1");
            _client.Subscribe("response/"+_testTopic);

            Stopwatch stopWatch = new Stopwatch();
            bool exit = false;
            while (!exit)
            {
                int x = 1;
                string? text = Console.ReadLine();
                exit = text?.ToLower() == "q";
                if (!exit)
                {
                    _logger.LogLevel = Level.silence;
                    stopWatch.Start();
                    for(int i = 0; i < x; i++)
                    {
                        string payload = JsonConvert.SerializeObject(
                            new {
                                sender = _client.ClientId,
                                message = text,
                                msgNumber = i,
                                sent = DateTimeOffset.UtcNow }
                                ); ;
                        _client.Publish(_testTopic, payload);

                    }
                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;
                    _logger.LogLevel = Level.info;
                    _logger.Information($"it took for {ts} for {x} executions. That is {ts / x} for one");
                }
            }
        }
    }
}
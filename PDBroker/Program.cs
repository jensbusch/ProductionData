using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft;
using Newtonsoft.Json;

// referenced pdcore lib
using PDCore.Logger;
using PDCore.MqTT;

namespace PDBroker // Note: actual namespace depends on the project name.
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to ProductionData (MQTT) Broker");
            ILogger _logger = ConsoleLogger.GetInstance;

            _logger.Information("Create messagebroker");
            MessageBroker _broker = new MessageBroker();

            bool exit = false;

            Console.WriteLine("Type 'quit' + return to terminat the broker");
            while (!exit)
            {
                string? text = Console.ReadLine();
                exit = text?.ToLower() == "quit";
                if (!exit)
                    Thread.Sleep(0);
            }
            Console.WriteLine("Terminate production data broker");
        }
    }
}

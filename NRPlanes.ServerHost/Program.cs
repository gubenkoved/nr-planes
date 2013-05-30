using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NRPlanes.Server;
using System.ServiceModel.Description;


namespace NRPlanes.ServerHost
{
    class Program
    {
        static private GameService _service;

        static void Main(string[] args)
        {
            // HOST APPLICATION FOR GAME SERVICE

            _service = new GameService(s => Console.WriteLine(s));

            using (ServiceHost host = new ServiceHost(_service))
            {
                ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();

                // if not found - add behavior with setting turned on 
                if (debug == null)
                {
                    host.Description.Behaviors.Add(
                         new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
                }
                else
                {
                    // make sure setting is turned ON
                    if (!debug.IncludeExceptionDetailInFaults)
                    {
                        debug.IncludeExceptionDetailInFaults = true;
                    }
                }

                host.Open();

                Console.WriteLine("Service is running...\n---\n");                

                Console.ReadKey();
            } 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NRPlanes.Server;


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
                host.Open();

                Console.WriteLine("Service is running");
                Console.WriteLine("Press any key to shutdown... \n\n");

                Console.ReadKey();
            } 
        }
    }
}

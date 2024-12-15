using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using ProxyCacheServer;

namespace ProxyCacheServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8733/Design_Time_Addresses/ProxyCacheServer/ProxyCacheService/");

            // Create the ServiceHost.
            using (ServiceHost host = new ServiceHost(typeof(ProxyCacheService), baseAddress))
            {
                try
                {
                    // Add a service endpoint.
                    host.AddServiceEndpoint(typeof(IProxyCacheService), new BasicHttpBinding(), "");

                    // Enable metadata exchange.
                    ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (smb == null)
                    {
                        smb = new ServiceMetadataBehavior
                        {
                            HttpGetEnabled = true,
                            HttpsGetEnabled = true
                        };
                        host.Description.Behaviors.Add(smb);
                    }

                    // Open the ServiceHost to start listening for messages.
                    host.Open();

                    Console.WriteLine("The service is ready at {0}", baseAddress);
                    Console.WriteLine("Press <Enter> to stop the service.");
                    Console.ReadLine();

                    // Close the ServiceHost.
                    host.Close();
                }
                catch (CommunicationException ce)
                {
                    Console.WriteLine("An exception occurred: {0}", ce.Message);
                    host.Abort();
                }
            }
        }
    }
}

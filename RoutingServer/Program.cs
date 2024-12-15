using RoutingServer;
using RoutingServer.CORS;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

class Program
{
    static void Main(string[] args)
    {
        string baseAddress = "http://localhost:8733/Design_Time_Addresses/RoutingServer/RoutingService";

        using (WebServiceHost host = new WebServiceHost(typeof(RoutingService), new Uri(baseAddress)))
        {
            ServiceEndpoint endpoint = host.AddServiceEndpoint(
                typeof(IRoutingService),
                new WebHttpBinding(),
                ""
            );

            endpoint.Behaviors.Add(new WebHttpBehavior());

            endpoint.Behaviors.Add(new CorsEndpointBehavior());

            host.Open();
            Console.WriteLine($"Service is running at {baseAddress}");
            Console.WriteLine("Press Enter to terminate.");
            Console.ReadLine();
        }
    }
}

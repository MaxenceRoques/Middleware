using System;
using System.ServiceModel.Configuration;


namespace RoutingServer.CORS
{
    public class CorsEndpointBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType => typeof(CorsEndpointBehavior);

        protected override object CreateBehavior()
        {
            return new CorsEndpointBehavior();
        }
    }
}

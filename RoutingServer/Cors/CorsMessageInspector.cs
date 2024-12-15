namespace RoutingServer.CORS
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    public class CorsMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (reply.Properties.ContainsKey(HttpResponseMessageProperty.Name))
            {
                var httpResponse = (HttpResponseMessageProperty)reply.Properties[HttpResponseMessageProperty.Name];
                httpResponse.Headers["Access-Control-Allow-Origin"] = "*";
                httpResponse.Headers["Access-Control-Allow-Methods"] = "GET";
                httpResponse.Headers["Access-Control-Allow-Headers"] = "Content-Type, Accept, Authorization";
            }
        }
    }
}
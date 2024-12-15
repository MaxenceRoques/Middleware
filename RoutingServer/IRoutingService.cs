using SharedModels.models;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace RoutingServer
{
    [ServiceContract]
    public interface IRoutingService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetItinerary?startLat={startLat}&startLon={startLon}&endLat={endLat}&endLon={endLon}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]

        Task<List<Itinerary>> GetItinerary(double startLat, double startLon, double endLat, double endLon);
    }

}

using SharedModels.models;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace ProxyCacheServer
{
    [ServiceContract]
    public interface IProxyCacheService
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetContracts")]
        Task<List<Contract>> GetContracts();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetStations?contractName={contractName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Task<List<Station>> GetStations(string contractName);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetGeocode/{cityName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Task<Position> GetGeocode(string cityName);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetRoute?startLat={startLat}&startLon={startLon}&endLat={endLat}&endLon={endLon}&profile={profile}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Task<(Itinerary, Instructions)> GetRoute(double startLat, double startLon, double endLat, double endLon, string profile);
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetAllStations", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Task<List<Station>> GetAllStations();
    }
}

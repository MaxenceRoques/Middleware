using RoutingServer.ProxyCacheServiceReference;
using RoutingServer.Utils;
using SharedModels.models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace RoutingServer
{
    public class RoutingService : IRoutingService
    {
        public static ProxyCacheServiceClient proxyCacheServiceClient = new ProxyCacheServiceClient();
        public async Task<List<Itinerary>> GetItinerary(double startLat, double startLon, double endLat, double endLon)
        {
            Position origin = new Position(startLat, startLon);
            Position destination = new Position(endLat, endLon);

            Task<Station> nearStartStationTask;
            Task<Station> nearEndStationTask;

            try
            {
                nearStartStationTask = RoutingUtils.GetClosestStation(origin, Utils.ActionEnum.PickUp);
                nearEndStationTask = RoutingUtils.GetClosestStation(destination, Utils.ActionEnum.DropOff);
            }
            catch (System.Exception e)
            {
                throw new WebFaultException<string>("Service unavailable", HttpStatusCode.ServiceUnavailable);
            }

            Station nearStartStation = await nearStartStationTask;
            Station nearEndStation = await nearEndStationTask;

            ActiveMQUtils.Initialize();

            try
            {
                if (nearEndStation != null && nearStartStation != null && ComputeUtils.isWorthBiking(origin, destination, nearStartStation, nearEndStation))
                {
                    var walkingToPickupTask = RoutingService.proxyCacheServiceClient.GetRouteAsync(origin.Latitude, origin.Longitude, nearStartStation.Position.Latitude, nearStartStation.Position.Longitude, "foot-walking");
                    var cyclingRouteTask = RoutingService.proxyCacheServiceClient.GetRouteAsync(nearStartStation.Position.Latitude, nearStartStation.Position.Longitude, nearEndStation.Position.Latitude, nearEndStation.Position.Longitude, "cycling-road");
                    var walkingToDestinationTask = RoutingService.proxyCacheServiceClient.GetRouteAsync(nearEndStation.Position.Latitude, nearEndStation.Position.Longitude, destination.Latitude, destination.Longitude, "foot-walking");

                    var itinerariesAndInstructions = await Task.WhenAll(walkingToPickupTask, cyclingRouteTask, walkingToDestinationTask);

                    Itinerary[] itineraries = itinerariesAndInstructions.Select(x => x.Item1).ToArray();
                    Instructions[] allInstructions = itinerariesAndInstructions.Select(x => x.Item2).ToArray();

                    ActiveMQUtils.SendInstructions(allInstructions);

                    return new List<Itinerary> { itineraries[0], itineraries[1], itineraries[2] };
                }

                var (walkingItinerary, walkingInstructions) = RoutingService.proxyCacheServiceClient.GetRoute(origin.Latitude, origin.Longitude, destination.Latitude, destination.Longitude, "foot-walking");

                Instructions[] walkingInstructionsArray = { walkingInstructions };
                ActiveMQUtils.SendInstructions(walkingInstructionsArray);
                return new List<Itinerary> { walkingItinerary };
            }
            catch (System.Exception e)
            {
                throw new WebFaultException<string>(e.Message, HttpStatusCode.NotFound);
            }
        }
    }
}

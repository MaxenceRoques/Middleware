using SharedModels.models;
using System.Device.Location;

namespace RoutingServer.Utils
{
    internal class ComputeUtils
    {
        private const double WALKING_SPEED = 5;
        private const double BIKING_SPEED = 15;

        public static double computeDistance(Position position1, Position position2)
        {
            GeoCoordinate geo1 = new GeoCoordinate(position1.Latitude, position1.Longitude);
            GeoCoordinate geo2 = new GeoCoordinate(position2.Latitude, position2.Longitude);
            return geo1.GetDistanceTo(geo2);
        }


        //Note : this is a very simple implementation of the time computation,
        //in a real world scenario we would need to take into account the terrain, the traffic, the weather, etc.
        public static double ComputeBikingTime(Position position1, Position position2)
        {
            double distance = computeDistance(position1, position2);
            return distance / BIKING_SPEED;
        }

        public static double ComputeWalkingTime(Position position1, Position position2)
        {
            double distance = computeDistance(position1, position2);
            return distance / WALKING_SPEED;
        }

        public static bool isWorthBiking(Position origin, Position destination, Station pickupStation, Station dropOffStation)
        {
            double directWalkingTime = ComputeWalkingTime(origin, destination);

            double walkingToPickupTime = ComputeWalkingTime(origin, pickupStation.Position);

            double bikingTime = ComputeBikingTime(pickupStation.Position, dropOffStation.Position);

            double walkingToDestinationTime = ComputeWalkingTime(dropOffStation.Position, destination);

            double totalBikingTime = walkingToPickupTime + bikingTime + walkingToDestinationTime;

            return totalBikingTime < directWalkingTime;
        }
    }
}

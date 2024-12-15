using RoutingServer.Utils;
using SharedModels.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingServer
{
    internal class RoutingUtils
    {
        public static async Task<string> GetClosestContractName(Position position)
        {
            try
            {
                List<Contract> contracts = RoutingService.proxyCacheServiceClient.GetContracts().ToList();

                var geocodeTasks = contracts.Select(async contract =>
                {
                    try
                    {
                        Position contractPosition = RoutingService.proxyCacheServiceClient.GetGeocode(contract.Name);
                        return (contract.Name, contractPosition);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Unable to get geocode for city {contract.Name}: {e.Message}");
                        return (contract.Name, (Position)null);
                    }
                });

                var geocodeResults = await Task.WhenAll(geocodeTasks);


                double minDistance = double.MaxValue;
                string closestContract = null;

                foreach (var (contractName, contractPosition) in geocodeResults)
                {
                    if (contractPosition != null)
                    {
                        double distance = ComputeUtils.computeDistance(position, contractPosition);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestContract = contractName;
                        }
                    }
                }
                return closestContract;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to get closest station: " + e.Message);
                return null;
            }
        }

        public static async Task<Station> GetClosestStation(Position position, ActionEnum action)
        {
            try
            {
                var stationTask = await RoutingService.proxyCacheServiceClient.GetAllStationsAsync();
                List<Station> stations = stationTask.ToList();

                double minDistance = double.MaxValue;
                Station closestStation = null;
                foreach (var station in stations)
                {
                    double distance = ComputeUtils.computeDistance(position, station.Position);
                    if (action == ActionEnum.PickUp && !station.IsAvailableForPickup())
                    {
                        continue;
                    }
                    if (action == ActionEnum.DropOff && !station.IsAvailableForDropOff())
                    {
                        continue;
                    }
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestStation = station;
                    }
                }
                return closestStation;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to get closest station: " + e.Message);
                return null;
            }

        }

        public static async Task<Station> GetClosestStationFromContract(Position position, string contractName, ActionEnum action)
        {
            try
            {
                var stationTask = await RoutingService.proxyCacheServiceClient.GetAllStationsAsync();
                List<Station> stations = stationTask.ToList();

                double minDistance = double.MaxValue;
                Station closestStation = null;

                foreach (var station in stations)
                {
                    double distance = ComputeUtils.computeDistance(position, station.Position);

                    if (action == ActionEnum.PickUp && !station.IsAvailableForPickup())
                    {
                        continue;
                    }

                    if (action == ActionEnum.DropOff && !station.IsAvailableForDropOff())
                    {
                        continue;
                    }

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestStation = station;
                    }
                }
                return closestStation;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to get closest station: " + e.Message);
                return null;
            }
        }
    }


}

using SharedModels.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace ProxyCacheServer
{
    public class ProxyCacheService : IProxyCacheService
    {
        private static GenericProxyCache<List<Station>> cache = new GenericProxyCache<List<Station>>();
        private static double cacheDurationInSeconds = 300;

        public async Task<List<Contract>> GetContracts()
        {
            try
            {
                List<Contract> contracts = await ApiCalls.GetContracts();
                return contracts;
            }
            catch (Exception)
            {
                throw new WebFaultException<string>("Service unavailable", HttpStatusCode.ServiceUnavailable);
            }
        }

        public async Task<List<Station>> GetStations(string contractName)
        {
            try
            {
                List<Station> stations = await ApiCalls.GetStations(contractName);
                return stations;
            }
            catch (Exception)
            {
                throw new WebFaultException<string>("Service unavailable", HttpStatusCode.ServiceUnavailable);
            }
        }

        public async Task<Position> GetGeocode(string cityName)
        {
            try
            {
                Position position = await ApiCalls.GetGeocode(cityName);
                return position;
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>("Service unavailable", HttpStatusCode.ServiceUnavailable);
            }
        }

        public async Task<(Itinerary, Instructions)> GetRoute(double startLat, double startLon, double endLat, double endLon, string profile)
        {
            try
            {
                var originPosition = new Position(startLat, startLon);
                var destinationPosition = new Position(endLat, endLon);

                var (itinerary, instructions) = await ApiCalls.GetRoute(originPosition, destinationPosition, profile);

                return (itinerary, instructions);
            }
            catch (Exception)
            {
                throw new WebFaultException<string>("Service unavailable", HttpStatusCode.NotFound);
            }
        }

        public async Task<List<Station>> GetAllStations()
        {
            try
            {
                var cacheKey = "AllStations";

                bool isCacheUsed = false;

                var stations = cache.Get(cacheKey, cacheDurationInSeconds);
                Debug.WriteLine($"Nombre d'éléments dans stations : {stations.Count}");

                if (stations == null || !stations.Any())
                {
                    stations = await ApiCalls.getAllStations();

                    cache.Set(cacheKey, stations, cacheDurationInSeconds);
                }
                else
                {
                    isCacheUsed = true;
                }

                Debug.WriteLine(isCacheUsed
                    ? "Cache utilisé pour récupérer les stations."
                    : "Cache non utilisé, données récupérées depuis l'API.");

                return stations;
            }
            catch (Exception)
            {
                throw new WebFaultException<string>("Service unavailable", HttpStatusCode.ServiceUnavailable);
            }
        }






    }
}

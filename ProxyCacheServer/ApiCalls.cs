using Newtonsoft.Json;
using SharedModels.models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProxyCacheServer
{
    public static class ApiCalls
    {
        private static readonly HttpClient client = new HttpClient();
        private const string JCDecauxAPIKey = "35d97b492bae5a505f8ef43fc84a124acc5454e4";
        private const string JCDecauxAPIURL = "https://api.jcdecaux.com/vls/v3/";
        private const string GeoCodingAPIKey = "6ea9f86b05754c24981e2621e420ffac";
        private const string GeoCodingAPIURL = "https://api.opencagedata.com/geocode/v1/";
        private const string ORSAPIKey = "5b3ce3597851110001cf62486ec40209b6914a57958dffb798bf9232";
        private const string ORSURL = "https://api.openrouteservice.org/v2/directions/";


        public static async Task<List<Contract>> GetContracts()
        {
            string url = $"{JCDecauxAPIURL}contracts?apiKey={JCDecauxAPIKey}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Contract>>(responseString);
        }

        public static async Task<List<Station>> GetStations(string contractName)
        {
            string url = $"{JCDecauxAPIURL}stations?contract={contractName}&apiKey={JCDecauxAPIKey}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Station>>(responseString);
        }

        public static async Task<List<Station>> getAllStations()
        {
            string url = $"{JCDecauxAPIURL}stations?apiKey={JCDecauxAPIKey}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Station>>(responseString);
        }


        public static async Task<Position> GetGeocode(string cityName)
        {
            string url = $"{GeoCodingAPIURL}json?q={cityName}&key={GeoCodingAPIKey}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();

            var geoData = JsonConvert.DeserializeObject<dynamic>(responseString);
            if (geoData.results != null && geoData.results.Count > 0)
            {
                double lat = geoData.results[0].geometry.lat;
                double lng = geoData.results[0].geometry.lng;
                return new Position(lat, lng);
            }

            throw new Exception("Geolocation data not found for the given city.");
        }

        public static async Task<(Itinerary, Instructions)> GetRoute(Position origin, Position destination, string profile)
        {
            string url = $"{ORSURL}{profile}/geojson?api_key={ORSAPIKey}";

            var coordinates = new[]
            {
                new[] { origin.Longitude, origin.Latitude },
                new[] { destination.Longitude, destination.Latitude }
            };

            var requestBody = new
            {
                coordinates = coordinates,
                language = "fr"
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();


            var routeData = JsonConvert.DeserializeObject<dynamic>(responseString);

            var totalDistance = (double)routeData.features[0].properties.segments[0].distance;
            var totalDuration = (double)routeData.features[0].properties.segments[0].duration;
            var geometryData = routeData.features[0].geometry.coordinates;
            var coordinatesList = geometryData.ToObject<List<List<double>>>();

            var itinerary = new Itinerary(profile)
            {
                TotalDistance = totalDistance,
                TotalDuration = totalDuration,
                Geometry = new Coordinates { Coord = coordinatesList, Type = "LineString" }
            };

            var instructions = new Instructions(profile);

            foreach (var step in routeData.features[0].properties.segments[0].steps)
            {
                var instruction = step.instruction.ToString();
                var duration = (double)step.duration;
                var distance = (double)step.distance;

                var route = new Route(instruction, duration, distance);
                instructions.Routes.Add(route);
            }

            return (itinerary, instructions);
        }

    }
}

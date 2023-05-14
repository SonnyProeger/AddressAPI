using System;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetEnv;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AddressAPI.Services
{
    public class DistanceMatrixResponse
    {
        public Row[] rows { get; set; }
    }

    public class Row
    {
        public Element[] elements { get; set; }
    }

    public class Element
    {
        public Distance distance { get; set; }
    }

    public class Distance
    {
        public double value { get; set; }
    }

    public class DistanceCalculator
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public DistanceCalculator()
        {
            Env.Load();
            _apiKey = System.Environment.GetEnvironmentVariable("API_KEY");
            _httpClient = new HttpClient();
        }

        public async Task<double> CalculateDistance(string originAddress, string destinationAddress)
        {
            var requestUrl = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={originAddress}&destinations={destinationAddress}&key={_apiKey}";

            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON response
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);

            // Check that the response contains at least one row and one element
            if (jsonResponse["rows"] is JArray rows && rows.Count > 0 &&
                rows[0]["elements"] is JArray elements && elements.Count > 0)
            {
                var distanceInMeters = elements[0]["distance"]["value"].Value<double>();
                var distanceInKilometers = distanceInMeters / 1000;

                return distanceInKilometers;
            }
            else
            {
                throw new Exception("Invalid Google Maps API response: " + responseContent);
            }

        }
    }
}



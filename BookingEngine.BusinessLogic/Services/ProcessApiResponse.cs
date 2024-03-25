using BookingEngine.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BookingEngine.BusinessLogic.Services
{
    public class ProcessApiResponse : IProcessApiResponse
    {
        private ILogger<ProcessApiResponse> _logger;

        public ProcessApiResponse(ILogger<ProcessApiResponse> logger)
        {
            _logger = logger;
        }

        public async Task<T> ProcessResponse<T>(HttpResponseMessage response)
        {
            try
            {
                var contentStream = await response.Content.ReadAsStreamAsync();

                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);

                JsonSerializer serializer = new JsonSerializer();

                var searchResponse = serializer.Deserialize<T>(jsonReader);

                _logger.LogInformation("Response from Amadeus api succesfull. Model fetched: " + searchResponse.ToString());

                return searchResponse;
            }
            catch (Exception e)
            {
                throw new Exception("Could not parse JSON response for Amadeus hotels search.", e);
            }
        }

        public async Task<T> ProcessError<T>(HttpResponseMessage response)
        {
            try
            {
                var contentStream = await response.Content.ReadAsStreamAsync();

                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);

                JsonSerializer serializer = new JsonSerializer();

                var searchResponse = serializer.Deserialize<T>(jsonReader);

                _logger.LogInformation("Response from Amadeus api succesfull. Model fetched: " + searchResponse.ToString());

                return searchResponse;
            }
            catch (Exception e)
            {
                throw new Exception("Could not parse JSON Error for Amadeus hotels search.", e);
            }
        }
    }
}

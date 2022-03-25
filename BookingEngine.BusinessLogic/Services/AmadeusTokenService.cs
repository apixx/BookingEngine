using BookingEngine.BusinessLogic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Options;
using BookingEngine.BusinessLogic.Config;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;

namespace BookingEngine.BusinessLogic.Services
{
    public class AmadeusTokenService : IAmadeusTokenService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptionsMonitor<AmadeusClientOptions> _amadeusClientOptions;
        private readonly ILogger<AmadeusTokenService> _logger;
        private static string _tokenString { get; set; } = "";
        private static DateTime _tokenExpiration { get; set; } = DateTime.MinValue;

        public AmadeusTokenService(IHttpClientFactory clientFactory, IOptionsMonitor<AmadeusClientOptions> amadeusClientOptions, ILogger<AmadeusTokenService> logger)
        {
            _clientFactory = clientFactory;
            _amadeusClientOptions = amadeusClientOptions;
            _logger = logger;
        }

        public async Task<string> GetAmadeusToken(CancellationToken cancellationToken)
        {
            if(String.IsNullOrEmpty(_tokenString) || _tokenExpiration < DateTime.Now)
            {
                var tokenGenerated = await this.GenerateToken(cancellationToken);

                if (!tokenGenerated)
                {
                    _logger.LogError("Access Token could not be generated.");
                    return null;
                }
            }
            return _tokenString;
        }

        /// <summary>
        /// Makes asynchronous POST request to generate access token (Access_token) and it's expiration (Expires_in). Parses the response into object.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a Task</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        private async Task<bool> GenerateToken(CancellationToken cancellationToken)
        {
            // Creates a new HTTPClient using the default Configuration
            var client = _clientFactory.CreateClient();

            // Gets the ApiKey and ApiSecret from Configuration (appsetting.json)
            string apiKey = _amadeusClientOptions.CurrentValue.ApiKey;
            string apiSecret = _amadeusClientOptions.CurrentValue.ApiSecret;

            // Throw ArgumentNullException if apiKey or apiSecret is null
            if(String.IsNullOrEmpty(apiKey) || String.IsNullOrEmpty(apiSecret))
            {
                _logger.LogError("ApiKey and/or ApiSecret parameters for Amadeus are not set.");
                throw new ArgumentNullException("ApiKey and/or ApiSecret parameters for Amadeus are not set.");
            }

            // Set the client's Content-Type request header to "application/x-www-form-urlencoded"
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            // Create new dictionary object with the authorization parameters
            var paramsDict = new Dictionary<string, string>();
            paramsDict.Add("grant_type", "client_credentials");
            paramsDict.Add("client_id", apiKey);
            paramsDict.Add("client_secret", apiSecret);

            // Send a POST request to authorize the client
            HttpResponseMessage response = await client.PostAsync(_amadeusClientOptions.CurrentValue.AuthTokenUrl, new FormUrlEncodedContent(paramsDict), cancellationToken);

            // Check if the HTTP response is successful
            response.EnsureSuccessStatusCode();

            // Read the response, deserialize the JSON into AmadeusTokenResponse object and set Access_token and Expires_in fields
            try
            {
                var contentStream = await response.Content.ReadAsStreamAsync();

                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);

                JsonSerializer serializer = new JsonSerializer();

                var amadeusTokenResponse = serializer.Deserialize<AmadeusTokenResponse>(jsonReader);

                _tokenString = amadeusTokenResponse.Access_token;
                double secondsToExpire = amadeusTokenResponse.Expires_in;
                _tokenExpiration = DateTime.Now.AddSeconds(secondsToExpire);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not parse JSON response for Amadeus token.", ex);
            }

        }
    }
}

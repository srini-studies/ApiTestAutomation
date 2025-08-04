using CategoryApiTestAutomation.Constants;
using FluentAssertions;
using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace CategoryApiTestAutomation.Service
{
    internal class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<JsonNode?> GetApiResponseAsync()
        {
            // Arrange
            string endpoint = $"/{ApiConstants.ApiVersion}/{ApiConstants.ResourcePath}?{ApiConstants.QueryString}";
            _logger.LogInformation("Sending GET request to {Endpoint}", endpoint);

            // Act
            try
            {
                var response = _httpClient.GetAsync(endpoint);

                response?.Result.EnsureSuccessStatusCode();
                var statusCode = response?.Result.StatusCode;
                statusCode.Should().Be(HttpStatusCode.OK);
                _logger.LogInformation("Received successful response: {StatusCode}", statusCode);

                var content = await response!.Result.Content.ReadAsStringAsync();
                _logger.LogDebug("Response content: {Content}", content);

                var node = JsonNode.Parse(content);
                return node;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calling API");
                throw;
            }

        }
    }
}

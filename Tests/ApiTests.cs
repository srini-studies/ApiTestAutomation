using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CategoryApiTestAutomation.Constants;
using CategoryApiTestAutomation.Settings;
using CategoryApiTestAutomation.Service;

namespace CategoryApiTestAutomation.Tests
{
    public class ApiTests : IAsyncLifetime
    {
        private IHost _host = null!;
        private ApiService _apiService = null!;
        private ILogger<ApiTests> _logger = null!;

        public async Task InitializeAsync()
        {
            // generic host builder to configure test environment - load configuration appsettings, setup dependency inejction and logging
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // load appsettings.json into configuration
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureLogging(logging =>
                {                    
                    logging.AddConsole();     // Log to Console
                    logging.AddDebug();       // Log to Debug output (Visual Studio)
                })
                .ConfigureServices((context, services) =>
                {
                    // Bind ApiSettings from configuration
                    var apiSettings = new ApiSettings();
                    context.Configuration.GetSection("ApiSettings").Bind(apiSettings);
                    // Add apiSettings to dependency injection
                    services.AddSingleton(apiSettings);

                    // Register HttpClient with ApiService using IHttpClientFactory with base address
                    services.AddHttpClient<ApiService>(client =>
                    {
                        client.BaseAddress = new Uri(apiSettings.BaseUrl);
                    });                    
                })
                .Build();

            // Start the host and initialise all the services
            await _host.StartAsync();

            // Get ApiService and logger from the DI container
            _apiService = _host.Services.GetRequiredService<ApiService>();
            _logger = _host.Services.GetRequiredService<ILogger<ApiTests>>();
            _logger.LogInformation("Starting test execution...");
        }

        [Fact]
        public async Task Test_ApiResponse_ShouldHave_CarbonCreditsCategory_CanRelist_And_GalleryPromotionWithCorrectDescription()
        {
            // Call Api using ApiService
            var response = await _apiService.GetApiResponseAsync();
            _logger.LogInformation("Api call done!");

            // Assertions
            using (new AssertionScope())
            {
                _logger.LogInformation("Starting Assertions...");

                // Name
                var name = response?[CategoryConstants.Key_Name]?.ToString();
                name.Should().Be(CategoryConstants.CarbonCredits);

                // CanRelist
                var canRelist = response?[CategoryConstants.Key_CanRelist]?.GetValue<bool>();
                canRelist.Should().BeTrue();
                
                // Promotions - get "Gallery" item in Promotions list
                var promotions = response?[CategoryConstants.Key_Promotions]?.AsArray();
                var galleryPromo = promotions?.FirstOrDefault(p =>
                    p?[CategoryConstants.Key_Name]?.ToString() == CategoryConstants.PromotionGallery);

                galleryPromo.Should().NotBeNull();

                // Gallery Description - get Description of "Gallery" item
                var description = galleryPromo?[CategoryConstants.Key_Description]?.ToString();
                description.Should().Contain(CategoryConstants.GalleryDescription);
            }

            _logger.LogInformation("Test Passed!");
        }

        public async Task DisposeAsync()
        {
            if (_host is not null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
        }
    }
}

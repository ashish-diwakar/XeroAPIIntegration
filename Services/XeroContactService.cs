using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text.Json;
using XeroDemo.Interfaces;
using XeroDemo.Models;

namespace XeroDemo.Services
{
    public class XeroContactService : IXeroContactService
    {
        private readonly HttpClient _httpClient;
        private readonly XeroConfigurationDto _settings;
        private readonly IXeroTokenService _tokenService;
        private readonly ILogger<XeroContactService> _logger;

        public XeroContactService(
            HttpClient httpClient,
            IOptions<XeroConfigurationDto> settings,
            IXeroTokenService tokenService,
            ILogger<XeroContactService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<List<XeroContactDto>> GetContactsAsync()
        {
            try
            {
                var accessToken =
                    await _tokenService.GetAccessTokenAsync();

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    _logger.LogError("Unable to obtain Xero access token.");
                    return [];
                }


                string url = string.IsNullOrEmpty(_settings.ContactServiceEndPoint) ? "https://api.xero.com/api.xro/2.0/Contacts" : _settings.ContactServiceEndPoint;

                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    url);

                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        accessToken);

                request.Headers.Accept.ParseAdd("application/json");

                using var response =
                    await _httpClient.SendAsync(request);

                var content =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Xero Contacts API failed. Status: {Status}. Response: {Response}",
                        response.StatusCode,
                        content);

                    return [];
                }

                _logger.LogInformation(content.ToString());

                var result =
                    JsonSerializer.Deserialize<XeroContactsResponse>(
                        content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return result?.Contacts ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error retrieving Xero contacts.");

                return [];
            }
        }
    }
}

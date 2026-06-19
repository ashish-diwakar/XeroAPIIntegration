using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using XeroDemo.Models;

namespace XeroDemo.Services
{
    public class XeroInvoiceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<XeroInvoiceService> _logger;

        public XeroInvoiceService(HttpClient httpClient, IConfiguration configuration, ILogger<XeroInvoiceService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Helper method to fetch the active OAuth2 token from Xero using Client Credentials.
        /// </summary>
        private async Task<string?> GetAccessTokenAsync()
        {
            var clientId = _configuration["XERO:ClientID"];
            var clientSecret = _configuration["XERO:ClientSecret"];

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://xero.com");

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", "accounting.transactions" }
            });

            try
            {
                using var response = await _httpClient.SendAsync(tokenRequest);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<XeroTokenResponse>(json);
                return tokenData?.AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch Xero access token.");
                return null;
            }
        }

        /// <summary>
        /// PASTE THE METHOD HERE: Fetches all active Xero organization connections/tenants.
        /// </summary>
        public async Task<List<XeroConnectionDto>> GetTenantConnectionsAsync()
        {
            var accessToken = await GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogError("Could not fetch access token to check connections.");
                return new List<XeroConnectionDto>();
            }

            // Call the dedicated Xero connections endpoint
            var request = new HttpRequestMessage(HttpMethod.Get, "https://xero.com");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                using var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed fetching Xero tenants: {Error}", error);
                    return new List<XeroConnectionDto>();
                }

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<XeroConnectionDto>>(json) ?? new List<XeroConnectionDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Xero connections.");
                return new List<XeroConnectionDto>();
            }
        }

        /// <summary>
        /// Sends the structured invoice payload over to the Xero API.
        /// </summary>
        public async Task<string?> CreateOrUpdateInvoiceAsync(InvoiceDto invoiceData, string tenantId)
        {
            var accessToken = await GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogError("Aborting invoice creation: Could not retrieve access token.");
                return null;
            }

            // Relative URL targets the BaseAddress configured in Program.cs
            var request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("Xero-tenant-id", tenantId);

            var jsonContent = JsonSerializer.Serialize(invoiceData);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                using var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Invoice created/updated successfully.");
                    return await response.Content.ReadAsStringAsync();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error creating/updating invoice: {ErrorContent}", errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while calling the Xero API.");
                return null;
            }
        }
    }
}

using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using XeroDemo.Interfaces;
using XeroDemo.Models;

namespace XeroDemo.Services
{
    public sealed class XeroTokenService : IXeroTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly XeroConfigurationDto _settings;

        private string? _cachedToken;
        private DateTime _tokenExpiryUtc;

        public XeroTokenService(
            HttpClient httpClient,
            IOptions<XeroConfigurationDto> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        /// <summary>
        /// Helper method to fetch the active OAuth2 token from Xero using Client Credentials.
        /// </summary>
        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrWhiteSpace(_cachedToken)
                && _tokenExpiryUtc > DateTime.UtcNow.AddMinutes(1))
            {
                return _cachedToken;
            }

            var credentials =
                $"{_settings.ClientId}:{_settings.ClientSecret}";

            var encodedCredentials =
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(credentials));

            string url = string.IsNullOrEmpty(_settings.TokenServiceEndPoint) ? "https://identity.xero.com/connect/token" : _settings.TokenServiceEndPoint;

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    encodedCredentials);

            request.Content =
                new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        ["grant_type"] = "client_credentials"
                    });

            var response =
                await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json =
                await response.Content.ReadAsStringAsync();

            var token =
                JsonSerializer.Deserialize<XeroAccessTokenResponse>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (token == null)
            {
                throw new InvalidOperationException(
                    "Unable to deserialize Xero token response.");
            }

            _cachedToken = token.AccessToken;

            _tokenExpiryUtc =
                DateTime.UtcNow.AddSeconds(token.ExpiresIn);

            return _cachedToken;
        }
    }
}

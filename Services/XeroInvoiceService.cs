using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using System.Text.Json;
using XeroDemo.Interfaces;
using XeroDemo.Models;

namespace XeroDemo.Services
{
    public class XeroInvoiceService : IXeroInvoiceService
    {
        private readonly HttpClient _httpClient;
        private readonly XeroConfigurationDto _settings;
        private readonly IXeroTokenService _tokenService;
        private readonly ILogger<XeroInvoiceService> _logger;

        public XeroInvoiceService(
         HttpClient httpClient,
         IOptions<XeroConfigurationDto> settings,
         IXeroTokenService tokenService,
         ILogger<XeroInvoiceService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Creates or updates an invoice in Xero.
        /// Custom Connections do not require a tenant id.
        /// </summary>
        public async Task<string?> CreateOrUpdateInvoiceAsync(
            InvoiceDto invoiceData)
        {
            try
            {
                var accessToken =
                    await _tokenService.GetAccessTokenAsync();

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    _logger.LogError(
                        "Could not obtain Xero access token.");

                    return null;
                }


                string url = string.IsNullOrEmpty(_settings.InvoiceServiceEndPoint) ? "https://api.xero.com/api.xro/2.0/Invoices" : _settings.InvoiceServiceEndPoint;

                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    url);

                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        accessToken);

                request.Headers.Accept.ParseAdd(
                    "application/json");

                request.Content =
                    new StringContent(
                        JsonSerializer.Serialize(invoiceData),
                        Encoding.UTF8,
                        "application/json");

                using var response =
                    await _httpClient.SendAsync(request);

                var content =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Xero invoice request failed. Status: {StatusCode}. Response: {Response}",
                        response.StatusCode,
                        content);

                    return null;
                }

                _logger.LogInformation(
                    "Invoice processed successfully.");

                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while sending invoice to Xero.");

                return null;
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using XeroDemo.Models;
using XeroDemo.Services;

namespace XeroDemo.Pages.Xero
{
    public class AddInvoiceModel : PageModel
    {
        private readonly XeroInvoiceService _xeroService;

        public AddInvoiceModel(XeroInvoiceService xeroService)
        {
            _xeroService = xeroService;
        }

        // Stores the currently chosen Tenant ID from the dropdown menu
        [BindProperty]
        [Required(ErrorMessage = "Please select a Xero organization from the list.")]
        [Display(Name = "Target Organization")]
        public string SelectedTenantId { get; set; } = string.Empty;

        // Binds the active dropdown select choices to render on screen
        public List<SelectListItem> XeroTenantsList { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Contact ID is mandatory.")]
        [RegularExpression(@"^[a-fA-F0-9-]{36}$", ErrorMessage = "Contact ID must be a valid 36-character Xero Guid.")]
        [Display(Name = "Xero Contact ID")]
        public string ContactId { get; set; } = string.Empty;

        [BindProperty]
        public string? ApiResult { get; set; }

        public void OnGet()
        {
            // Initial view state shows the fetch button
        }

        // HANDLER 1: Fetches all connected organizations and populates the dropdown
        public async Task<IActionResult> OnPostFetchTenantAsync()
        {
            var connections = await _xeroService.GetTenantConnectionsAsync();

            if (connections != null && connections.Count > 0)
            {
                XeroTenantsList = connections.ConvertAll(t => new SelectListItem
                {
                    Value = t.TenantId,
                    Text = t.TenantName
                });

                ApiResult = $"Found {connections.Count} connected organization(s). Please choose one from the list below to build your invoice.";
            }
            else
            {
                ApiResult = "Failed to fetch any connected Xero profiles. Check your developer credentials or API status.";
            }

            return Page();
        }

        // HANDLER 2: Processes invoice data submission using the selected dropdown option
        public async Task<IActionResult> OnPostCreateInvoiceAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate the list if validation fails, otherwise the dropdown breaks on reload
                await RePopulateDropdownAsync();
                return Page();
            }

            var sampleInvoice = new InvoiceDto
            {
                Type = "ACCREC",
                Contact = new ContactDto { ContactId = this.ContactId },
                LineItems = new()
                {
                    new LineItemDto
                    {
                        Description = "Consulting services",
                        Quantity = 10,
                        UnitAmount = 100,
                        AccountCode = "200"
                    }
                },
                Date = "2023-10-01",
                DueDate = "2023-10-15",
                Status = "AUTHORISED"
            };

            var result = await _xeroService.CreateOrUpdateInvoiceAsync(sampleInvoice, SelectedTenantId);
            ApiResult = result ?? "Failed to create invoice. Check server logs.";

            // Maintain the list state for successive invoice attempts
            await RePopulateDropdownAsync();
            return Page();
        }

        // Helper to reconstruct dropdown items on form errors or re-posts
        private async Task RePopulateDropdownAsync()
        {
            var connections = await _xeroService.GetTenantConnectionsAsync();
            if (connections != null)
            {
                XeroTenantsList = connections.ConvertAll(t => new SelectListItem
                {
                    Value = t.TenantId,
                    Text = t.TenantName,
                    Selected = (t.TenantId == SelectedTenantId)
                });
            }
        }
    }
}

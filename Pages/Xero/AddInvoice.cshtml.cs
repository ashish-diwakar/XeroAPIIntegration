using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using XeroDemo.Interfaces;
using XeroDemo.Models;
using XeroDemo.Services;

namespace XeroDemo.Pages.Xero
{
    public class AddInvoiceModel : PageModel
    {
        private readonly IXeroInvoiceService _xeroInvoiceService;
        private readonly IXeroContactService _xeroContactService;

        public AddInvoiceModel(
            IXeroInvoiceService xeroInvoiceService,
            IXeroContactService xeroContactService)
        {
            _xeroInvoiceService = xeroInvoiceService;
            _xeroContactService = xeroContactService;
        }

        // Stores the currently chosen Contact ID from the dropdown menu
        [BindProperty]
        [Required(ErrorMessage = "Please select a Xero organization from the list.")]
        [Display(Name = "Target Organization")]
        public string SelectedContactId { get; set; } = string.Empty;

        // Binds the active dropdown select choices to render on screen
        public List<SelectListItem> XeroContactsList { get; set; } = new();

        //[BindProperty]
        //[Required(ErrorMessage = "Contact ID is mandatory.")]
        //[RegularExpression(@"^[a-fA-F0-9-]{36}$", ErrorMessage = "Contact ID must be a valid 36-character Xero Guid.")]
        //[Display(Name = "Xero Contact ID")]
        //public string ContactId { get; set; } = string.Empty;

        [BindProperty]
        public string? ApiResult { get; set; }

        public void OnGet()
        {
            // Initial view state shows the fetch button
        }

        // HANDLER 1: Fetches all connected organizations and populates the dropdown
        public async Task<IActionResult> OnPostFetchContactAsync()
        {
            var connections = await _xeroContactService.GetContactsAsync();

            if (connections != null && connections.Count > 0)
            {
                XeroContactsList = connections.ConvertAll(t => new SelectListItem
                {
                    Value = t.ContactId.ToString(),
                    Text = t.Name
                });

                ApiResult = $"Found {connections.Count} connected contact(s). Please choose one from the list below to build your invoice.";
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

            Guid selectedContactIDGUID = Guid.Parse(SelectedContactId);

            var sampleInvoice = new InvoiceDto
            {
                Type = "ACCREC",
                Contact = new XeroContactDto { ContactId = selectedContactIDGUID },                
                LineItems = new()
                {
                    new LineItemDto
                    {
                        Description = "Demo Consulting services",
                        Quantity = 10,
                        UnitAmount = 100,
                        AccountCode = "200"
                    }
                },
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                DueDate = DateTime.Now.AddDays(15).ToString("yyyy-MM-dd"), //"2023-10-15",
                Status = "AUTHORISED"
            };


            var result = await _xeroInvoiceService.CreateOrUpdateInvoiceAsync(sampleInvoice);
            ApiResult = result ?? "Failed to create invoice. Check server logs.";

            // Maintain the list state for successive invoice attempts
            await RePopulateDropdownAsync();
            return Page();
        }

        // Helper to reconstruct dropdown items on form errors or re-posts
        private async Task RePopulateDropdownAsync()
        {
            var connections = await _xeroContactService.GetContactsAsync();
            if (connections != null)
            {
                XeroContactsList = connections.ConvertAll(t => new SelectListItem
                {
                    Value = t.ContactId.ToString(),
                    Text = t.Name,
                    Selected = (t.ContactId.ToString() == SelectedContactId)
                });
            }
        }
    }
}

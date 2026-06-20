using XeroDemo.Models;

namespace XeroDemo.Interfaces
{
    public interface IXeroInvoiceService
    {
        Task<string?> CreateOrUpdateInvoiceAsync(InvoiceDto invoiceData);
    }
}

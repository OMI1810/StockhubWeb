// Services/InvoiceService/IInvoiceService.cs
using StockhubWeb.Models;

namespace StockhubWeb.Services.InvoiceService
{
    public interface IInvoiceService
    {
        Task<string> GenerateInvoiceAsync(Warehouse warehouse);
        Task<byte[]> GenerateInvoicePdfAsync(Warehouse warehouse);
        Task<string> GetInvoiceContentAsync(Warehouse warehouse);
    }
}
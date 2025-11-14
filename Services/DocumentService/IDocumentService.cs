using StockhubWeb.Models;

namespace StockhubWeb.Services.DocumentService
{
    public interface IDocumentService
    {
        Task<byte[]> GenerateWarehouseStockPdfAsync(Warehouse warehouse);
        Task<byte[]> GenerateInvoicePdfAsync(InvoiceData invoiceData);
        Task<string> GenerateInvoiceNumberAsync(string organizationId);
    }
}
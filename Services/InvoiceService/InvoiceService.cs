// Services/InvoiceService/InvoiceService.cs
using System.Text;
using StockhubWeb.Models;

namespace StockhubWeb.Services.InvoiceService
{
    public class InvoiceService : IInvoiceService
    {
        private readonly string _invoiceTemplatePath = "Templates/InvoiceTemplate.txt";
        private readonly string _productsTableTemplatePath = "Templates/ProductsTableTemplate.txt";
        private string? _cachedInvoiceTemplate;
        private string? _cachedProductsTableTemplate;

        public async Task<string> GenerateInvoiceAsync(Warehouse warehouse)
        {
            // Загружаем шаблоны из файлов
            var invoiceTemplate = await LoadInvoiceTemplateFromFile();
            var productsTableTemplate = await LoadProductsTableTemplateFromFile();

            // Генерируем таблицу товаров
            var productsTable = await GenerateProductsTable(warehouse.Products, productsTableTemplate);

            // Заменяем плейсхолдеры в шаблоне накладной
            return ReplaceInvoiceTemplatePlaceholders(invoiceTemplate, warehouse, productsTable);
        }

        private async Task<string> LoadInvoiceTemplateFromFile()
        {
            if (!string.IsNullOrEmpty(_cachedInvoiceTemplate))
                return _cachedInvoiceTemplate;

            try
            {
                if (File.Exists(_invoiceTemplatePath))
                {
                    _cachedInvoiceTemplate = await File.ReadAllTextAsync(_invoiceTemplatePath, Encoding.UTF8);
                    return _cachedInvoiceTemplate;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки шаблона накладной: {ex.Message}");
            }

            return GetError();
        }

        private async Task<string> LoadProductsTableTemplateFromFile()
        {
            if (!string.IsNullOrEmpty(_cachedProductsTableTemplate))
                return _cachedProductsTableTemplate;

            try
            {
                if (File.Exists(_productsTableTemplatePath))
                {
                    _cachedProductsTableTemplate = await File.ReadAllTextAsync(_productsTableTemplatePath, Encoding.UTF8);
                    return _cachedProductsTableTemplate;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки шаблона таблицы товаров: {ex.Message}");
            }

            return GetError();
        }

        private async Task<string> GenerateProductsTable(List<Product> products, string tableTemplate)
        {
            if (!products.Any())
                return "           На складе нет товаров";

            var productRows = new StringBuilder();
            foreach (var product in products)
            {
                var name = product.Name.Length > 28 ? product.Name.Substring(0, 28) + "..." : product.Name.PadRight(28);
                var quantity = product.Quantity.ToString().PadLeft(8);
                var price = product.Price.ToString("C").PadLeft(10);

                productRows.AppendLine($"│ {name} │ {quantity} │ {price} │");
            }

            return tableTemplate.Replace("{{PRODUCT_ROWS}}", productRows.ToString());
        }

        private string ReplaceInvoiceTemplatePlaceholders(string template, Warehouse warehouse, string productsTable)
        {
            var totalQuantity = warehouse.Products.Sum(p => p.Quantity);
            var totalValue = warehouse.Products.Sum(p => p.Quantity * p.Price);

            var totalStats = warehouse.Products.Any()
                ? $"\nОбщее количество товаров: {totalQuantity} шт.\nОбщая стоимость: {totalValue:C}"
                : string.Empty;

            var result = template
                .Replace("{{WAREHOUSE_NAME}}", warehouse.Name)
                .Replace("{{WAREHOUSE_ID}}", warehouse.Id)
                .Replace("{{WAREHOUSE_STATUS}}", warehouse.IsActive ? "Активен" : "Неактивен")
                .Replace("{{CREATION_DATE}}", warehouse.CreatedAt.ToString("dd.MM.yyyy HH:mm"))
                .Replace("{{CURRENT_DATE}}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                .Replace("{{PRODUCTS_COUNT}}", warehouse.Products.Count.ToString())
                .Replace("{{PRODUCTS_TABLE}}", productsTable)
                .Replace("{{TOTAL_STATS}}", totalStats);

            return result;
        }

        private string GetError()
        {
            return "Ошибка!!!";
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(Warehouse warehouse)
        {
            await Task.Delay(100);
            return Array.Empty<byte>();
        }

        public async Task<string> GetInvoiceContentAsync(Warehouse warehouse)
        {
            return await GenerateInvoiceAsync(warehouse);
        }
    }
}
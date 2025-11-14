// Services/DocumentService/DocumentService.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StockhubWeb.Models;

namespace StockhubWeb.Services.DocumentService
{
    public class DocumentService : IDocumentService
    {
        public DocumentService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateWarehouseStockPdfAsync(Warehouse warehouse)
        {
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .AlignCenter()
                            .Text($"Остатки на складе: {warehouse.Name}")
                            .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Spacing(10);

                                // Информация о складе
                                column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(infoColumn =>
                                {
                                    infoColumn.Item().Text($"Адрес: {warehouse.Country}, {warehouse.Region}, {warehouse.City}, {warehouse.Address}");
                                    infoColumn.Item().Text($"Почтовый индекс: {warehouse.PostalCode}");
                                    infoColumn.Item().Text($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}");
                                });

                                // Таблица товаров
                                if (warehouse.Products.Any())
                                {
                                    column.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(1); // №
                                            columns.RelativeColumn(2); // Артикул
                                            columns.RelativeColumn(4); // Наименование
                                            columns.RelativeColumn(2); // Количество
                                            columns.RelativeColumn(2); // Цена
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("№");
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Артикул");
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Наименование");
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Количество");
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Цена, руб");

                                            header.Cell().ColumnSpan(5).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);
                                        });

                                        for (int i = 0; i < warehouse.Products.Count; i++)
                                        {
                                            var product = warehouse.Products[i];
                                            var rowNumber = i + 1;

                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(rowNumber.ToString());
                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(product.Id[..8].ToUpper());
                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(product.Name);
                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(product.Quantity.ToString());
                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(product.Price.ToString("C"));
                                        }
                                    });

                                    // Итоги
                                    column.Item().AlignRight().Text($"Всего товаров: {warehouse.Products.Count}")
                                        .SemiBold();
                                    column.Item().AlignRight().Text($"Общее количество: {warehouse.Products.Sum(p => p.Quantity)}")
                                        .SemiBold();
                                    column.Item().AlignRight().Text($"Общая стоимость: {warehouse.Products.Sum(p => p.Price * p.Quantity):C}")
                                        .SemiBold();
                                }
                                else
                                {
                                    column.Item().AlignCenter().Text("На складе нет товаров").Italic();
                                }
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Страница ");
                                x.CurrentPageNumber();
                                x.Span(" из ");
                                x.TotalPages();
                            });
                    });
                });

                return document.GeneratePdf();
            });
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(InvoiceData invoiceData)
        {
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .AlignCenter()
                            .Text($"Накладная №{invoiceData.InvoiceNumber}")
                            .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Spacing(15);

                                // Информация о накладной
                                column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(infoColumn =>
                                {
                                    infoColumn.Item().Text($"Склад отправитель: {invoiceData.Warehouse.Name}");
                                    infoColumn.Item().Text($"Адрес: {invoiceData.Warehouse.Country}, {invoiceData.Warehouse.Region}, {invoiceData.Warehouse.City}, {invoiceData.Warehouse.Address}");
                                    infoColumn.Item().Text($"Дата формирования: {invoiceData.GenerationDate:dd.MM.yyyy HH:mm}");
                                });

                                // Таблица товаров
                                if (invoiceData.Products.Any())
                                {
                                    column.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(1); // №
                                            columns.RelativeColumn(2); // Артикул
                                            columns.RelativeColumn(3); // Наименование
                                            columns.RelativeColumn(2); // Количество
                                            columns.RelativeColumn(2); // Цена
                                            columns.RelativeColumn(2); // Сумма
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("№");
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Артикул");
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Наименование");
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Количество");
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Цена, руб");
                                            header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Сумма, руб");

                                            header.Cell().ColumnSpan(6).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);
                                        });

                                        for (int i = 0; i < invoiceData.Products.Count; i++)
                                        {
                                            var product = invoiceData.Products[i];
                                            var rowNumber = i + 1;
                                            var total = product.Quantity * product.Price;

                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(rowNumber.ToString());
                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(product.Id[..8].ToUpper());
                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(product.Name);
                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(product.Quantity.ToString());
                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(product.Price.ToString("C"));
                                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(total.ToString("C"));
                                        }
                                    });

                                    // Итоги
                                    column.Item().AlignRight().Text($"Общее количество: {invoiceData.TotalQuantity}")
                                        .SemiBold();
                                    column.Item().AlignRight().Text($"Общая стоимость: {invoiceData.TotalValue:C}")
                                        .SemiBold();
                                }
                                else
                                {
                                    column.Item().AlignCenter().Text("Нет товаров для отображения").Italic();
                                }
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Страница ");
                                x.CurrentPageNumber();
                                x.Span(" из ");
                                x.TotalPages();
                            });
                    });
                });

                return document.GeneratePdf();
            });
        }

        public Task<string> GenerateInvoiceNumberAsync(string organizationId)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var orgPrefix = organizationId[..4].ToUpper();
            return Task.FromResult($"INV-{orgPrefix}-{timestamp}");
        }
    }
}
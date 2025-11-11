// Services/InvoiceService/InvoiceService.cs
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StockhubWeb.Models;
using System.Text;

namespace StockhubWeb.Services.InvoiceService
{
    public class InvoiceService : IInvoiceService
    {
        private readonly string _invoiceTemplatePath = "Templates/InvoiceTemplate.docx";
        private readonly string _productsTableTemplatePath = "Templates/ProductsTableTemplate.docx";

        public InvoiceService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(Warehouse warehouse)
        {
            try
            {
                // Загружаем шаблоны с обработкой ошибок
                var invoiceTemplate = await LoadUniversalWordDocument(_invoiceTemplatePath)
                    ?? await CreateFallbackTemplate("ОСНОВНОЙ ШАБЛОН НАКЛАДНОЙ");

                var productsTableTemplate = await LoadUniversalWordDocument(_productsTableTemplatePath)
                    ?? await CreateFallbackTemplate("ШАБЛОН ТАБЛИЦЫ ТОВАРОВ");

                // Генерируем контент
                var productsTable = GenerateProductsTable(warehouse.Products, productsTableTemplate);
                var finalContent = ReplaceTemplatePlaceholders(invoiceTemplate, warehouse, productsTable);

                return GenerateUniversalPdf(finalContent);
            }
            catch (Exception ex)
            {
                return await GenerateErrorPdfAsync($"Критическая ошибка: {ex.Message}");
            }
        }

        private async Task<WordDocumentContent> LoadUniversalWordDocument(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл не найден: {filePath}");
                return null;
            }

            try
            {
                using var wordDocument = WordprocessingDocument.Open(filePath, false);
                var body = wordDocument.MainDocumentPart?.Document.Body;

                if (body == null)
                {
                    Console.WriteLine($"Пустой документ: {filePath}");
                    return null;
                }

                var content = new WordDocumentContent();

                foreach (var paragraph in body.Elements<Paragraph>())
                {
                    var paragraphContent = new ParagraphContent();

                    // Обрабатываем каждый Run в параграфе
                    foreach (var run in paragraph.Elements<Run>())
                    {
                        var runContent = new RunContent();

                        // Получаем свойства форматирования
                        var runProperties = run.RunProperties;
                        if (runProperties != null)
                        {
                            runContent.IsBold = runProperties.Bold != null;
                            runContent.IsItalic = runProperties.Italic != null;
                            runContent.FontSize = GetFontSize(runProperties);
                        }

                        // Получаем текст
                        foreach (var text in run.Elements<Text>())
                        {
                            runContent.Text += text.Text;
                        }

                        paragraphContent.Runs.Add(runContent);
                    }

                    // Получаем выравнивание параграфа
                    var paragraphProperties = paragraph.ParagraphProperties;
                    if (paragraphProperties?.Justification != null)
                    {
                        paragraphContent.Alignment = paragraphProperties.Justification.Val.Value.ToString();
                    }

                    content.Paragraphs.Add(paragraphContent);
                }

                if (!content.Paragraphs.Any())
                {
                    Console.WriteLine($"Документ не содержит контента: {filePath}");
                    return null;
                }

                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения Word документа {filePath}: {ex.Message}");
                return null;
            }
        }

        private string GetFontSize(RunProperties runProperties)
        {
            if (runProperties?.FontSize?.Val?.Value != null)
            {
                var fontSize = Convert.ToInt32(runProperties.FontSize.Val.Value) / 2; // Конвертируем в пункты
                return fontSize.ToString();
            }
            return "12"; // Размер по умолчанию
        }

        private async Task<WordDocumentContent> CreateFallbackTemplate(string templateName)
        {
            var content = new WordDocumentContent();

            // Создаем простой шаблон с сообщением об ошибке
            var errorParagraph = new ParagraphContent
            {
                Alignment = "Center"
            };
            errorParagraph.Runs.Add(new RunContent
            {
                Text = $"═══════════════════════════════════════",
                FontSize = "12"
            });

            content.Paragraphs.Add(errorParagraph);

            var messageParagraph = new ParagraphContent
            {
                Alignment = "Center"
            };
            messageParagraph.Runs.Add(new RunContent
            {
                Text = $"ОШИБКА: {templateName}",
                FontSize = "14",
                IsBold = true
            });

            content.Paragraphs.Add(messageParagraph);

            var descParagraph = new ParagraphContent
            {
                Alignment = "Center"
            };
            descParagraph.Runs.Add(new RunContent
            {
                Text = $"Не удалось загрузить шаблон",
                FontSize = "12"
            });

            content.Paragraphs.Add(descParagraph);

            return content;
        }

        private string GenerateProductsTable(List<Product> products, WordDocumentContent tableTemplate)
        {
            if (!products.Any())
                return "           На складе нет товаров";

            // Преобразуем шаблон таблицы в текст
            var templateText = ConvertToText(tableTemplate);
            var templateLines = templateText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (templateLines.Length < 3)
                return "           На складе нет товаров";

            var result = new StringBuilder();

            // Используем первую строку как верхнюю границу
            if (templateLines.Length > 0)
                result.AppendLine(templateLines[0].Trim());

            // Вторая строка - заголовок
            if (templateLines.Length > 1)
                result.AppendLine(templateLines[1].Trim());

            // Третья строка - разделитель
            if (templateLines.Length > 2)
                result.AppendLine(templateLines[2].Trim());

            // Генерируем строки товаров
            foreach (var product in products)
            {
                var rowTemplate = templateLines.Length > 3 ? templateLines[3].Trim() : "│ {0,-30} │ {1,8} │ {2,10} │";

                var name = product.Name.Length > 28 ? product.Name.Substring(0, 28) + "..." : product.Name;
                var quantity = product.Quantity.ToString();
                var price = product.Price.ToString("C");

                var productRow = string.Format(ConvertTemplateToFormat(rowTemplate), name, quantity, price);
                result.AppendLine(productRow);
            }

            // Нижняя граница
            if (templateLines.Length > 4)
                result.AppendLine(templateLines[4].Trim());

            return result.ToString();
        }

        private string ConvertTemplateToFormat(string template)
        {
            return template
                .Replace("{{PRODUCT_NAME}}", "{0}")
                .Replace("{{PRODUCT_QUANTITY}}", "{1}")
                .Replace("{{PRODUCT_PRICE}}", "{2}");
        }

        private string ReplaceTemplatePlaceholders(WordDocumentContent template, Warehouse warehouse, string productsTable)
        {
            var result = new WordDocumentContent();

            foreach (var paragraph in template.Paragraphs)
            {
                var newParagraph = new ParagraphContent
                {
                    Alignment = paragraph.Alignment
                };

                foreach (var run in paragraph.Runs)
                {
                    var newRun = new RunContent
                    {
                        Text = run.Text
                            .Replace("{{WAREHOUSE_NAME}}", warehouse.Name)
                            .Replace("{{WAREHOUSE_ID}}", warehouse.Id)
                            .Replace("{{WAREHOUSE_STATUS}}", warehouse.IsActive ? "Активен" : "Неактивен")
                            .Replace("{{CREATION_DATE}}", warehouse.CreatedAt.ToString("dd.MM.yyyy HH:mm"))
                            .Replace("{{CURRENT_DATE}}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                            .Replace("{{PRODUCTS_TABLE}}", productsTable)
                            .Replace("{{TOTAL_STATS}}", GetTotalStats(warehouse))
                            .Replace("Херня", "ТОВАРЫ НА СКЛАДЕ"),
                        IsBold = run.IsBold,
                        IsItalic = run.IsItalic,
                        FontSize = run.FontSize
                    };

                    newParagraph.Runs.Add(newRun);
                }

                result.Paragraphs.Add(newParagraph);
            }

            return ConvertToText(result);
        }

        private string GetTotalStats(Warehouse warehouse)
        {
            if (!warehouse.Products.Any())
                return string.Empty;

            var totalQuantity = warehouse.Products.Sum(p => p.Quantity);
            var totalValue = warehouse.Products.Sum(p => p.Quantity * p.Price);

            return $"\nОбщее количество товаров: {totalQuantity} шт.\nОбщая стоимость: {totalValue:C}";
        }

        private string ConvertToText(WordDocumentContent content)
        {
            var sb = new StringBuilder();

            foreach (var paragraph in content.Paragraphs)
            {
                foreach (var run in paragraph.Runs)
                {
                    sb.Append(run.Text);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private byte[] GenerateUniversalPdf(string content)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Times New Roman"));

                    page.Content()
                        .PaddingVertical(0.5f, Unit.Centimetre)
                        .Column(column =>
                        {
                            var lines = content.Split('\n');
                            foreach (var line in lines)
                            {
                                if (string.IsNullOrWhiteSpace(line))
                                {
                                    column.Item().Height(10);
                                }
                                else
                                {
                                    // Универсальная обработка строк
                                    var formattedLine = line.Trim();

                                    if (formattedLine.Contains("═══════════════════════════════════════"))
                                    {
                                        column.Item()
                                            .PaddingVertical(5)
                                            .AlignCenter()
                                            .Text(formattedLine)
                                            .FontSize(10);
                                    }
                                    else if (formattedLine.StartsWith("┌") || formattedLine.Contains("─") ||
                                             formattedLine.Contains("┐") || formattedLine.Contains("├") ||
                                             formattedLine.Contains("┼") || formattedLine.Contains("┤") ||
                                             formattedLine.StartsWith("└") || formattedLine.Contains("┴") ||
                                             formattedLine.Contains("┘") || formattedLine.Contains("│"))
                                    {
                                        // Табличные символы - используем моноширинный шрифт
                                        column.Item()
                                            .PaddingVertical(0)
                                            .Text(formattedLine)
                                            .FontFamily("Courier New")
                                            .FontSize(8)
                                            .LineHeight(0.8f);
                                    }
                                    else if (formattedLine.StartsWith("НАКЛАДНАЯ СКЛАДА") ||
                                             formattedLine.StartsWith("ТОВАРЫ НА СКЛАДЕ"))
                                    {
                                        column.Item()
                                            .PaddingVertical(10)
                                            .AlignCenter()
                                            .Text(formattedLine)
                                            .FontSize(14)
                                            .Bold();
                                    }
                                    else if (formattedLine.StartsWith("ОШИБКА:"))
                                    {
                                        column.Item()
                                            .PaddingVertical(10)
                                            .Background(Colors.Red.Lighten5)
                                            .Padding(10)
                                            .AlignCenter()
                                            .Text(formattedLine)
                                            .FontSize(12)
                                            .Bold()
                                            .FontColor(Colors.Red.Darken2);
                                    }
                                    else
                                    {
                                        // Обычный текст
                                        column.Item()
                                            .PaddingVertical(2)
                                            .Text(formattedLine)
                                            .FontSize(10);
                                    }
                                }
                            }
                        });
                });
            });

            return document.GeneratePdf();
        }

        private async Task<byte[]> GenerateErrorPdfAsync(string errorMessage)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    page.Content()
                        .PaddingVertical(2, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item()
                                .Background(Colors.Red.Lighten5)
                                .Border(1)
                                .BorderColor(Colors.Red.Darken2)
                                .Padding(20)
                                .AlignCenter()
                                .Text("ОШИБКА ГЕНЕРАЦИИ НАКЛАДНОЙ")
                                .FontSize(16)
                                .Bold()
                                .FontColor(Colors.Red.Darken2);

                            column.Item()
                                .Padding(10)
                                .Text(errorMessage)
                                .FontSize(11);

                            column.Item()
                                .Padding(10)
                                .Background(Colors.Grey.Lighten3)
                                .Padding(15)
                                .Text("Перезапустите приложение после изменения шаблонов")
                                .FontSize(10)
                                .Italic();
                        });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<string> GenerateInvoiceAsync(Warehouse warehouse)
        {
            var pdfBytes = await GenerateInvoicePdfAsync(warehouse);
            return "PDF накладная сгенерирована";
        }

        public async Task<string> GetInvoiceContentAsync(Warehouse warehouse)
        {
            return await GenerateInvoiceAsync(warehouse);
        }
    }

    // Классы для хранения структуры Word документа
    public class WordDocumentContent
    {
        public List<ParagraphContent> Paragraphs { get; set; } = new();
    }

    public class ParagraphContent
    {
        public string Alignment { get; set; } = "Left";
        public List<RunContent> Runs { get; set; } = new();
    }

    public class RunContent
    {
        public string Text { get; set; } = string.Empty;
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public string FontSize { get; set; } = "12";
    }
}
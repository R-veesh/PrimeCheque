using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class PdfGenerationService : IPdfGenerationService
    {
        static PdfGenerationService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public Task<string> GenerateChequePdfAsync(Cheque cheque, BankTemplate template, PrinterCalibration? calibration = null, string? watermarkText = null)
        {
            float widthMm = (float)template.ChequeWidthMm;
            float heightMm = (float)template.ChequeHeightMm;

            float hOffset = calibration != null ? (float)calibration.HorizontalOffsetMm : 0f;
            float vOffset = calibration != null ? (float)calibration.VerticalOffsetMm : 0f;

            TemplateConfigDto config = new TemplateConfigDto();
            try
            {
                if (!string.IsNullOrWhiteSpace(template.TemplateConfig))
                {
                    config = JsonSerializer.Deserialize<TemplateConfigDto>(template.TemplateConfig) ?? new TemplateConfigDto();
                }
            }
            catch
            {
                // Fallback to empty config if invalid JSON
            }

            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrimeCheque", "GeneratedPdfs");
            Directory.CreateDirectory(folder);
            var filePath = Path.Combine(folder, $"Cheque_{cheque.ChequeNumber}_{Guid.NewGuid():N}.pdf");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(new PageSize(widthMm, heightMm, Unit.Millimetre));
                    page.Margin(0, Unit.Millimetre);
                    page.PageColor(Colors.White);

                    page.Content().Layers(layers =>
                    {
                        void AddField(FieldConfig? cfg, string text, bool bold = false)
                        {
                            if (cfg == null || string.IsNullOrWhiteSpace(text)) return;

                            float posX = cfg.x + hOffset;
                            float posY = cfg.y + vOffset;
                            float fontSz = cfg.fontSize > 0 ? cfg.fontSize : 11f;
                            float fieldW = cfg.width > 0 ? cfg.width : 150f;
                            float fieldH = cfg.height > 0 ? cfg.height : 10f;

                            layers.Layer()
                                .OffsetX(posX, Unit.Millimetre)
                                .OffsetY(posY, Unit.Millimetre)
                                .Width(fieldW, Unit.Millimetre)
                                .Height(fieldH, Unit.Millimetre)
                                .Rotate(cfg.angle)
                                .Text(txt =>
                                {
                                    var span = txt.Span(text).FontSize(fontSz).FontColor(Colors.Black);
                                    if (bold || cfg.fontWeight?.Equals("Bold", StringComparison.OrdinalIgnoreCase) == true) span.Bold();
                                });
                        }

                        // Date parts
                        string dayStr = cheque.ChequeDate.ToString("dd");
                        string monthStr = cheque.ChequeDate.ToString("MM");
                        string yearStr = cheque.ChequeDate.ToString("yyyy");

                        AddField(config.dateDay, dayStr, true);
                        AddField(config.dateMonth, monthStr, true);
                        AddField(config.dateYear, yearStr, true);

                        // Payee Name (with line 2 support if long)
                        string payeeStr = $"**{cheque.PayeeName}**";
                        if (payeeStr.Length > 45 && config.payeeLine2 != null)
                        {
                            int splitIdx = cheque.PayeeName.LastIndexOf(' ', 40);
                            if (splitIdx <= 0) splitIdx = 40;

                            string line1 = $"**{cheque.PayeeName.Substring(0, splitIdx)}";
                            string line2 = $"{cheque.PayeeName.Substring(splitIdx).Trim()}**";

                            AddField(config.payeeLine1, line1, true);
                            AddField(config.payeeLine2, line2, true);
                        }
                        else
                        {
                            AddField(config.payeeLine1, payeeStr, true);
                        }

                        // Amount Words
                        string words = cheque.AmountInWords;
                        if (words.Length > 50 && config.amountWordsLine2 != null)
                        {
                            int splitIdx = words.LastIndexOf(' ', 50);
                            if (splitIdx <= 0) splitIdx = 50;

                            string line1 = words.Substring(0, splitIdx);
                            string line2 = words.Substring(splitIdx).Trim();

                            AddField(config.amountWordsLine1, line1);
                            AddField(config.amountWordsLine2, line2);
                        }
                        else
                        {
                            AddField(config.amountWordsLine1, words);
                        }

                        // Amount Figures
                        AddField(config.amountFigures, $"**{cheque.Amount:N2}**", true);

                        // Memo
                        if (!string.IsNullOrEmpty(cheque.Memo))
                        {
                            AddField(config.memoLine, cheque.Memo);
                        }

                        // Crossing
                        if (cheque.CrossingType != CrossingType.None && config.crossingZone != null)
                        {
                            string crossingText = cheque.CrossingType switch
                            {
                                CrossingType.AccountPayeeOnly => "// A/C PAYEE ONLY //",
                                CrossingType.NotNegotiable => "// NOT NEGOTIABLE //",
                                CrossingType.AccountPayeeAndNotNegotiable => "// A/C PAYEE ONLY - NOT NEGOTIABLE //",
                                _ => ""
                            };
                            AddField(config.crossingZone, crossingText, true);
                        }

                        // Watermark if requested
                        if (!string.IsNullOrEmpty(watermarkText))
                        {
                            layers.Layer()
                                .OffsetX(widthMm / 4, Unit.Millimetre)
                                .OffsetY(heightMm / 3, Unit.Millimetre)
                                .Text(txt =>
                                {
                                    txt.Span(watermarkText).FontSize(24).FontColor(Colors.Grey.Lighten2).Bold();
                                });
                        }
                    });
                });
            });

            document.GeneratePdf(filePath);
            return Task.FromResult(filePath);
        }
    }
}

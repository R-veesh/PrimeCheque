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
            float widthMm = Math.Max(1f, (float)template.ChequeWidthMm);
            float heightMm = Math.Max(1f, (float)template.ChequeHeightMm);

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

            // Use WinRT LocalFolder to bypass MSIX virtualization issues for native libraries (QuestPDF/SkiaSharp)
            var folder = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "GeneratedPdfs");
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
                        // A primary layer is strictly required by QuestPDF for the Layers component.
                        layers.PrimaryLayer();

                        void AddField(FieldConfig? cfg, string text, bool bold = false)
                        {
                            if (cfg == null || string.IsNullOrWhiteSpace(text)) return;

                            float posX = Math.Max(0, cfg.x + hOffset);
                            float posY = Math.Max(0, cfg.y + vOffset);
                            float fontSz = cfg.fontSize > 0 ? cfg.fontSize : 11f;
                            float fieldW = cfg.width > 0 ? cfg.width : 150f;
                            float fieldH = cfg.height > 0 ? cfg.height : 10f;

                            layers.Layer()
                                .Unconstrained()
                                .OffsetX(posX, Unit.Millimetre)
                                .OffsetY(posY, Unit.Millimetre)
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

                        AddField(config.dateD1, dayStr[0].ToString(), true);
                        AddField(config.dateD2, dayStr[1].ToString(), true);
                        AddField(config.dateM1, monthStr[0].ToString(), true);
                        AddField(config.dateM2, monthStr[1].ToString(), true);
                        AddField(config.dateY1, yearStr[0].ToString(), true);
                        AddField(config.dateY2, yearStr[1].ToString(), true);
                        AddField(config.dateY3, yearStr[2].ToString(), true);
                        AddField(config.dateY4, yearStr[3].ToString(), true);

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
                                CrossingType.AccountPayeeOnly => "A/C PAYEE ONLY",
                                CrossingType.AccountPayeeAndNotNegotiable => "A/C PAYEE ONLY - NOT NEGOTIABLE",
                                CrossingType.CrossAccountPayeeAndOrBearer => "A/C PAYEE ONLY",
                                CrossingType.CrossAccountPayeeAndNotNegotiableAndOrBearer => "A/C PAYEE ONLY - NOT NEGOTIABLE",
                                _ => ""
                            };

                            var cfg = config.crossingZone;
                            float posX = Math.Max(0, cfg.x + hOffset);
                            float posY = Math.Max(0, cfg.y + vOffset);
                            float fontSz = cfg.fontSize > 0 ? cfg.fontSize : 11f;
                            float fieldW = cfg.width > 0 ? cfg.width : 35f;
                            float fieldH = cfg.height > 0 ? cfg.height : 18f;

                            layers.Layer()
                                .Unconstrained()
                                .OffsetX(posX, Unit.Millimetre)
                                .OffsetY(posY, Unit.Millimetre)
                                .Rotate(cfg.angle)
                                .Width(fieldW, Unit.Millimetre)
                                .Height(fieldH, Unit.Millimetre)
                                .BorderTop(1)
                                .BorderBottom(1)
                                .AlignCenter()
                                .AlignMiddle()
                                .Text(txt =>
                                {
                                    txt.Span(crossingText).FontSize(fontSz).FontColor(Colors.Black).Bold();
                                });
                        }

                        // Or Bearer Strikeout
                        bool hasBearerStrike = cheque.CrossingType == CrossingType.CrossAccountPayeeAndOrBearer || 
                                               cheque.CrossingType == CrossingType.CrossAccountPayeeAndNotNegotiableAndOrBearer;

                        if (hasBearerStrike && config.orBearerZone != null)
                        {
                            var cfg = config.orBearerZone;
                            float posX = Math.Max(0, cfg.x + hOffset);
                            float posY = Math.Max(0, cfg.y + vOffset);
                            float fieldW = cfg.width > 0 ? cfg.width : 25f;
                            float fieldH = cfg.height > 0 ? cfg.height : 5f;

                            layers.Layer()
                                .Unconstrained()
                                .OffsetX(posX, Unit.Millimetre)
                                .OffsetY(posY + (fieldH / 2), Unit.Millimetre)
                                .Rotate(cfg.angle)
                                .Width(fieldW, Unit.Millimetre)
                                .Height(1, Unit.Millimetre) // Just need height for the border
                                .BorderTop(1);
                        }

                        // Watermark if requested
                        if (!string.IsNullOrEmpty(watermarkText))
                        {
                            layers.Layer()
                                .Unconstrained()
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

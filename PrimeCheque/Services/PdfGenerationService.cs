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

            var localAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrimeCheque");
            var folder = Path.Combine(localAppData, "GeneratedPdfs");
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

                        // Helper for splitting text
                        int GetMaxChars(FieldConfig? cfg, int defaultLimit)
                        {
                            if (cfg == null || cfg.width <= 0) return defaultLimit;
                            float fontSz = cfg.fontSize > 0 ? cfg.fontSize : 11f;
                            // A conservative estimate: Arial bold 11pt char is ~1.7mm wide
                            // We divide the field width (mm) by the estimated char width
                            float charWidthMm = fontSz * 0.155f; 
                            return (int)(cfg.width / charWidthMm);
                        }

                        string[] SplitTextIntoLines(string text, int[] lineMaxChars)
                        {
                            var lines = new System.Collections.Generic.List<string>();
                            string remaining = text;

                            for (int i = 0; i < lineMaxChars.Length; i++)
                            {
                                if (string.IsNullOrWhiteSpace(remaining)) break;

                                int maxChars = lineMaxChars[i];
                                if (remaining.Length <= maxChars)
                                {
                                    lines.Add(remaining.Trim());
                                    remaining = "";
                                    break;
                                }

                                int splitIdx = remaining.LastIndexOf(' ', maxChars);
                                if (splitIdx <= 0) splitIdx = maxChars;

                                lines.Add(remaining.Substring(0, splitIdx).Trim());
                                remaining = remaining.Substring(splitIdx).Trim();
                            }

                            if (!string.IsNullOrWhiteSpace(remaining) && lines.Count > 0)
                            {
                                lines[lines.Count - 1] += " " + remaining;
                            }
                            return lines.ToArray();
                        }

                        // Payee Name
                        string payeeStr = $"**{cheque.PayeeName}**";
                        if (config.payeeLine2 != null)
                        {
                            int maxL1 = GetMaxChars(config.payeeLine1, 80);
                            int maxL2 = GetMaxChars(config.payeeLine2, 80);
                            var payeeLines = SplitTextIntoLines(payeeStr, new[] { maxL1, maxL2 });
                            if (payeeLines.Length > 0) AddField(config.payeeLine1, payeeLines[0], true);
                            if (payeeLines.Length > 1) AddField(config.payeeLine2, payeeLines[1], true);
                        }
                        else
                        {
                            AddField(config.payeeLine1, payeeStr, true);
                        }

                        // Amount Words
                        string words = cheque.AmountInWords;
                        if (config.amountWordsLine2 != null || config.amountWordsLine3 != null)
                        {
                            int maxW1 = GetMaxChars(config.amountWordsLine1, 52);
                            int maxW2 = GetMaxChars(config.amountWordsLine2, 58);
                            int maxW3 = GetMaxChars(config.amountWordsLine3, 58);
                            var wordLines = SplitTextIntoLines(words, new[] { maxW1, maxW2, maxW3 });
                            
                            if (wordLines.Length > 0) AddField(config.amountWordsLine1, wordLines[0]);
                            if (wordLines.Length > 1 && config.amountWordsLine2 != null) AddField(config.amountWordsLine2, wordLines[1]);
                            if (wordLines.Length > 2 && config.amountWordsLine3 != null) AddField(config.amountWordsLine3, wordLines[2]);
                            
                            // Fallback if line2 is null but line3 exists (unlikely, but safe)
                            if (wordLines.Length > 1 && config.amountWordsLine2 == null && config.amountWordsLine3 != null)
                            {
                                AddField(config.amountWordsLine3, wordLines[1] + " " + (wordLines.Length > 2 ? wordLines[2] : ""));
                            }
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
                                CrossingType.NotNegotiable => "NOT NEGOTIABLE",
                                CrossingType.AccountPayeeAndNotNegotiable => "A/C PAYEE ONLY\nNOT NEGOTIABLE",
                                CrossingType.CrossAccountPayeeAndOrBearer => "A/C PAYEE ONLY",
                                CrossingType.CrossAccountPayeeAndNotNegotiableAndOrBearer => "A/C PAYEE ONLY\nNOT NEGOTIABLE",
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

                            float fontSz = cfg.fontSize > 0 ? cfg.fontSize : 11f;

                            layers.Layer()
                                .Unconstrained()
                                .OffsetX(posX, Unit.Millimetre)
                                .OffsetY(posY, Unit.Millimetre)
                                .Rotate(cfg.angle)
                                .Width(fieldW, Unit.Millimetre)
                                .Height(fieldH, Unit.Millimetre)
                                .AlignLeft()
                                .AlignMiddle()
                                .Text(txt =>
                                {
                                    txt.Span("XXXX").FontSize(fontSz).FontColor(Colors.Black).Bold();
                                });
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

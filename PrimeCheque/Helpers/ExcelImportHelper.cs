using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Helpers
{
    public static class ExcelImportHelper
    {
        public static async Task<List<BatchImportRow>> ParseExcelOrCsvAsync(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            if (extension == ".csv" || extension == ".txt")
            {
                return await CsvImportHelper.ParseCsvAsync(filePath);
            }
            else if (extension == ".xml" || extension == ".xlsx" || extension == ".xls")
            {
                return await ParseXmlOrTabbedSpreadsheetAsync(filePath);
            }
            else
            {
                return await CsvImportHelper.ParseCsvAsync(filePath);
            }
        }

        private static async Task<List<BatchImportRow>> ParseXmlOrTabbedSpreadsheetAsync(string filePath)
        {
            var results = new List<BatchImportRow>();
            var content = await File.ReadAllTextAsync(filePath);

            // Check if file is XML Spreadsheet 2003 format
            if (content.Contains("<Workbook", StringComparison.OrdinalIgnoreCase) || content.Contains("<Row", StringComparison.OrdinalIgnoreCase))
            {
                var rowMatches = Regex.Matches(content, @"<Row[^>]*>(.*?)</Row>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                int rowNo = 0;
                bool isHeader = true;

                foreach (Match rowMatch in rowMatches)
                {
                    rowNo++;
                    var rowContent = rowMatch.Groups[1].Value;
                    var cellMatches = Regex.Matches(rowContent, @"<Data[^>]*>(.*?)</Data>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    var cellValues = new List<string>();
                    foreach (Match cellMatch in cellMatches)
                    {
                        cellValues.Add(cellMatch.Groups[1].Value.Trim());
                    }

                    if (cellValues.Count == 0) continue;

                    if (isHeader)
                    {
                        if (cellValues[0].Equals("Payee", StringComparison.OrdinalIgnoreCase) || cellValues[0].Equals("PayeeName", StringComparison.OrdinalIgnoreCase))
                        {
                            isHeader = false;
                            continue;
                        }
                        isHeader = false;
                    }

                    var item = ProcessRowValues(rowNo, cellValues.ToArray());
                    results.Add(item);
                }

                if (results.Count > 0) return results;
            }

            // Fallback for TSV / tab-delimited or plain text
            var lines = await File.ReadAllLinesAsync(filePath);
            int lineNo = 0;
            bool isHeaderLine = true;

            foreach (var line in lines)
            {
                lineNo++;
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                var parts = trimmed.Split(new[] { '\t', ',' });
                if (isHeaderLine)
                {
                    if (parts[0].Equals("Payee", StringComparison.OrdinalIgnoreCase) || parts[0].Equals("PayeeName", StringComparison.OrdinalIgnoreCase))
                    {
                        isHeaderLine = false;
                        continue;
                    }
                    isHeaderLine = false;
                }

                results.Add(ProcessRowValues(lineNo, parts));
            }

            return results;
        }

        private static BatchImportRow ProcessRowValues(int rowNo, string[] parts)
        {
            var item = new BatchImportRow { RowNumber = rowNo };

            if (parts.Length < 2)
            {
                item.IsValid = false;
                item.ErrorMessage = "Row must contain at least Payee Name and Amount.";
                return item;
            }

            item.PayeeName = parts[0].Trim();
            if (string.IsNullOrWhiteSpace(item.PayeeName))
            {
                item.IsValid = false;
                item.ErrorMessage = "Payee Name cannot be empty.";
            }

            if (!decimal.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amt) || amt <= 0)
            {
                item.IsValid = false;
                item.ErrorMessage = string.IsNullOrEmpty(item.ErrorMessage) ? "Invalid amount (must be positive number)." : item.ErrorMessage + " Invalid amount.";
            }
            else
            {
                item.Amount = amt;
            }

            if (parts.Length >= 3 && DateOnly.TryParse(parts[2].Trim(), out var d))
            {
                item.ChequeDate = d;
            }
            else
            {
                item.ChequeDate = DateOnly.FromDateTime(DateTime.Today);
            }

            if (parts.Length >= 4)
            {
                item.Memo = parts[3].Trim();
            }

            if (parts.Length >= 5 && Enum.TryParse<CrossingType>(parts[4].Trim(), true, out var crossing))
            {
                item.CrossingType = crossing;
            }

            return item;
        }
    }
}

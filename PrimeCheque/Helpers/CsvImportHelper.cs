using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Helpers
{
    public class BatchImportRow
    {
        public int RowNumber { get; set; }
        public string PayeeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateOnly ChequeDate { get; set; }
        public string Memo { get; set; } = string.Empty;
        public CrossingType CrossingType { get; set; } = CrossingType.AccountPayeeOnly;
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public static class CsvImportHelper
    {
        public static async Task<List<BatchImportRow>> ParseCsvAsync(string filePath)
        {
            var results = new List<BatchImportRow>();
            var lines = await File.ReadAllLinesAsync(filePath);

            int rowNo = 0;
            bool isHeader = true;

            foreach (var rawLine in lines)
            {
                rowNo++;
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (isHeader)
                {
                    // Check if first line is header
                    if (parts[0].Equals("Payee", StringComparison.OrdinalIgnoreCase) || parts[0].Equals("PayeeName", StringComparison.OrdinalIgnoreCase))
                    {
                        isHeader = false;
                        continue;
                    }
                    isHeader = false;
                }

                var item = new BatchImportRow { RowNumber = rowNo };

                if (parts.Length < 2)
                {
                    item.IsValid = false;
                    item.ErrorMessage = "Line must contain at least Payee and Amount (comma separated).";
                    results.Add(item);
                    continue;
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
                    item.ErrorMessage = item.ErrorMessage.Length > 0 ? item.ErrorMessage + " Invalid amount." : "Invalid amount (must be positive number).";
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

                results.Add(item);
            }

            return results;
        }
    }
}

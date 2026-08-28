using System;
using System.Text;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class AmountToWordsService : IAmountToWordsService
    {
        public static AmountToWordsOptions DefaultOptions { get; set; } = new();

        private static readonly string[] Units = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static readonly string[] Tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public string Convert(decimal amount, AmountToWordsOptions? options = null)
        {
            options ??= DefaultOptions;

            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(amount));

            long integerPart = (long)Math.Floor(amount);
            int centsPart = (int)Math.Round((amount - integerPart) * 100);

            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(options.StartSymbol))
                sb.Append(options.StartSymbol).Append(' ');

            if (!string.IsNullOrWhiteSpace(options.Prefix))
                sb.Append(options.Prefix).Append(' ');

            if (integerPart == 0)
            {
                sb.Append("Zero");
            }
            else
            {
                sb.Append(ConvertNumberToWords(integerPart));
            }

            if (centsPart > 0)
            {
                if (options.UseAnd)
                    sb.Append(" and ");
                else
                    sb.Append(" ");

                if (!string.IsNullOrWhiteSpace(options.CentsWord))
                    sb.Append(options.CentsWord).Append(' ');

                sb.Append(ConvertNumberToWords(centsPart));
            }

            if (!string.IsNullOrWhiteSpace(options.Suffix))
                sb.Append(' ').Append(options.Suffix);

            if (!string.IsNullOrWhiteSpace(options.EndSymbol))
                sb.Append(' ').Append(options.EndSymbol);

            string result = sb.ToString().Trim();

            if (options.Uppercase)
                result = result.ToUpperInvariant();

            return result;
        }

        private static string ConvertNumberToWords(long number)
        {
            if (number == 0)
                return "";

            if (number < 20)
                return Units[number];

            if (number < 100)
            {
                return Tens[number / 10] + (number % 10 > 0 ? "-" + Units[number % 10] : "");
            }

            if (number < 1000)
            {
                return Units[number / 100] + " Hundred" + (number % 100 > 0 ? " " + ConvertNumberToWords(number % 100) : "");
            }

            if (number < 1000000)
            {
                return ConvertNumberToWords(number / 1000) + " Thousand" + (number % 1000 > 0 ? " " + ConvertNumberToWords(number % 1000) : "");
            }

            if (number < 1000000000)
            {
                return ConvertNumberToWords(number / 1000000) + " Million" + (number % 1000000 > 0 ? " " + ConvertNumberToWords(number % 1000000) : "");
            }

            return ConvertNumberToWords(number / 1000000000) + " Billion" + (number % 1000000000 > 0 ? " " + ConvertNumberToWords(number % 1000000000) : "");
        }
    }
}

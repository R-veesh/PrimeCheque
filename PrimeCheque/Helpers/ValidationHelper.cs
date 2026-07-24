using System;

namespace PrimeCheque.Helpers
{
    public static class ValidationHelper
    {
        public static (bool IsValid, string ErrorMessage) ValidateChequeDate(DateOnly chequeDate, int maxBackdays = 90, int maxFutureDays = 180)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            
            if (chequeDate < today.AddDays(-maxBackdays))
            {
                return (false, $"Cheque date cannot be older than {maxBackdays} days (Stale Cheque warning).");
            }

            if (chequeDate > today.AddDays(maxFutureDays))
            {
                return (false, $"Post-dated cheque date cannot exceed {maxFutureDays} days into the future.");
            }

            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidateChequeAmount(decimal amount, decimal maxAmount = 100_000_000m)
        {
            if (amount <= 0)
            {
                return (false, "Cheque amount must be greater than zero.");
            }

            if (amount > maxAmount)
            {
                return (false, $"Cheque amount exceeds maximum allowed limit of LKR {maxAmount:N2}.");
            }

            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidatePayeeName(string? payeeName)
        {
            if (string.IsNullOrWhiteSpace(payeeName))
            {
                return (false, "Payee name is required.");
            }

            var trimmed = payeeName.Trim();
            if (trimmed.Length < 2)
            {
                return (false, "Payee name must be at least 2 characters long.");
            }

            if (trimmed.Length > 150)
            {
                return (false, "Payee name cannot exceed 150 characters.");
            }

            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidateChequeNumberRange(int startNo, int endNo)
        {
            if (startNo <= 0)
            {
                return (false, "Start cheque number must be greater than zero.");
            }

            if (endNo < startNo)
            {
                return (false, "End cheque number must be greater than or equal to start cheque number.");
            }

            if (endNo - startNo > 1000)
            {
                return (false, "Cheque book capacity cannot exceed 1,000 leaves per book.");
            }

            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidateAccountNumber(string? accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return (false, "Account number is required.");
            }

            var cleaned = accountNumber.Trim().Replace(" ", "").Replace("-", "");
            if (cleaned.Length < 5 || cleaned.Length > 30)
            {
                return (false, "Account number must be between 5 and 30 characters.");
            }

            return (true, string.Empty);
        }
    }
}

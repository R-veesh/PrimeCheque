using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PrimeCheque.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("PrimeCheque_AES_SecretKey_2026!!"); // 32 bytes

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cs))
            {
                writer.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            var fullBytes = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = Key;

            var iv = new byte[16];
            Array.Copy(fullBytes, 0, iv, 0, 16);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(fullBytes, 16, fullBytes.Length - 16);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);

            return reader.ReadToEnd();
        }

        public static string MaskAccountNumber(string accountNo)
        {
            if (string.IsNullOrWhiteSpace(accountNo) || accountNo.Length <= 4)
                return "****";

            return new string('*', accountNo.Length - 4) + accountNo.Substring(accountNo.Length - 4);
        }
    }
}

using System;
using System.Text;

namespace BlockChain.CLI.Bitcoin.Core
{
    public static partial class EncodingExtensions
    {
        public static string EncodeBase64(this Encoding encoding, string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            var plainTextBytes = encoding.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string DecodeBase64(this Encoding encoding, string encodedData)
        {
            if (string.IsNullOrWhiteSpace(encodedData))
                return string.Empty;

            var base64EncodedBytes = Convert.FromBase64String(encodedData);
            return encoding.GetString(base64EncodedBytes);
        }

        public static string EncodeBase64(this byte[] plainText) => plainText == null ? string.Empty : Convert.ToBase64String(plainText);

        public static byte[] DecodeBase64(this string encodedData) => string.IsNullOrWhiteSpace(encodedData) ? new byte[] { } : Convert.FromBase64String(encodedData);
    }
}

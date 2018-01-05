using System;
using System.Text;

namespace BlockChain.CLI.BitCoin
{
    public static class Extensions
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

        public static string EncodeBase58(this Encoding encoding, string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            var plainTextBytes = encoding.GetBytes(plainText);
            return EncodeBase58(plainTextBytes);
        }

        public static string DecodeBase58(this Encoding encoding, string encodedData)
        {
            if (string.IsNullOrWhiteSpace(encodedData))
                return string.Empty;

            var base64EncodedBytes = DecodeBase58(encodedData);
            return encoding.GetString(base64EncodedBytes);
        }

        const string BASE58_ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static string EncodeBase58(this uint number)
        {
            var sb = new StringBuilder();

            while (number > 0)
            {
                var remainder = (number % 58);
                number = Convert.ToUInt32(number / 58);
                sb.Insert(0, BASE58_ALPHABET[Convert.ToInt32(remainder)]);
            }

            return sb.ToString();
        }

        public static uint DecodeBase58Uint(this string encodedData)
        {
            long decoded = 0;
            long multi = 1;

            while (encodedData.Length > 0)
            {
                var sCurrentCharacter = encodedData.Substring(encodedData.Length - 1);
                decoded = decoded + (multi * BASE58_ALPHABET.IndexOf(sCurrentCharacter, StringComparison.Ordinal));
                multi = multi * BASE58_ALPHABET.Length;
                encodedData = encodedData.Substring(0, encodedData.Length - 1);
            }

            return Convert.ToUInt32(decoded);
        }

        public static string EncodeBase58(this byte[] plainText)
        {
            var sb = new StringBuilder();

            //while (number > 0)
            //{
            //    var remainder = (number % 58);
            //    number = Convert.ToUInt32(number / 58);
            //    sb.Insert(0, BASE58_ALPHABET[Convert.ToInt32(remainder)]);
            //}

            return sb.ToString();
        }

        public static byte[] DecodeBase58(this string encodedData)
        {
            int i = 0;

            while (i < encodedData.Length)
            {
                if (encodedData[i] == 0 || !Char.IsWhiteSpace(encodedData[i]))
                {
                    break;
                }
                i++;
            }

            int zeros = 0;

            while (encodedData[i] == '1')
            {
                zeros++;
                i++;
            }

            byte[] b256 = new byte[(encodedData.Length - i) * 733 / 1000 + 1];

            while (i < encodedData.Length && !Char.IsWhiteSpace(encodedData[i]))
            {
                int ch = BASE58_ALPHABET.IndexOf(encodedData[i]);

                if (ch == -1) //null
                {
                    return new byte[] { };
                }

                int carry = BASE58_ALPHABET.IndexOf(encodedData[i]);

                for (int k = b256.Length - 1; k >= 0; k--)
                {
                    carry += 58 * b256[k];
                    b256[k] = (byte)(carry % 256);
                    carry /= 256;
                }
                i++;
            }

            while (i < encodedData.Length && Char.IsWhiteSpace(encodedData[i]))
            {
                i++;
            }

            if (i != encodedData.Length)
            {
                return new byte[] { };
            }

            int j = 0;

            while (j < b256.Length && b256[j] == 0)
            {
                j++;
            }

            var plainTextBytes = new byte[zeros + (b256.Length - j)];
            for (int kk = 0; kk < plainTextBytes.Length; kk++)
            {
                if (kk < zeros)
                {
                    plainTextBytes[kk] = 0x00;
                }
                else
                {
                    plainTextBytes[kk] = b256[j++];
                }
            }
            return plainTextBytes;
        }
    }
}

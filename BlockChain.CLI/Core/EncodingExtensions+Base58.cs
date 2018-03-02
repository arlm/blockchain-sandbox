using System;
using System.Text;

namespace BlockChain.CLI.Bitcoin.Core
{
    public static partial class EncodingExtensions
    {
        const string BASE58_ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

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

            var base58EncodedBytes = DecodeBase58(encodedData);
            return encoding.GetString(base58EncodedBytes);
        }

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

        // Code based on https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp
        public static string EncodeBase58(this byte[] plainText)
        {
            if ((plainText?.Length ?? 0) == 0)
                return string.Empty;

            int zeroes = 0;
            int length = 0;
            int begin = 0;

            while (begin != plainText.Length && plainText[begin] == 0)
            {
                begin++;
                zeroes++;
            }

            int size = (plainText.Length - begin) * 138 / 100 + 1; // log(256) / log(58), rounded up.
            var b58 = new byte[size];

            while (begin != plainText.Length)
            {
                int carry = plainText[begin];
                int tempLength = 0;

                for (int pointer = b58.Length - 1; (carry != 0 || tempLength < length) && (pointer >= 0); pointer--, tempLength++)
                {
                    carry += 256 * b58[pointer];
                    b58[pointer] = (byte)(carry % 58);
                    carry /= 58;
                }

                length = tempLength;
                begin++;
            }

            int index = size - length;

            while (index < b58.Length && b58[index] == 0)
            {
                index++;
            }

            var destination = new char[zeroes + (b58.Length - index)];
            for (int copyIndex = 0; copyIndex < destination.Length; copyIndex++)
            {
                if (copyIndex < zeroes)
                {
                    destination[copyIndex] = '1';
                }
                else
                {
                    destination[copyIndex] = BASE58_ALPHABET[b58[index++]];
                }
            }

            return new string(destination);
        }

        // Code based on https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp
        public static byte[] DecodeBase58(this string encodedData)
        {
            int begin = 0;

            while (begin < encodedData.Length)
            {
                if (encodedData[begin] == 0 || !Char.IsWhiteSpace(encodedData[begin]))
                {
                    break;
                }
                begin++;
            }

            int zeroes = 0;

            while (begin != encodedData.Length && encodedData[begin] == '1')
            {
                zeroes++;
                begin++;
            }

            int size = (encodedData.Length - begin) * 733 / 1000 + 1; // log(58) / log(256), rounded up.
            byte[] b256 = new byte[size];

            while (begin < encodedData.Length && !Char.IsWhiteSpace(encodedData[begin]))
            {
                int ch = BASE58_ALPHABET.IndexOf(encodedData[begin]);

                if (ch == -1) //null
                {
                    return new byte[] { };
                }

                int carry = BASE58_ALPHABET.IndexOf(encodedData[begin]);

                for (int pointer = b256.Length - 1; pointer >= 0; pointer--)
                {
                    carry += 58 * b256[pointer];
                    b256[pointer] = (byte)(carry % 256);
                    carry /= 256;
                }
                begin++;
            }

            while (begin < encodedData.Length && Char.IsWhiteSpace(encodedData[begin]))
            {
                begin++;
            }

            if (begin != encodedData.Length)
            {
                return new byte[] { };
            }

            int index = 0;

            while (index < b256.Length && b256[index] == 0)
            {
                index++;
            }

            var plainTextBytes = new byte[zeroes + (b256.Length - index)];
            for (int copyIndex = 0; copyIndex < plainTextBytes.Length; copyIndex++)
            {
                if (copyIndex < zeroes)
                {
                    plainTextBytes[copyIndex] = 0x00;
                }
                else
                {
                    plainTextBytes[copyIndex] = b256[index++];
                }
            }
            return plainTextBytes;
        }
    }
}

using System;
using System.Text;

namespace BlockChain.CLI.Bitcoin.Core
{
    public static partial class EncodingExtensions
    {
        const string BASE16_ALPHABET = "0123456789ABCDEF";

        public static string EncodeBase16(this Encoding encoding, string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            var plainTextBytes = encoding.GetBytes(plainText);
            return EncodeBase16(plainTextBytes);
        }

        public static string DecodeBase16(this Encoding encoding, string encodedData)
        {
            if (string.IsNullOrWhiteSpace(encodedData))
                return string.Empty;

            var base16EncodedBytes = DecodeBase16(encodedData);
            return encoding.GetString(base16EncodedBytes);
        }

        public static string EncodeBase16(this uint number)
        {
            var sb = new StringBuilder();

            while (number > 0)
            {
                var remainder = (number % 16);
                number = Convert.ToUInt32(number / 16);
                sb.Insert(0, BASE16_ALPHABET[Convert.ToInt32(remainder)]);
            }

            return sb.ToString();
        }

        public static uint DecodeBase16Uint(this string encodedData)
        {
            long decoded = 0;
            long multi = 1;

            while (encodedData.Length > 0)
            {
                var sCurrentCharacter = encodedData.Substring(encodedData.Length - 1);
                decoded = decoded + (multi * BASE16_ALPHABET.IndexOf(sCurrentCharacter, StringComparison.Ordinal));
                multi = multi * BASE16_ALPHABET.Length;
                encodedData = encodedData.Substring(0, encodedData.Length - 1);
            }

            return Convert.ToUInt32(decoded);
        }

        public static string EncodeBase16(this byte[] plainText)
        {
            if ((plainText?.Length ?? 0) == 0)
                return string.Empty;

            int length = 0;
            int begin = 0;

            int size = plainText.Length * 2; // log(256) / log(16).
            var b16 = new byte[size];

            while (begin != plainText.Length)
            {
                int carry = plainText[begin];
                int tempLength = 0;

                for (int pointer = b16.Length - 1; (carry != 0 || tempLength < length) && (pointer >= 0); pointer--, tempLength++)
                {
                    carry += 256 * b16[pointer];
                    b16[pointer] = (byte)(carry % 16);
                    carry /= 16;
                }

                length = tempLength;
                begin++;
            }

            int index = size - length;

            while (index < b16.Length && b16[index] == 0)
            {
                index++;
            }

            var destination = new char[b16.Length - index];
            for (int copyIndex = 0; copyIndex < destination.Length; copyIndex++)
            {
                destination[copyIndex] = BASE16_ALPHABET[b16[index++]];
            }

            return new string(destination);
        }

        // Code based on https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp
        public static byte[] DecodeBase16(this string encodedData)
        {
            int begin = 0;

            int size = encodedData.Length / 2 + 1; // log(16) / log(256), rounded up.
            byte[] b256 = new byte[size];

            while (begin < encodedData.Length && !Char.IsWhiteSpace(encodedData[begin]))
            {
                int ch = BASE16_ALPHABET.IndexOf(encodedData[begin]);

                if (ch == -1) //null
                {
                    return new byte[] { };
                }

                int carry = BASE16_ALPHABET.IndexOf(encodedData[begin]);

                for (int pointer = b256.Length - 1; pointer >= 0; pointer--)
                {
                    carry += 16 * b256[pointer];
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

            var plainTextBytes = new byte[b256.Length - index];
            for (int copyIndex = 0; copyIndex < plainTextBytes.Length; copyIndex++)
            {
                plainTextBytes[copyIndex] = b256[index++];
            }
            return plainTextBytes;
        }
    }
}

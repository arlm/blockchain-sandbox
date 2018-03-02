using System;
using System.Text;

namespace BlockChain.CLI.Bitcoin.Core
{
    public static partial class EncodingExtensions
    {
        private static byte B32(int @char)
        {
            char _ = unchecked((char)@char);

            return unchecked((byte)((_) == 'A' ? 0
                : (_) == 'B' ? 1
                : (_) == 'C' ? 2
                : (_) == 'D' ? 3
                : (_) == 'E' ? 4
                : (_) == 'F' ? 5
                : (_) == 'G' ? 6
                : (_) == 'H' ? 7
                : (_) == 'I' ? 8
                : (_) == 'J' ? 9
                : (_) == 'K' ? 10
                : (_) == 'L' ? 11
                : (_) == 'M' ? 12
                : (_) == 'N' ? 13
                : (_) == 'O' ? 14
                : (_) == 'P' ? 15
                : (_) == 'Q' ? 16
                : (_) == 'R' ? 17
                : (_) == 'S' ? 18
                : (_) == 'T' ? 19
                : (_) == 'U' ? 20
                : (_) == 'V' ? 21
                : (_) == 'W' ? 22
                : (_) == 'X' ? 23
                : (_) == 'Y' ? 24
                : (_) == 'Z' ? 25
                : (_) == '2' ? 26
                : (_) == '3' ? 27
                : (_) == '4' ? 28
                : (_) == '5' ? 29
                : (_) == '6' ? 30
                : (_) == '7' ? 31
                : -1));
        }

        const string BASE32_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static string EncodeBase32(this Encoding encoding, string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            var plainTextBytes = encoding.GetBytes(plainText);
            return EncodeBase32(plainTextBytes);
        }

        public static string DecodeBase32(this Encoding encoding, string encodedData)
        {
            if (string.IsNullOrWhiteSpace(encodedData))
                return string.Empty;

            var base32EncodedBytes = DecodeBase32(encodedData);
            return encoding.GetString(base32EncodedBytes);
        }

        public static string EncodeBase32(this uint number)
        {
            var sb = new StringBuilder();

            while (number > 0)
            {
                var remainder = (number % 32);
                number = Convert.ToUInt32(number / 32);
                sb.Insert(0, BASE32_ALPHABET[Convert.ToInt32(remainder)]);
            }

            return sb.ToString();
        }

        public static uint DecodeBase32Uint(this string encodedData)
        {
            long decoded = 0;
            long multi = 1;

            while (encodedData.Length > 0)
            {
                var sCurrentCharacter = encodedData.Substring(encodedData.Length - 1);
                decoded = decoded + (multi * BASE32_ALPHABET.IndexOf(sCurrentCharacter, StringComparison.Ordinal));
                multi = multi * BASE32_ALPHABET.Length;
                encodedData = encodedData.Substring(0, encodedData.Length - 1);
            }

            return Convert.ToUInt32(decoded);
        }

        // Code based on https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp
        public static string EncodeBase32(this byte[] plainText)
        {
            if ((plainText?.Length ?? 0) == 0)
                return string.Empty;

            int inLength = plainText.Length;
            int index = 0;

            // This uses that the expression (n+(k-1))/k means the smallest
            // integer >= n / k, i.e., the ceiling of n/ k.
            int outLength = ((plainText.Length + 6) / 5) * 8;
            var b32 = new StringBuilder(outLength);

            while (inLength > 0 && outLength > 0)
            {
                b32.Append(BASE32_ALPHABET[(plainText[index] >> 3) & 0x1F]);

                outLength--;
                if (outLength == 0)
                    break;

                b32.Append(BASE32_ALPHABET[((plainText[index] << 2) & 0x1C) + (((inLength - 1) > 0 ? plainText[index + 1] >> 6 : 0) & 0x03)]);
                inLength--;

                outLength--;
                if (outLength == 0)
                    break;

                b32.Append(inLength > 0 ? BASE32_ALPHABET[(plainText[index + 1] >> 1) & 0x1F] : '=');

                if (outLength == 0)
                    break;

                b32.Append(inLength > 0 ? BASE32_ALPHABET[((plainText[index + 1] << 4) & 0x10) + (((inLength - 1) > 0 ? plainText[index + 2] >> 4 : 0) & 0x0F)] : '=');
                inLength--;

                if (outLength == 0)
                    break;

                b32.Append(inLength > 0 ? BASE32_ALPHABET[((plainText[index + 2] << 1) & 0x1E) + (((inLength - 2) > 0 ? plainText[index + 3] >> 7 : 0) & 0x01)] : '=');
                inLength--;

                if (outLength == 0)
                    break;

                b32.Append(inLength > 0 ? BASE32_ALPHABET[(plainText[index + 3] >> 2) & 0x1F] : '=');

                if (outLength == 0)
                    break;

                b32.Append(inLength > 0 ? BASE32_ALPHABET[((plainText[index + 3] << 3) & 0x18) + (((inLength - 1) > 0 ? plainText[index + 4] >> 5 : 0) & 0x07)] : '=');
                inLength--;

                if (outLength == 0)
                    break;

                b32.Append(inLength > 0 ? BASE32_ALPHABET[plainText[index + 4] & 0x1F] : '=');
                inLength--;

                outLength--;
                if (outLength == 0)
                    break;

                if (inLength > 0)
                    index += 5;
            }

            return b32.ToString();
        }

        // Code based on https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp
        public static byte[] DecodeBase32(this string encodedData)
        {
            int inLength = encodedData.Length;
            int index = 0;
            int outIndex = 0;

            // This may allocate a few bytes too much, depending on input,
            // but it's not worth the extra CPU time to compute the exact amount.
            // The exact amount is 5 * inlen / 8, minus 1 if the input ends
            // with "=" and minus another 1 if the input ends with "==", 
            // and so on until "======".
            // Dividing before multiplying avoids the possibility of overflow.
            int outLength = 5 * (encodedData.Length / 8) + 6;
            byte[] b256 = new byte[outLength];

            while (inLength >= 8)
            {
                if (BASE32_ALPHABET.IndexOf(encodedData[index]) < 0 ||
                    BASE32_ALPHABET.IndexOf(encodedData[index + 1]) < 0)
                    break;

                if (outLength > 0)
                {
                    b256[outIndex] = unchecked((byte)((B32(encodedData[index]) << 3) | (B32(encodedData[index + 1]) >> 2)));
                    outLength--;
                    outIndex++;
                }

                if (inLength == 2)
                    break;

                if (encodedData[index + 2] == '=')
                {
                    if (inLength != 8)
                        break;

                    if (encodedData[index + 3] != '=')
                        break;

                    if (encodedData[index + 4] != '=')
                        break;

                    if (encodedData[index + 5] != '=')
                        break;

                    if (encodedData[index + 6] != '=')
                        break;

                    if (encodedData[index + 7] != '=')
                        break;
                }
                else
                {
                    if ((BASE32_ALPHABET.IndexOf(encodedData[index + 2]) < 0) ||
                        (BASE32_ALPHABET.IndexOf(encodedData[index + 3]) < 0))
                        break;

                    if (outLength > 0)
                    {
                        b256[outIndex] = unchecked((byte)(((B32(encodedData[index + 1]) << 6) & 0xC0) | ((B32(encodedData[index + 2]) << 1) & 0x3E) | (B32(encodedData[index + 3]) >> 4)));
                        outLength--;
                        outIndex++;
                    }

                    if (inLength == 4)
                        break;

                    if (encodedData[index + 4] == '=')
                    {
                        if (inLength != 8)
                            break;

                        if (encodedData[index + 5] != '=')
                            break;

                        if (encodedData[index + 6] != '=')
                            break;

                        if (encodedData[index + 7] != '=')
                            break;
                    }
                    else
                    {
                        if (BASE32_ALPHABET.IndexOf(encodedData[index + 4]) < 0)
                            break;

                        if (outLength > 0)
                        {
                            b256[outIndex] = unchecked((byte)(((B32(encodedData[index + 3]) << 4) & 0xF0) | (B32(encodedData[index + 4]) >> 1)));
                            outLength--;
                            outIndex++;
                        }

                        if (inLength == 5)
                            break;

                        if (encodedData[index + 5] == '=')
                        {
                            if (inLength != 8)
                                break;

                            if (encodedData[index + 6] != '=')
                                break;

                            if (encodedData[index + 7] != '=')
                                break;
                        }
                        else
                        {
                            if ((BASE32_ALPHABET.IndexOf(encodedData[index + 5]) < 0) ||
                                (BASE32_ALPHABET.IndexOf(encodedData[index + 6]) < 0))
                                break;

                            if (outLength > 0)
                            {
                                b256[outIndex] = unchecked((byte)(((B32(encodedData[index + 4]) << 7) & 0x80) | ((B32(encodedData[index + 5]) << 2) & 0x7C) | ((B32(encodedData[index + 6]) >> 3) & 0x03)));
                                outLength--;
                                outIndex++;
                            }

                            if (inLength == 7)
                                break;

                            if (encodedData[index + 7] == '=')
                            {
                                if (inLength != 8)
                                    break;
                            }
                            else
                            {
                                if (BASE32_ALPHABET.IndexOf(encodedData[index + 7]) < 0)
                                    break;

                                if (outLength > 0)
                                {
                                    b256[outIndex] = unchecked((byte)(((B32(encodedData[index + 6]) << 5) & 0xE0) | B32(encodedData[index + 7])));
                                    outLength--;
                                    outIndex++;
                                }
                            }
                        }
                    }
                }

                index += 8;
                inLength -= 8;
            }

            if (inLength != 0)
                return new byte[] { };

            return b256.SubArray(0, b256.Length - outLength);
        }
    }
}

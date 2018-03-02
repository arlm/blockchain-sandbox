using System;
using System.Text;

namespace BlockChain.CLI.Bitcoin.Core
{
    public static partial class EncodingExtensions
    {
        private static byte B64Url(int @char)
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
                : (_) == 'a' ? 26
                : (_) == 'b' ? 27
                : (_) == 'c' ? 28
                : (_) == 'd' ? 29
                : (_) == 'e' ? 30
                : (_) == 'f' ? 31
                : (_) == 'g' ? 32
                : (_) == 'h' ? 33
                : (_) == 'i' ? 34
                : (_) == 'j' ? 35
                : (_) == 'k' ? 36
                : (_) == 'l' ? 37
                : (_) == 'm' ? 38
                : (_) == 'n' ? 39
                : (_) == 'o' ? 40
                : (_) == 'p' ? 41
                : (_) == 'q' ? 42
                : (_) == 'r' ? 43
                : (_) == 's' ? 44
                : (_) == 't' ? 45
                : (_) == 'u' ? 46
                : (_) == 'v' ? 47
                : (_) == 'w' ? 48
                : (_) == 'x' ? 49
                : (_) == 'y' ? 50
                : (_) == 'z' ? 51
                : (_) == '0' ? 52
                : (_) == '1' ? 53
                : (_) == '2' ? 54
                : (_) == '3' ? 55
                : (_) == '4' ? 56
                : (_) == '5' ? 57
                : (_) == '6' ? 58
                : (_) == '7' ? 59
                : (_) == '8' ? 60
                : (_) == '9' ? 61
                : (_) == '-' ? 62
                : (_) == '_' ? 63
                : -1));
        }

        //const string BASE64_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        const string BASE64_URL_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

        public static string EncodeBase64Url(this Encoding encoding, string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            var plainTextBytes = encoding.GetBytes(plainText);
            return EncodeBase64Url(plainTextBytes);
        }

        public static string DecodeBase64Url(this Encoding encoding, string encodedData)
        {
            if (string.IsNullOrWhiteSpace(encodedData))
                return string.Empty;

            var base64UrlEncodedBytes = DecodeBase64Url(encodedData);
            return encoding.GetString(base64UrlEncodedBytes);
        }

        public static string EncodeBase64Url(this uint number)
        {
            var sb = new StringBuilder();

            while (number > 0)
            {
                var remainder = (number % 64);
                number = Convert.ToUInt32(number / 64);
                sb.Insert(0, BASE64_URL_ALPHABET[Convert.ToInt32(remainder)]);
            }

            return sb.ToString();
        }

        public static uint DecodeBase64UrlUint(this string encodedData)
        {
            long decoded = 0;
            long multi = 1;

            while (encodedData.Length > 0)
            {
                var sCurrentCharacter = encodedData.Substring(encodedData.Length - 1);
                decoded = decoded + (multi * BASE64_URL_ALPHABET.IndexOf(sCurrentCharacter, StringComparison.Ordinal));
                multi = multi * BASE64_URL_ALPHABET.Length;
                encodedData = encodedData.Substring(0, encodedData.Length - 1);
            }

            return Convert.ToUInt32(decoded);
        }

        // Code based on http://cvs.savannah.gnu.org/viewvc/gnulib/gnulib/lib/base64.c?view=markup&content-type=text%2Fvnd.viewcvs-markup&revision=HEAD
        public static string EncodeBase64Url(this byte[] plainText)
        {
            if ((plainText?.Length ?? 0) == 0)
                return string.Empty;

            int inLength = plainText.Length;
            int index = 0;

            // This uses that the expression (n+(k-1))/k means the smallest
            // integer >= n / k, i.e., the ceiling of n/ k.
            int outLength = ((plainText.Length + 2) / 3) * 4;
            var b64 = new StringBuilder(outLength);

            while (inLength > 0 && outLength > 0)
            {
                b64.Append(BASE64_URL_ALPHABET[(plainText[index] >> 2) & 0x3F]);

                outLength--;
                if (outLength == 0)
                    break;

                b64.Append(BASE64_URL_ALPHABET[(plainText[index] << 4) + (inLength - 1 > 0 ? plainText[index + 1] >> 4 : 0) & 0x3F]);
                inLength--;

                outLength--;
                if (outLength == 0)
                    break;

                b64.Append(inLength > 0 ? BASE64_URL_ALPHABET[(plainText[index + 1] << 2) + ((inLength - 1) > 0 ? plainText[index + 2] >> 6 : 0) & 0x3F] : '=');
                inLength--;

                if (outLength == 0)
                    break;

                b64.Append(inLength > 0 ? BASE64_URL_ALPHABET[plainText[index + 2] & 0x3F] : '=');

                outLength--;
                if (outLength == 0)
                    break;

                if (inLength > 0)
                    inLength--;

                if (inLength > 0)
                    index += 3;
            }

            return b64.ToString();
        }

        // Code based on http://cvs.savannah.gnu.org/viewvc/gnulib/gnulib/lib/base64.c?view=markup&content-type=text%2Fvnd.viewcvs-markup&revision=HEAD
        public static byte[] DecodeBase64Url(this string encodedData)
        {
            int inLength = encodedData.Length;
            int index = 0;
            int outIndex = 0;

            // This may allocate a few bytes too much, depending on input,
            // but it's not worth the extra CPU time to compute the exact amount.
            // The exact amount is 3 * inlen / 4, minus 1 if the input ends
            // with "=" and minus another 1 if the input ends with "==".
            // Dividing before multiplying avoids the possibility of overflow.
            int outLength = 3 * (encodedData.Length / 4) + 2;
            byte[] b256 = new byte[outLength];

            while (inLength >= 2)
            {
                if (BASE64_URL_ALPHABET.IndexOf(encodedData[index]) < 0 ||
                    BASE64_URL_ALPHABET.IndexOf(encodedData[index + 1]) < 0)
                    break;

                if (outLength > 0)
                {
                    b256[outIndex] = unchecked((byte)((B64Url(encodedData[index]) << 2) | (B64Url(encodedData[index + 1]) >> 4)));
                    outLength--;
                    outIndex++;
                }

                if (inLength == 2)
                    break;

                if (encodedData[index + 2] == '=')
                {
                    if (inLength != 4)
                        break;

                    if (encodedData[index + 3] != '=')
                        break;
                }
                else
                {
                    if (BASE64_URL_ALPHABET.IndexOf(encodedData[index + 2]) < 0)
                        break;

                    if (outLength > 0)
                    {
                        b256[outIndex] = unchecked((byte)(((B64Url(encodedData[index + 1]) << 4) & 0xF0) | (B64Url(encodedData[index + 2]) >> 2)));
                        outLength--;
                        outIndex++;
                    }

                    if (inLength == 3)
                        break;

                    if (encodedData[index + 3] == '=')
                    {
                        if (inLength != 4)
                            break;
                    }
                    else
                    {
                        if (BASE64_URL_ALPHABET.IndexOf(encodedData[index + 3]) < 0)
                            break;

                        if (outLength > 0)
                        {
                            b256[outIndex] = unchecked((byte)(((B64Url(encodedData[index + 2]) << 6) & 0xC0) | (B64Url(encodedData[index + 3]))));
                            outLength--;
                            outIndex++;
                        }
                    }
                }

                index += 4;
                inLength -= 4;
            }

            if (inLength != 0)
                return new byte[] { };

            return b256.SubArray(0, b256.Length - outLength);
        }
    }
}

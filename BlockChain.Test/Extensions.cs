using System;
namespace BlockChain.Test
{
    public static class Extensions
    {
        public static byte[] ToBytes(this string hexa)
        {
            if (string.IsNullOrWhiteSpace(hexa))
            {
                return new byte[] { };
            }

            if (hexa.Length % 2 != 0)
            {
                hexa = hexa.Insert(0, "0");
            }

            var result = new byte[hexa.Length / 2];

            for (int index = 0; index < hexa.Length; index += 2)
            {
                result[index / 2] = byte.Parse(hexa.Substring(index, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return result;
        }

        public static DateTime FromJavascriptUTC(this long milliseconds) => new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(milliseconds);
    }
}

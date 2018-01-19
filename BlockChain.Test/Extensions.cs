using System;
namespace BlockChain.Test
{
    public static class Extensions
    {
        public static DateTime FromJavascriptUTC(this long milliseconds) => new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(milliseconds);
    }
}

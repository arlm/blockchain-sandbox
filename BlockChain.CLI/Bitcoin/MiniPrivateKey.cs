using System.Security.Cryptography;
using System.Text;
using BlockChain.CLI.Core;
using BlockChain.Core;

namespace BlockChain.CLI.Bitcoin
{
    public static class MiniPrivateKey
    {
        public static string[] Generate(int count)
        {
            var result = new string[count];
            var sha256 = new SHA256Managed();


            return result;
        }

        public static bool ValidateMiniPrivKey(this string miniPrivKey)
        {
            int length = miniPrivKey?.Length ?? 0;

            if (!(length == 30 || length == 22))
            {
                return false;
            }

            if (miniPrivKey[0] != 'S')
            {
                return false;
            }

            var sha256 = new SHA256Managed();
            var checkSum = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{miniPrivKey}?"));

            return checkSum[0] == 0x00;
        }

        public static PrivateKey ExpandMiniPrivKey(this string miniPrivKey)
        {
            if (!ValidateMiniPrivKey(miniPrivKey))
            {
                return null;
            }

            var sha256 = new SHA256Managed();
            var privKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(miniPrivKey));

            return new PrivateKey(privKey);
        }
    }
}

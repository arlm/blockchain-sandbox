using System;
namespace BlockChain.CLI.Bitcoin
{
    public class PrivateKey
    {
        public byte[] Key { get; private set; }

        public PrivateKey()
        {

        }

        public PrivateKey(byte[] publicKey)
        {
            Key = publicKey;
        }
    }
}

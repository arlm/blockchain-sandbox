using System;
namespace BlockChain.CLI.Core
{
    public class PrivateKey : IKey
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

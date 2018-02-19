using System;
namespace BlockChain.CLI.Core
{
    public class PublicKey : IKey
    {
        public byte[] Key { get; private set; }

        public PublicKey()
        {
            
        }

        public PublicKey(byte[] publicKey)
        {
            Key = publicKey;
        }
    }
}

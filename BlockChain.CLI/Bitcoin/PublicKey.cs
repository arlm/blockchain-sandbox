using System;
namespace BlockChain.CLI.Bitcoin
{
    public class PublicKey
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

using System;
using BlockChain.CLI.Core.Interfaces;

namespace BlockChain.CLI.Core
{
    public class PublicKey : IKey
    {
        public byte[] Data { get; private set; }

        public PublicKey()
        {
            
        }

        public PublicKey(byte[] publicKey)
        {
            Data = publicKey;
        }
    }
}

using System;
using BlockChain.CLI.Core.Interfaces;

namespace BlockChain.CLI.Core
{
    public class PrivateKey : IKey
    {
        public byte[] Data { get; private set; }

        public PrivateKey()
        {

        }

        public PrivateKey(byte[] publicKey)
        {
            Data = publicKey;
        }
    }
}

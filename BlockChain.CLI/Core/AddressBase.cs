using System;
using BlockChain.CLI.Bitcoin;
using BlockChain.CLI.Core.Interfaces;

namespace BlockChain.CLI.Core
{
    public abstract class AddressBase<TAddress, TNetwork> : IAddress<TAddress, TNetwork>
            where TAddress : struct, IComparable
            where TNetwork : struct, IComparable
    {
        public IKey Key { get; protected set; }
        public string Address { get; protected set; }
        public INetworkVersion<TAddress, TNetwork> Version { get; protected set; }

        public int AddressSize { get; protected set; } = 0;
        public int ChecksumSize { get; protected set; } = 0;

        protected abstract (bool IsValid, NetworkVersion type) Verify(string address);
        protected abstract (byte[] Key, NetworkVersion type, byte[] Checksum) Extract(string address);
        protected abstract string Calculate(byte[] key, TNetwork type);

        public static (bool IsValid, INetworkVersion<TAddress, TNetwork> Type) Verify<TVersion>(string address)
            where TVersion : INetworkVersion<TAddress, TNetwork>, new()
        {
            try
            {
                var version = new TVersion();
                var resolvedAddress = version.Create(address);
                return (true, resolvedAddress.Version);
            }
            catch { }

            return (false, null);
        }


        internal static (byte[] Key, INetworkVersion<TAddress, TNetwork> Type) Extract<TVersion>(string address)
            where TVersion : INetworkVersion<TAddress, TNetwork>, new()
        {
            var version = new TVersion();
            var resolvedAddress = version.Create(address);
            return (resolvedAddress.Key.Data, resolvedAddress.Version);
        }
    }
}

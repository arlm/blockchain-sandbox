using System;
namespace BlockChain.CLI.Core.Interfaces
{
    public interface INetworkVersion<TAddress, TNetwork> : IComparable, IEquatable<INetworkVersion<TAddress, TNetwork>>, IComparable<INetworkVersion<TAddress, TNetwork>> 
        where TAddress : struct, IComparable
        where TNetwork : struct, IComparable
    {
        TNetwork Network { get; }
        TAddress Type { get;  }

        byte[] Prefix { get; }

        AddressBase<TAddress, TNetwork> Create(string address);
        AddressBase<TAddress, TNetwork> Create(byte[] key, bool compressedPubKey = false);
    }
}

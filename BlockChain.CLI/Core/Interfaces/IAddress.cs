using System;
namespace BlockChain.CLI.Core.Interfaces
{
    public interface IAddress<TAddress, TNetwork> 
        where TAddress : struct, IComparable
        where TNetwork : struct, IComparable
    {
        IKey Key { get; }
        string Address { get; }
        INetworkVersion<TAddress, TNetwork> Version { get; }

        int AddressSize { get; }
        int ChecksumSize { get; }
    }
}

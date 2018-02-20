using System;
namespace BlockChain.CLI.Core
{
    public interface IAddress<TEnum> 
        where TEnum : struct, IComparable
    {
        IKey Key { get; }
        string Base58Check { get; }
        TEnum Type { get; }
    }
}

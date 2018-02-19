using System;
using BlockChain.CLI.Bitcoin;

namespace BlockChain.CLI.Core
{
    public delegate (bool IsValid, TEnum Type) VerificationDelegate<TEnum>(byte[] data)
        where TEnum : struct, IComparable, IFormattable, IConvertible;

    public delegate string CalculateDelegate<TEnum>(byte[] key, TEnum type)
        where TEnum : struct, IComparable, IFormattable, IConvertible;

    public delegate (byte[] Key, TEnum Type, byte[] Checksum) ExtractDelegate<TEnum>(byte[] data)
        where TEnum : struct, IComparable, IFormattable, IConvertible;

    public abstract class AddressBase<TEnum> : IAddress<TEnum>
        where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        public IKey Key { get; protected set; }
        public string Base58Check { get; protected set; }
        public TEnum Type { get; protected set; }

        protected static int AddressSize { get; set; } = 0;
        protected static int ChecksumSize { get; set; } = 4;

        protected static VerificationDelegate<TEnum> VerifyFunction { get; set; }
        protected static CalculateDelegate<TEnum> CalculateFunction { get; set; }
        internal static ExtractDelegate<TEnum> ExtractFunction { get; set; }

        public static (bool IsValid, TEnum Type) Verify(byte[] data) => VerifyFunction?.Invoke(data) ?? (false, default(TEnum));
        public static (bool IsValid, TEnum Type) Verify(string address) => VerifyFunction?.Invoke(address.DecodeBase58()) ?? (false, default(TEnum));

        protected static string Calculate(byte[] key, TEnum type) => CalculateFunction?.Invoke(key, type) ?? null;

        internal static (byte[] Key, TEnum Type, byte[] Checksum) Extract(byte[] data) => ExtractFunction?.Invoke(data) ?? (new byte[] { }, default(TEnum), new byte[] { });
        internal static (byte[] Key, TEnum Type, byte[] Checksum) Extract(string address) => ExtractFunction?.Invoke(address.DecodeBase58()) ?? (new byte[] { }, default(TEnum), new byte[] { });
    }
}

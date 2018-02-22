using System;
using System.Linq;
using BlockChain.CLI.Core;

namespace BlockChain.CLI.Bitcoin
{
    public struct NetworkVersion : IComparable, IComparable<NetworkVersion>, IEquatable<NetworkVersion>, IEquatable<AddressType>, IEquatable<NetworkType>
    {
        // From https://en.bitcoin.it/wiki/List_of_address_prefixes

        private static readonly byte[] UnknownPrefix = { 0xFF };
        private static readonly byte[] MainNetworkPubKey = { 0x00 };
        private static readonly byte[] MainNetworkScript = { 0x05 };
        private static readonly byte[] MainNetworkPrivKey = { 0x80 };
        private static readonly byte[] TestNetworkPubKey = { 0x6F };
        private static readonly byte[] TestNetworkScript = { 0xC4 };
        private static readonly byte[] TestNetworkPrivKey = { 0xEF };
        private static readonly byte[] MainNetworkBip32PubKey = { 0x04, 0x88, 0xB2, 0x1E };
        private static readonly byte[] MainNetworkBip32PrivKey = { 0x04, 0x88, 0xAD, 0xE4 };
        private static readonly byte[] TestNetworkBip32PubKey = { 0x04, 0x35, 0x87, 0xCF };
        private static readonly byte[] TestNetworkBip32PrivKey = { 0x04, 0x35, 0x83, 0x94 };
        private static readonly byte[] MainNetworkWitnessPubKey = { 0x06 };
        private static readonly byte[] MainNetworkWitnessScript = { 0x0A };
        private static readonly byte[] TestNetworkWitnessPubKey = { 0x03 };
        private static readonly byte[] TestNetworkWitnessScript = { 0x28 };

        public static readonly NetworkVersion Unknown = new NetworkVersion(AddressType.Unknown, NetworkType.Unknown);

        public readonly NetworkType Network;
        public readonly AddressType Type;

        public NetworkVersion(AddressType type, NetworkType network) : this()
        {
            this.Type = type;
            this.Network = network;
        }

        public byte[] Prefix
        {
            get
            {
                switch (Network)
                {
                    case NetworkType.Main:
                        switch (Type)
                        {
                            case AddressType.PublicKey:
                                return MainNetworkPubKey;
                            case AddressType.PrivateKey:
                                return MainNetworkPrivKey;
                            case AddressType.Script:
                                return MainNetworkScript;
                            case AddressType.Bip32PublicKey:
                                return MainNetworkBip32PubKey;
                            case AddressType.Bip32PrivateKey:
                                return MainNetworkBip32PrivKey;
                            case AddressType.WitnessPublicKey:
                                return MainNetworkWitnessPubKey;
                            case AddressType.WitnessScript:
                                return MainNetworkWitnessScript;
                            default:
                                return new byte[] { };
                        }
                    case NetworkType.Test:
                        switch (Type)
                        {
                            case AddressType.PublicKey:
                                return TestNetworkPubKey;
                            case AddressType.PrivateKey:
                                return TestNetworkPrivKey;
                            case AddressType.Script:
                                return TestNetworkScript;
                            case AddressType.Bip32PublicKey:
                                return TestNetworkBip32PubKey;
                            case AddressType.Bip32PrivateKey:
                                return TestNetworkBip32PrivKey;
                            case AddressType.WitnessPublicKey:
                                return TestNetworkWitnessPubKey;
                            case AddressType.WitnessScript:
                                return TestNetworkWitnessScript;
                            default:
                                return new byte[] { };
                        }
                    default:
                        return new byte[] { };
                }
            }
        }

        public AddressBase<NetworkVersion> Create(string address)
        {
            if (Network == NetworkType.Unknown)
            {
                throw new InvalidOperationException("Unknown network type");
            }

            switch (Type)
            {
                case AddressType.PublicKey:
                    return new PublicAddress(address);
                case AddressType.PrivateKey:
                    return new Wallet(address);
                case AddressType.Script:
                    return new Script(address);
                case AddressType.Bip32PublicKey:
                    return new PublicAddress(address);
                case AddressType.Bip32PrivateKey:
                    return new Wallet(address);
                case AddressType.WitnessPublicKey:
                    return new Witness(address);
                case AddressType.WitnessScript:
                    return new Witness(address);
                default:
                    throw new InvalidOperationException("Unknown address type");
            }
        }

        public AddressBase<NetworkVersion> Create(byte[] key, bool compressedPubKey = false)
        {
            if (Network == NetworkType.Unknown)
            {
                throw new InvalidOperationException("Unknown network type");
            }

            switch (Type)
            {
                case AddressType.PublicKey:
                    return new PublicAddress(key, this);
                case AddressType.PrivateKey:
                    return new Wallet(key, this, compressedPubKey);
                case AddressType.Script:
                    return new Script(key, this);
                case AddressType.Bip32PublicKey:
                    return new PublicAddress(key, this);
                case AddressType.Bip32PrivateKey:
                    return new Wallet(key, this, compressedPubKey);
                case AddressType.WitnessPublicKey:
                    return new Witness(key, this);
                case AddressType.WitnessScript:
                    return new Witness(key, this);
                default:
                    throw new InvalidOperationException("Unknown address type");
            }
        }

        public static AddressBase<NetworkVersion> CreateFromAddress(string address)
        {
            var version = Parse(address);

            return version.Create(address);
        }

        public static AddressBase<NetworkVersion> CreateFromAddress(byte[] data)
        {
            var version = Parse(data);

            return version.Create(data.EncodeBase58());
        }

        public static NetworkVersion Parse(string address)
        {
            return Parse(address.DecodeBase58());
        }

        public static NetworkVersion Parse(byte[] data)
        {
            byte headerByte = data[0];

            switch (headerByte)
            {
                case 0x00:
                    return new NetworkVersion(AddressType.PublicKey, NetworkType.Main);
                case 0x03:
                    return new NetworkVersion(AddressType.WitnessPublicKey, NetworkType.Test);
                case 0x04:
                    {
                        var header = data.SubArray(4);

                        if (header.SequenceEqual(MainNetworkBip32PubKey))
                            return new NetworkVersion(AddressType.Bip32PublicKey, NetworkType.Main);
                        if (header.SequenceEqual(MainNetworkBip32PrivKey))
                            return new NetworkVersion(AddressType.Bip32PrivateKey, NetworkType.Main);
                        if (header.SequenceEqual(TestNetworkBip32PubKey))
                            return new NetworkVersion(AddressType.Bip32PublicKey, NetworkType.Test);
                        if (header.SequenceEqual(TestNetworkBip32PrivKey))
                            return new NetworkVersion(AddressType.Bip32PrivateKey, NetworkType.Test);
                    }
                    return new NetworkVersion(AddressType.Unknown, NetworkType.Unknown);
                case 0x05:
                    return new NetworkVersion(AddressType.Script, NetworkType.Main);
                case 0x06:
                    return new NetworkVersion(AddressType.WitnessPublicKey, NetworkType.Main);
                case 0x0A:
                    return new NetworkVersion(AddressType.WitnessScript, NetworkType.Main);
                case 0x28:
                    return new NetworkVersion(AddressType.WitnessScript, NetworkType.Test);
                case 0x6F:
                    return new NetworkVersion(AddressType.PublicKey, NetworkType.Test);
                case 0x80:
                    return new NetworkVersion(AddressType.PrivateKey, NetworkType.Main);
                case 0xC4:
                    return new NetworkVersion(AddressType.Script, NetworkType.Test);
                case 0xEF:
                    return new NetworkVersion(AddressType.PrivateKey, NetworkType.Test);
                default:
                    return new NetworkVersion(AddressType.Unknown, NetworkType.Unknown);
            }
        }

        public override bool Equals(object obj)
        {
            var version = obj as NetworkVersion?;

            if (version.HasValue)
            {
                return this.Equals(version.Value);
            }
            else
            {
                return false;    
            }
        }

        public bool Equals(NetworkVersion other)
        {
            return this.Type == other.Type && this.Network == other.Network;
        }

        public bool Equals(AddressType other)
        {
            return this.Type == other;
        }

        public bool Equals(NetworkType other)
        {
            return this.Network == other;
        }

        public int CompareTo(NetworkVersion other)
        {
            return this.Type.CompareTo(other.Type) + this.Network.CompareTo(other.Network);
        }

        public int CompareTo(object obj)
        {
            var version = obj as NetworkVersion?;

            if (version.HasValue)
            {
                return this.CompareTo(version.Value);
            }
            else
            {
                return -1;
            } 
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ Network.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Address Type = {Type:G}, Network = {Network:G}]";
        }
    }
}

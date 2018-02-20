using System;
using System.Linq;
using System.Security.Cryptography;
using BlockChain.CLI.Core;

namespace BlockChain.CLI.Bitcoin
{
    public class PublicAddress
    {
        private const int CHECKSUM_SIZE = 4;
        private const int ADDRESS_SIZE = 20; // RIPEMD-160 hash size

        public PublicKey PublicKey { get; private set; }
        public string Base58Check { get; private set; }
        public (NetworkVersion.Type, NetworkVersion.Network) Type { get; private set; }

        public PublicAddress(string wifWallet)
        {
            (var publicKey, var type, _) = Extract(wifWallet);

            PublicKey = new PublicKey(publicKey);
            Type = type;
            Base58Check = wifWallet;
        }

        public PublicAddress(PublicKey publicKey, (NetworkVersion.Type, NetworkVersion.Network) type)
        {
            PublicKey = publicKey;
			Type = type;
            Base58Check = Calculate(publicKey.Key, type);
        }

        public PublicAddress(byte[] publicKey, (NetworkVersion.Type, NetworkVersion.Network) type)
            : this(new PublicKey(publicKey), type)
        { }

        public override string ToString()
        {
            return $"[Base58Check Address: {Base58Check}]";
        }

        public override int GetHashCode()
        {
            return Base58Check.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as PublicAddress;

            if (object.ReferenceEquals(null, other))
                return false;

            return other.PublicKey == this.PublicKey;
        }

        public static (bool, NetworkVersion.Network) Verify(string address)
        {
            var data = address.DecodeBase58();

            return Verify(data);
        }

        public static (bool, NetworkVersion.Network) Verify(byte[] data)
        {
            if ((data?.Length ?? 0) <= CHECKSUM_SIZE)
                return (false, NetworkVersion.Network.Unknown);
            
            var address = data.SubArray(0, data.Length - CHECKSUM_SIZE);
            var givenChecksum = data.SubArray(data.Length - CHECKSUM_SIZE);
            var type = data.GetNetworkVersion();
            var prefix = type.GetPrefix();

            var sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(address);
            var hash2 = sha256.ComputeHash(hash1);

            var correctChecksum = new byte[CHECKSUM_SIZE];
            Buffer.BlockCopy(hash2, 0, correctChecksum, 0, correctChecksum.Length);

            bool properSize = address.Length - prefix.Length == ADDRESS_SIZE;
            bool validChecksum = givenChecksum.SequenceEqual(correctChecksum);
            bool validType = type.type == NetworkVersion.Type.PublicKey;

            return (properSize && validChecksum && validType, type.network);
        }

        private static string Calculate(byte[] publicKey, (NetworkVersion.Type, NetworkVersion.Network) type)
        {
            var result = InternalCalculate(publicKey, type);
            return result.Item8;
        }

        internal static (byte[], byte[], byte[], byte[], byte[], byte[], byte[], string) InternalCalculate(byte[] publicKey, (NetworkVersion.Type, NetworkVersion.Network) type)
        {
            var sha256 = new SHA256Managed();
            var ripeMD160 = new RIPEMD160Managed();

            var publicKeySha = sha256.ComputeHash(publicKey);
            var publicKeyShaRipe = ripeMD160.ComputeHash(publicKeySha);
            var preHashNetwork = type.GetPrefix().Concat(publicKeyShaRipe);
            var publicHash = sha256.ComputeHash(preHashNetwork);
            var publicHash2x = sha256.ComputeHash(publicHash);

            var checksum = new byte[CHECKSUM_SIZE];
            Buffer.BlockCopy(publicHash2x, 0, checksum, 0, checksum.Length);

            var address = preHashNetwork.Concat(checksum);
            var base58Address = address.EncodeBase58();

            return (publicKeySha, publicKeyShaRipe, preHashNetwork, publicHash, publicHash2x, checksum, address, base58Address);
        }

        internal static (byte[], (NetworkVersion.Type, NetworkVersion.Network), byte[]) Extract(string wifWallet)
        {
            var wallet = wifWallet.DecodeBase58();
            var type = wallet.GetNetworkVersion();
            var checksumStart = wallet.Length - CHECKSUM_SIZE;
            var checksum = wallet.SubArray(checksumStart, CHECKSUM_SIZE);
            var privateKey = wallet.SubArray(1, checksumStart - 1);

            return (privateKey, type, checksum);
        }
    }
}

using System;
using System.Linq;
using System.Security.Cryptography;

namespace BlockChain.CLI.Bitcoin
{
    public class Address
    {
        private const int CHECKSUM_SIZE = 4;

        public byte[] PublicKey { get; private set; }
        public string Base58Check { get; private set; }

        public Address(byte[] publicKey, NetworkVersion.Type type = NetworkVersion.Type.MainNetworkPubKey)
        {
            PublicKey = publicKey;
            Base58Check = CalculateAddress(publicKey, type);
        }

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
            var other = obj as Address;

            if (object.ReferenceEquals(null, other))
                return false;

            return other.PublicKey.SequenceEqual(this.PublicKey);
        }

        public static (bool, NetworkVersion.Type) VerifyAddress(string address)
        {
            var data = address.DecodeBase58();

            return VerifyAddress(data);
        }

        public static (bool, NetworkVersion.Type) VerifyAddress(byte[] data)
        {
            var address = ArrayHelpers.SubArray(data, 0, data.Length - CHECKSUM_SIZE);
            var givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CHECKSUM_SIZE);

            var sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(address);
            var hash2 = sha256.ComputeHash(hash1);

            var correctCheckSum = new byte[CHECKSUM_SIZE];
            Buffer.BlockCopy(hash2, 0, correctCheckSum, 0, correctCheckSum.Length);

            return (givenCheckSum.SequenceEqual(correctCheckSum), data.GetNetworkType());
        }

        private static string CalculateAddress(byte[] publicKey, NetworkVersion.Type type)
        {
            var result = InternalCalculateAddress(publicKey, type);
            return result.Item8;
        }

        internal static (byte[], byte[], byte[], byte[], byte[], byte[], byte[], string) InternalCalculateAddress(byte[] publicKey, NetworkVersion.Type type)
        {
            var sha256 = new SHA256Managed();
            var ripeMD160 = new RIPEMD160Managed();

            var publicKeySha = sha256.ComputeHash(publicKey);
            var publicKeyShaRipe = ripeMD160.ComputeHash(publicKeySha);
            var preHashNetwork = ArrayHelpers.ConcatArrays(type.GetPrefix(), publicKeyShaRipe);
            var publicHash = sha256.ComputeHash(preHashNetwork);
            var publicHash2x = sha256.ComputeHash(publicHash);

            var checksum = new byte[CHECKSUM_SIZE];
            Buffer.BlockCopy(publicHash2x, 0, checksum, 0, checksum.Length);

            var address = ArrayHelpers.ConcatArrays(preHashNetwork, checksum);
            var base58Address = address.EncodeBase58();

            return (publicKeySha, publicKeyShaRipe, preHashNetwork, publicHash, publicHash2x, checksum, address, base58Address);
        }
    }
}

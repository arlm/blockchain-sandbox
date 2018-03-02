using System;
using System.Linq;
using System.Security.Cryptography;
using BlockChain.CLI.Bitcoin.Core;
using BlockChain.CLI.Core;

namespace BlockChain.CLI.Bitcoin.DeterministicWallet
{
    public sealed class PublicAddress : AddressBase<AddressType, NetworkType>
    {
        private const int CHECKSUM_SIZE = 4;

        private PublicAddress()
        {
            // RIPEMD-160 hash size
            AddressSize = 20;
            ChecksumSize = CHECKSUM_SIZE;
        }

        protected override (bool IsValid, NetworkVersion type) Verify(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return (false, NetworkVersion.Unknown);

            var data = address.DecodeBase58();

            if ((data?.Length ?? 0) <= CHECKSUM_SIZE)
                return (false, NetworkVersion.Unknown);

            var realAddress = data.SubArray(0, data.Length - CHECKSUM_SIZE);
            var givenChecksum = data.SubArray(data.Length - CHECKSUM_SIZE);
            var type = NetworkVersion.Parse(data);
            var prefix = type.Prefix;
            bool validChecksum = ValidateChecksum(realAddress, givenChecksum);

            bool properSize = realAddress.Length - prefix.Length == AddressSize;
            bool validType = type == AddressType.Bip32PublicKey;

            return (properSize && validChecksum && validType, type);
        }

        private static bool ValidateChecksum(byte[] realAddress, byte[] givenChecksum)
        {
            var sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(realAddress);
            var hash2 = sha256.ComputeHash(hash1);

            var correctChecksum = new byte[CHECKSUM_SIZE];
            Buffer.BlockCopy(hash2, 0, correctChecksum, 0, correctChecksum.Length);
            bool validChecksum = givenChecksum.SequenceEqual(correctChecksum);
            return validChecksum;
        }

        protected override (byte[] Key, NetworkVersion type, byte[] Checksum) Extract(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new InvalidOperationException("Address is not a valid Public Address");

            var data = address.DecodeBase58();
            var type = NetworkVersion.Parse(data);
            var prefix = type.Prefix;

            if (type != AddressType.Bip32PublicKey)
                throw new InvalidOperationException("Address is not a valid Public Address");

            var checksumStart = data.Length - CHECKSUM_SIZE;
            var checksum = data.SubArray(checksumStart, CHECKSUM_SIZE);

            var realAddress = data.SubArray(0, data.Length - CHECKSUM_SIZE);
            bool validChecksum = ValidateChecksum(realAddress, checksum);

            if (!validChecksum)
                throw new InvalidOperationException("Address is not a valid Public Address");

            bool properSize = realAddress.Length - prefix.Length == AddressSize;

            if (!properSize)
                throw new InvalidOperationException("Address is not a valid Public Address");
            
            var key = data.SubArray(1, checksumStart - 1);

            return (key, type, checksum);
        }

        protected override string Calculate(byte[] key, NetworkType type) => InternalCalculate(key, type).base58Address;

        public PublicAddress(string wifWallet)
            : this()
        {
            (var publicKey, var type, _) = Extract(wifWallet);

            Key = new PublicKey(publicKey);
            Version = type;
            Address = wifWallet;
        }

        public PublicAddress(PublicKey publicKey, NetworkType type)
            : this()
        {
            Key = publicKey;
            Version = new NetworkVersion(AddressType.Bip32PublicKey, type);
            Address = Calculate(publicKey.Data, type);
        }

        public PublicAddress(byte[] publicKey, NetworkType type)
            : this(new PublicKey(publicKey), type)
        { }

        public override string ToString()
        {
            return $"[Base58Check Address: {Address}]";
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as PublicAddress;

            if (object.ReferenceEquals(null, other))
                return false;

            return other.Key == this.Key;
        }

        internal static (byte[] publicKeySha, byte[] publicKeyShaRipe, byte[] preHashNetwork, byte[] publicHash, byte[] publicHash2x, byte[] checksum, byte[] address, string base58Address) InternalCalculate(byte[] publicKey, NetworkType type)
        {
            var sha256 = new SHA256Managed();
            var ripeMD160 = new RIPEMD160Managed();

            var publicKeySha = sha256.ComputeHash(publicKey);
            var publicKeyShaRipe = ripeMD160.ComputeHash(publicKeySha);
            var preHashNetwork = new NetworkVersion(AddressType.Bip32PublicKey, type).Prefix.Concat(publicKeyShaRipe);
            var publicHash = sha256.ComputeHash(preHashNetwork);
            var publicHash2x = sha256.ComputeHash(publicHash);

            var checksum = new byte[CHECKSUM_SIZE];
            Buffer.BlockCopy(publicHash2x, 0, checksum, 0, checksum.Length);

            var address = preHashNetwork.Concat(checksum);
            var base58Address = address.EncodeBase58();

            return (publicKeySha, publicKeyShaRipe, preHashNetwork, publicHash, publicHash2x, checksum, address, base58Address);
        }
    }
}

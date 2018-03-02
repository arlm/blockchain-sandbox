using System;
using System.Linq;
using System.Security.Cryptography;
using BlockChain.CLI.Bitcoin.Core;
using BlockChain.CLI.Core;

namespace BlockChain.CLI.Bitcoin.DeterministicWallet
{
    public sealed class PrivateAddress : AddressBase<AddressType, NetworkType>
    {
        private const int CHECKSUM_SIZE = 4;

        private static readonly byte[] COMPRESSED_PUB_KEY_SUFFIX = { 0x01 };

        public bool IsCompressed { get; private set; }

        private PrivateAddress()
        {
            // SHA-256 hash size
            AddressSize = 32;
            ChecksumSize = CHECKSUM_SIZE;
        }

        protected override (bool IsValid, NetworkVersion type) Verify(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return (false, NetworkVersion.Unknown);

            var data = address.DecodeBase58();

            if ((data?.Length ?? 0) <= CHECKSUM_SIZE)
                return (false, NetworkVersion.Unknown);

            var checksumStart = data.Length - CHECKSUM_SIZE;
            var realAddress = data.SubArray(0, checksumStart);
            var givenChecksum = data.SubArray(checksumStart);
            var type = NetworkVersion.Parse(data);
            var prefix = type.Prefix;
            bool validChecksum = ValidateChecksum(realAddress, givenChecksum);

            var privKeySize = realAddress.Length - prefix.Length;
            var endsWith0x01 = realAddress[checksumStart - 1] == COMPRESSED_PUB_KEY_SUFFIX[0];
            var expectedSize = endsWith0x01 ? privKeySize - 1 == AddressSize : privKeySize == AddressSize;

            var isCompressed = endsWith0x01 && expectedSize;
            var properSize = isCompressed ? privKeySize - 1 == AddressSize : privKeySize == AddressSize;
            var validType = type == AddressType.Bip32PrivateKey;

            return (properSize && validChecksum && validType, type);
        }

        private static bool ValidateChecksum(byte[] realAddress, byte[] givenChecksum)
        {
            var sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(realAddress);
            var hash2 = sha256.ComputeHash(hash1);

            var correctChecksum = new byte[CHECKSUM_SIZE];
            Buffer.BlockCopy(hash2, 0, correctChecksum, 0, correctChecksum.Length);
            var validChecksum = givenChecksum.SequenceEqual(correctChecksum);
            return validChecksum;
        }

        protected override (byte[] Key, NetworkVersion type, byte[] Checksum) Extract(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new InvalidOperationException("Address is not a valid Private Address");

            var data = address.DecodeBase58();
            var type = NetworkVersion.Parse(data);
            var prefix = type.Prefix;

            if (type != AddressType.Bip32PrivateKey)
                throw new InvalidOperationException("Address is not a valid Private Address");

            var checksumStart = data.Length - CHECKSUM_SIZE;
            var checksum = data.SubArray(checksumStart, CHECKSUM_SIZE);

            var endsWith0x01 = data[checksumStart - 1] == COMPRESSED_PUB_KEY_SUFFIX[0];
            int privKeySize = data.Length - prefix.Length - CHECKSUM_SIZE;
            var expectedSize = endsWith0x01 ? privKeySize - 1 == AddressSize : privKeySize == AddressSize;
            IsCompressed = endsWith0x01 && expectedSize;

            var realAddress = data.SubArray(0, data.Length - CHECKSUM_SIZE);
            bool validChecksum = ValidateChecksum(realAddress, checksum);

            if (!validChecksum)
                throw new InvalidOperationException("Address is not a valid Private Address");

            bool properSize = IsCompressed ? privKeySize - 1 == AddressSize : privKeySize == AddressSize;

            if (!properSize)
                throw new InvalidOperationException("Address is not a valid Private Address");
            
            var key = data.SubArray(1, checksumStart - (IsCompressed ? 2 : 1));

            return (key, type, checksum);
        }

        protected override string Calculate(byte[] key, NetworkType type) => InternalCalculate(key, type, false).base58Address;

        public PrivateAddress(string wifWallet)
            : this()
        {
            (var privateKey, var type, _) = Extract(wifWallet);

            Key = new PrivateKey(privateKey);
            Version = type;
            Address = wifWallet;
        }

        public PrivateAddress(PrivateKey privateKey, NetworkType type, bool compressedPubKey = false)
            : this()
        {
            if (compressedPubKey)
            {
                Key = privateKey;
            }
            else
            {
                Key = privateKey;
            }

            Version = new NetworkVersion(AddressType.Bip32PrivateKey, type);
            IsCompressed = compressedPubKey;
            Address = Calculate(privateKey.Data, type, compressedPubKey);
        }

        public PrivateAddress(byte[] privateKey, NetworkType type, bool compressedPubKey = false)
            : this(new PrivateKey(privateKey), type, compressedPubKey)
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
            var other = obj as PrivateAddress;

            if (object.ReferenceEquals(null, other))
                return false;

            return other.Key == this.Key;
        }

        private string Calculate(byte[] key, NetworkType type, bool compressedPubKey) => InternalCalculate(key, type, compressedPubKey).base58Address;

        internal static (byte[] preHashNetwork, byte[] publicHash, byte[] publicHash2x, byte[] checksum, byte[] wallet, string base58Address) InternalCalculate(byte[] privateKey, NetworkType type, bool compressedPubKey)
        {
            var sha256 = new SHA256Managed();
            byte[] preHashNetwork;

            var prefix = new NetworkVersion(AddressType.Bip32PrivateKey, type).Prefix;

            if (compressedPubKey)
            {
                preHashNetwork = ArrayHelpers.ConcatArrays(prefix, privateKey, COMPRESSED_PUB_KEY_SUFFIX);
            }
            else
            {
                preHashNetwork = prefix.Concat(privateKey);
            }

            var publicHash = sha256.ComputeHash(preHashNetwork);
            var publicHash2x = sha256.ComputeHash(publicHash);

            var checksum = new byte[CHECKSUM_SIZE];
            Buffer.BlockCopy(publicHash2x, 0, checksum, 0, checksum.Length);

            var wallet = preHashNetwork.Concat(checksum);
            var base58Address = wallet.EncodeBase58();

            return (preHashNetwork, publicHash, publicHash2x, checksum, wallet, base58Address);
        }
    }
}

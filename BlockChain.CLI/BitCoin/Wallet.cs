using System;
using System.Linq;
using System.Security.Cryptography;
using BlockChain.CLI.Core;

namespace BlockChain.CLI.Bitcoin
{
    public class Wallet : AddressBase<NetworkVersion>
    {
        private static readonly byte[] COMPRESSED_PUB_KEY_SUFFIX = { 0x01 };

        public bool IsCompressed { get; private set; }

        static Wallet()
        {
            // SHA-256 hash size
            AddressSize = 32;

            VerifyFunction = HandleVerificationDelegate;
            ExtractFunction = HandleExtractDelegate;
            CalculateFunction = HandleCalculateDelegate;
        }

        static (bool IsValid, NetworkVersion type) HandleVerificationDelegate(byte[] data)
        {
            if ((data?.Length ?? 0) <= ChecksumSize)
                return (false, NetworkVersion.Unknown);

            var checksumStart = data.Length - ChecksumSize;
            var wallet = data.SubArray(0, checksumStart);
            var givenChecksum = data.SubArray(checksumStart);
            var type = NetworkVersion.Parse(data);
            var prefix = type.Prefix;

            var sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(wallet);
            var hash2 = sha256.ComputeHash(hash1);

            var correctChecksum = new byte[ChecksumSize];
            Buffer.BlockCopy(hash2, 0, correctChecksum, 0, correctChecksum.Length);

            var privKeySize = wallet.Length - prefix.Length;
            var endsWith0x01 = wallet[checksumStart - 1] == COMPRESSED_PUB_KEY_SUFFIX[0];
            var expectedSize = endsWith0x01 ? privKeySize - 1 == AddressSize : privKeySize == AddressSize;

            var isCompressed = endsWith0x01 && expectedSize;
            var properSize = isCompressed ? privKeySize - 1 == AddressSize : privKeySize == AddressSize;
            var validChecksum = givenChecksum.SequenceEqual(correctChecksum);
            var validType = type.Type == AddressType.PrivateKey;

            return (properSize && validChecksum && validType, type);
        }

        static (byte[] Key, NetworkVersion type, byte[] Checksum) HandleExtractDelegate(byte[] data)
        {
            var type = NetworkVersion.Parse(data);
            var prefix = type.Prefix;

            var checksumStart = data.Length - ChecksumSize;
            var checksum = data.SubArray(checksumStart, ChecksumSize);

            var endsWith0x01 = data[checksumStart - 1] == COMPRESSED_PUB_KEY_SUFFIX[0];
            int privKeySize = data.Length - prefix.Length - ChecksumSize;
            var expectedSize = endsWith0x01 ? privKeySize - 1 == AddressSize : privKeySize == AddressSize;
            var isCompressed = endsWith0x01 && expectedSize;

            var privateKey = data.SubArray(1, checksumStart - (isCompressed ? 2 : 1));

            return (privateKey, type, checksum);
        }

        static string HandleCalculateDelegate(byte[] key, NetworkVersion type) => InternalCalculate(key, type, false).base58Address;

        public Wallet(string wifWallet)
        {
            // SHA-256 hash size
            AddressSize = 32;

            (var privateKey, var type, _) = Extract(wifWallet);

            Key = new PrivateKey(privateKey);
            Type = type;
            IsCompressed = false;
            Base58Check = wifWallet;
        }

        public Wallet(PrivateKey privateKey, NetworkVersion type, bool compressedPubKey = false)
        {
            if (compressedPubKey)
            {
                Key = privateKey;
            }
            else
            {
                Key = privateKey;
            }

            Type = type;
            IsCompressed = compressedPubKey;
            Base58Check = Calculate(privateKey.Key, type, compressedPubKey);
        }

        public Wallet(byte[] privateKey, NetworkVersion type, bool compressedPubKey = false)
            : this(new PrivateKey(privateKey), type, compressedPubKey)
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
            var other = obj as Wallet;

            if (object.ReferenceEquals(null, other))
                return false;

            return other.Key == this.Key;
        }

        protected static string Calculate(byte[] key, NetworkVersion type, bool compressedPubKey) => InternalCalculate(key, type, compressedPubKey).base58Address;

        internal static (byte[] preHashNetwork, byte[] publicHash, byte[] publicHash2x, byte[] checksum, byte[] wallet, string base58Address) InternalCalculate(byte[] privateKey, NetworkVersion type, bool compressedPubKey)
        {
            var sha256 = new SHA256Managed();
            byte[] preHashNetwork;

            if (compressedPubKey)
            {
                preHashNetwork = ArrayHelpers.ConcatArrays(type.Prefix, privateKey, COMPRESSED_PUB_KEY_SUFFIX);
            }
            else
            {
                preHashNetwork = type.Prefix.Concat(privateKey);
            }

            var publicHash = sha256.ComputeHash(preHashNetwork);
            var publicHash2x = sha256.ComputeHash(publicHash);

            var checksum = new byte[ChecksumSize];
            Buffer.BlockCopy(publicHash2x, 0, checksum, 0, checksum.Length);

            var wallet = preHashNetwork.Concat(checksum);
            var base58Address = wallet.EncodeBase58();

            return (preHashNetwork, publicHash, publicHash2x, checksum, wallet, base58Address);
        }
    }
}

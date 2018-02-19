using System;
using System.Linq;
using System.Security.Cryptography;
using BlockChain.CLI.Core;

namespace BlockChain.CLI.Bitcoin
{
    public class Wallet
    {
        private const int CHECKSUM_SIZE = 4;
        private const int WALLET_SIZE = 32; // SHA-256 hash size
        private static readonly byte[] COMPRESSED_PUB_KEY_SUFFIX = { 0x01 };

        public PrivateKey PrivateKey { get; private set; }
        public string Base58Check { get; private set; }
        public bool IsCompressed { get; private set; }
        public NetworkVersion.Type Type { get; private set; }

        public Wallet(string wifWallet)
        {
            (var privateKey, var type, _, var compressedPubKey) = Extract(wifWallet);

            PrivateKey = new PrivateKey(privateKey);
            Type = type;
            IsCompressed = compressedPubKey;
            Base58Check = wifWallet;
        }

        public Wallet(PrivateKey privateKey, NetworkVersion.Type type = NetworkVersion.Type.MainNetworkPrivKey, bool compressedPubKey = false)
        {
            if (compressedPubKey)
            {
                PrivateKey = privateKey;
            }
            else
            {
                PrivateKey = privateKey;
            }

			Type = type;
			IsCompressed = compressedPubKey;
            Base58Check = Calculate(privateKey.Key, type, compressedPubKey);
        }

        public Wallet(byte[] privateKey, NetworkVersion.Type type = NetworkVersion.Type.MainNetworkPrivKey, bool compressedPubKey = false)
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

            return other.PrivateKey == this.PrivateKey;
        }

        public static (bool, NetworkVersion.Type) Verify(string address)
        {
            var data = address.DecodeBase58();

            return Verify(data);
        }

        public static (bool, NetworkVersion.Type) Verify(byte[] data)
        {
            if ((data?.Length ?? 0) <= CHECKSUM_SIZE)
                return (false, NetworkVersion.Type.Unknown);
            
            var checksumStart = data.Length - CHECKSUM_SIZE;
            var wallet = data.SubArray(0, checksumStart);
            var givenChecksum = data.SubArray(checksumStart);
            var type = data.GetNetworkType();
            var prefix = type.GetPrefix();

            var sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(wallet);
            var hash2 = sha256.ComputeHash(hash1);

            var correctChecksum = new byte[CHECKSUM_SIZE];
            Buffer.BlockCopy(hash2, 0, correctChecksum, 0, correctChecksum.Length);

            var privKeySize = wallet.Length - prefix.Length;
            var endsWith0x01 = wallet[checksumStart - 1] == COMPRESSED_PUB_KEY_SUFFIX[0];
            var expectedSize = endsWith0x01 ? privKeySize - 1 == WALLET_SIZE : privKeySize == WALLET_SIZE;

            var isCompressed = endsWith0x01 && expectedSize;
            var properSize = isCompressed ? privKeySize  - 1 == WALLET_SIZE : privKeySize == WALLET_SIZE;
            var validChecksum = givenChecksum.SequenceEqual(correctChecksum);
            var validType = type == NetworkVersion.Type.MainNetworkPrivKey || type == NetworkVersion.Type.TestNetworkPrivKey;

            return (properSize && validChecksum && validType, type);
        }

        private static string Calculate(byte[] privateKey, NetworkVersion.Type type, bool compressedPubKey)
        {
            var result = InternalCalculate(privateKey, type, compressedPubKey);
            return result.Item6;
        }

        internal static (byte[], byte[], byte[], byte[], byte[], string) InternalCalculate(byte[] privateKey, NetworkVersion.Type type, bool compressedPubKey)
        {
            var sha256 = new SHA256Managed();
            byte[] preHashNetwork;

            if (compressedPubKey)
            {
                preHashNetwork = ArrayHelpers.ConcatArrays(type.GetPrefix(), privateKey, COMPRESSED_PUB_KEY_SUFFIX);
            }
            else
            {
				preHashNetwork = type.GetPrefix().Concat(privateKey);
            }

            var publicHash = sha256.ComputeHash(preHashNetwork);
            var publicHash2x = sha256.ComputeHash(publicHash);

            var checksum = new byte[CHECKSUM_SIZE];
            Buffer.BlockCopy(publicHash2x, 0, checksum, 0, checksum.Length);

            var wallet = preHashNetwork.Concat(checksum);
            var base58Address = wallet.EncodeBase58();

            return (preHashNetwork, publicHash, publicHash2x, checksum, wallet, base58Address);
        }

        internal static (byte[], NetworkVersion.Type, byte[], bool) Extract(string wifWallet)
        {
            var wallet = wifWallet.DecodeBase58();
            var type = wallet.GetNetworkType();
            var prefix = type.GetPrefix();

            var checksumStart = wallet.Length - CHECKSUM_SIZE;
            var checksum = wallet.SubArray(checksumStart, CHECKSUM_SIZE);

            var endsWith0x01 = wallet[checksumStart - 1] == COMPRESSED_PUB_KEY_SUFFIX[0];
            int privKeySize = wallet.Length - prefix.Length - CHECKSUM_SIZE;
            var expectedSize = endsWith0x01 ? privKeySize - 1 == WALLET_SIZE : privKeySize == WALLET_SIZE;
            var isCompressed = endsWith0x01 && expectedSize;

            var privateKey = wallet.SubArray(1, checksumStart - (isCompressed ? 2 : 1));

            return (privateKey, type, checksum, isCompressed);
        }
    }
}

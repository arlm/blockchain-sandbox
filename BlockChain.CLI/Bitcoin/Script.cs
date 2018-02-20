using System;
using System.Linq;
using System.Security.Cryptography;
using BlockChain.CLI.Core;

namespace BlockChain.CLI.Bitcoin
{
    public class Script : AddressBase<(NetworkVersion.Type, NetworkVersion.Network)>
    {
        static Script()
        {
			// RIPEMD-160 hash size
            AddressSize = 20;

            VerifyFunction = HandleVerificationDelegate;
            ExtractFunction = HandleExtractDelegate;
            CalculateFunction = HandleCalculateDelegate;
        }

        static (bool IsValid, (NetworkVersion.Type, NetworkVersion.Network) type) HandleVerificationDelegate(byte[] data)
        {
            if ((data?.Length ?? 0) <= ChecksumSize)
                return (false, (NetworkVersion.Type.Unknown, NetworkVersion.Network.Unknown));

            var address = data.SubArray(0, data.Length - ChecksumSize);
            var givenChecksum = data.SubArray(data.Length - ChecksumSize);
            var type = data.GetNetworkVersion();
            var prefix = type.GetPrefix();

            var sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(address);
            var hash2 = sha256.ComputeHash(hash1);

            var correctChecksum = new byte[ChecksumSize];
            Buffer.BlockCopy(hash2, 0, correctChecksum, 0, correctChecksum.Length);

            bool properSize = address.Length - prefix.Length == AddressSize;
            bool validChecksum = givenChecksum.SequenceEqual(correctChecksum);
            bool validType = type.type == NetworkVersion.Type.Script;

            return (properSize && validChecksum && validType, type);
        }

        static (byte[] Key, (NetworkVersion.Type, NetworkVersion.Network) type, byte[] Checksum) HandleExtractDelegate(byte[] data)
        {
            var type = data.GetNetworkVersion();
            var checksumStart = data.Length - ChecksumSize;
            var checksum = data.SubArray(checksumStart, ChecksumSize);
            var privateKey = data.SubArray(1, checksumStart - 1);

            return (privateKey, type, checksum);
        }

        static string HandleCalculateDelegate(byte[] key, (NetworkVersion.Type, NetworkVersion.Network) type)
        {
            var result = InternalCalculate(key, type);
            return result.base58Address;
        }

        public Script(string wifWallet)
        {
            (var publicKey, var type, _) = Extract(wifWallet);

            Key = new PublicKey(publicKey);
            Type = type;
            Base58Check = wifWallet;
        }

        public Script(PublicKey publicKey, (NetworkVersion.Type, NetworkVersion.Network) type)
        {
            Key = publicKey;
            Type = type;
            Base58Check = Calculate(publicKey.Key, type);
        }

        public Script(byte[] publicKey, (NetworkVersion.Type, NetworkVersion.Network) type)
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
            var other = obj as Script;

            if (object.ReferenceEquals(null, other))
                return false;

            return other.Key == this.Key;
        }

        internal static (byte[] publicKeySha, byte[] publicKeyShaRipe, byte[] preHashNetwork, byte[] publicHash, byte[] publicHash2x, byte[] checksum, byte[] address, string base58Address) InternalCalculate(byte[] publicKey, (NetworkVersion.Type, NetworkVersion.Network) type)
        {
            var sha256 = new SHA256Managed();
            var ripeMD160 = new RIPEMD160Managed();

            var publicKeySha = sha256.ComputeHash(publicKey);
            var publicKeyShaRipe = ripeMD160.ComputeHash(publicKeySha);
            var preHashNetwork = type.GetPrefix().Concat(publicKeyShaRipe);
            var publicHash = sha256.ComputeHash(preHashNetwork);
            var publicHash2x = sha256.ComputeHash(publicHash);

            var checksum = new byte[ChecksumSize];
            Buffer.BlockCopy(publicHash2x, 0, checksum, 0, checksum.Length);

            var address = preHashNetwork.Concat(checksum);
            var base58Address = address.EncodeBase58();

            return (publicKeySha, publicKeyShaRipe, preHashNetwork, publicHash, publicHash2x, checksum, address, base58Address);
        }
    }
}

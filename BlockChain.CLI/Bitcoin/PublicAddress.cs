using System;
using System.Linq;
using System.Security.Cryptography;
using BlockChain.CLI.Core;

namespace BlockChain.CLI.Bitcoin
{
    public class PublicAddress : AddressBase<NetworkVersion>
    {
        static PublicAddress()
        {
            // RIPEMD-160 hash size
            AddressSize = 20;

            VerifyFunction = HandleVerificationDelegate;
            ExtractFunction = HandleExtractDelegate;
            CalculateFunction = HandleCalculateDelegate;
        }

        static (bool IsValid, NetworkVersion type) HandleVerificationDelegate(byte[] data)
        {
            if ((data?.Length ?? 0) <= ChecksumSize)
                return (false, NetworkVersion.Unknown);

            var address = data.SubArray(0, data.Length - ChecksumSize);
            var givenChecksum = data.SubArray(data.Length - ChecksumSize);
            var type = NetworkVersion.Parse(data);
            var prefix = type.Prefix;

            var sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(address);
            var hash2 = sha256.ComputeHash(hash1);

            var correctChecksum = new byte[ChecksumSize];
            Buffer.BlockCopy(hash2, 0, correctChecksum, 0, correctChecksum.Length);

            bool properSize = address.Length - prefix.Length == AddressSize;
            bool validChecksum = givenChecksum.SequenceEqual(correctChecksum);
            bool validType = type.Type == AddressType.PublicKey;

            return (properSize && validChecksum && validType, type);
        }

        static (byte[] Key, NetworkVersion type, byte[] Checksum) HandleExtractDelegate(byte[] data)
        {
            var type = NetworkVersion.Parse(data);
            var checksumStart = data.Length - ChecksumSize;
            var checksum = data.SubArray(checksumStart, ChecksumSize);
            var privateKey = data.SubArray(1, checksumStart - 1);

            return (privateKey, type, checksum);
        }

        static string HandleCalculateDelegate(byte[] key, NetworkVersion type) => InternalCalculate(key, type).base58Address;

        public PublicAddress(string wifWallet)
        {
            // RIPEMD-160 hash size
            AddressSize = 20;

            (var publicKey, var type, _) = Extract(wifWallet);

            Key = new PublicKey(publicKey);
            Type = type;
            Base58Check = wifWallet;
        }

        public PublicAddress(PublicKey publicKey, NetworkVersion type)
        {
            Key = publicKey;
            Type = type;
            Base58Check = Calculate(publicKey.Key, type);
        }

        public PublicAddress(byte[] publicKey, NetworkVersion type)
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

            return other.Key == this.Key;
        }

        internal static (byte[] publicKeySha, byte[] publicKeyShaRipe, byte[] preHashNetwork, byte[] publicHash, byte[] publicHash2x, byte[] checksum, byte[] address, string base58Address) InternalCalculate(byte[] publicKey, NetworkVersion type)
        {
            var sha256 = new SHA256Managed();
            var ripeMD160 = new RIPEMD160Managed();

            var publicKeySha = sha256.ComputeHash(publicKey);
            var publicKeyShaRipe = ripeMD160.ComputeHash(publicKeySha);
            var preHashNetwork = type.Prefix.Concat(publicKeyShaRipe);
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

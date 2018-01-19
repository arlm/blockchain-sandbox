using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace BlockChain.Core
{
    public static class Extensions
    {
        public static int CalculateDifficulty(this byte[] hash)
        {
            int zeroes = 0;

            for (int index = 0; index < hash.Length; index++)
            {
                if ((hash[index] & 0xF0) != 0)
                {
                    break;
                }
                zeroes++;

                if ((hash[index] & 0x0F) != 0)
                {
                    break;
                }
                zeroes++;
            }

            return zeroes;
        }

        public static bool Validate(this IEnumerable<Block> chain, int? difficulty = null)
        {
            if (chain.First() != Chain.GenesisBlock)
            {
                return false;
            }

            using (var enumerator = chain.GetEnumerator())
            {
                var lastBlock = chain.First();

                while (enumerator.MoveNext())
                {
                    if (object.ReferenceEquals(enumerator.Current, lastBlock))
                    {
                        continue;
                    }

                    if (!Block.IsValidNewBlock(enumerator.Current, lastBlock, difficulty))
                    {
                        return false;
                    }

                    lastBlock = enumerator.Current;
                }
            }

            return true;
        }

        public static byte[] ToBytes(this string hexa)
        {
            if (string.IsNullOrWhiteSpace(hexa))
            {
                return new byte[] { };
            }

            if (hexa.Length % 2 != 0)
            {
                hexa = hexa.Insert(0, "0");
            }

            var result = new byte[hexa.Length / 2];

            for (int index = 0; index < hexa.Length; index += 2)
            {
                result[index / 2] = byte.Parse(hexa.Substring(index, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return result;
        }

        public static string Dump(this byte[] bytes) => string.Join(string.Empty, bytes.Select(b => b.ToString("X2")));

        public static bool IsEqualsTo(this byte[] bytes1, byte[] bytes2)
        {
            if (bytes1?.Length != bytes2?.Length)
            {
                return false;
            }

            for (int index = 0; index < (bytes1?.Length ?? 0); index++)
            {
                if (bytes1[index] != bytes2[index])
                {
                    return false;
                }
            }

            return true;
        }

        public static void TransformBlock(this HashAlgorithm hash, int value)
        {
            var bytes = BitConverter.GetBytes(value);

            hash.TransformBlock(bytes, 0, bytes.Length, null, 0);
        }

        public static void TransformBlock(this HashAlgorithm hash, uint value)
        {
            var bytes = BitConverter.GetBytes(value);

            hash.TransformBlock(bytes, 0, bytes.Length, null, 0);
        }

        public static void TransformBlock(this HashAlgorithm hash, long value)
        {
            var bytes = BitConverter.GetBytes(value);

            hash.TransformBlock(bytes, 0, bytes.Length, null, 0);
        }

        public static void TransformBlock(this HashAlgorithm hash, ulong value)
        {
            var bytes = BitConverter.GetBytes(value);

            hash.TransformBlock(bytes, 0, bytes.Length, null, 0);
        }

        public static void TransformBlock(this HashAlgorithm hash, byte[] value)
        {
            if (value != null)
            {
                hash.TransformBlock(value, 0, value.Length, null, 0);
            }
        }

        public static void TransformFinalBlock(this HashAlgorithm hash, int value)
        {
            var bytes = BitConverter.GetBytes(value);

            hash.TransformFinalBlock(bytes, 0, bytes.Length);
        }

        public static void TransformFinalBlock(this HashAlgorithm hash, uint value)
        {
            var bytes = BitConverter.GetBytes(value);

            hash.TransformFinalBlock(bytes, 0, bytes.Length);
        }

        public static void TransformFinalBlock(this HashAlgorithm hash, long value)
        {
            var bytes = BitConverter.GetBytes(value);

            hash.TransformFinalBlock(bytes, 0, bytes.Length);
        }

        public static void TransformFinalBlock(this HashAlgorithm hash, ulong value)
        {
            var bytes = BitConverter.GetBytes(value);

            hash.TransformFinalBlock(bytes, 0, bytes.Length);
        }

        public static void TransformFinalBlock(this HashAlgorithm hash, byte[] value)
        {
            if (value != null)
            {
                hash.TransformFinalBlock(value, 0, value.Length);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

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

        public static string Dump(this object obj)
        {
            var sb = new StringBuilder();

            // Include the type of the object
            var type = obj.GetType();
            sb.Append($"Type: {type.Name}");

            // Include information for each Field
            sb.Append("\r\n\r\nFields:");
            var fi = type.GetFields();

            if (fi.Length > 0)
            {
                foreach (FieldInfo f in fi)
                {
                    sb.Append($"\r\n {f} = {f.GetValue(obj) ?? "null"}");
                }
            }
            else
            {
                sb.Append("\r\n None");
            }

            // Include information for each Property
            sb.Append("\r\n\r\nProperties:");
            var pi = type.GetProperties();

            if (pi.Length > 0)
            {
                foreach (PropertyInfo p in pi)
                {
                    sb.Append($"\r\n {p} = {p.GetValue(obj, null) ?? "null"}");
                }
            }
            else
            {
                sb.Append("\r\n None");
            }

            return sb.ToString();
        }
    }
}

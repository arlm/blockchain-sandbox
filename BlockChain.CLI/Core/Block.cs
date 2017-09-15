using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlockChain.Core
{
    public class Block : IComparable, IComparable<Block>
    {
        public ulong Index { get; set; }
        public byte[] PreviousHash { get; set; }
        public DateTime TimeStamp { get; set; }
        public byte[] Data { get; set; }
        public byte[] Hash { get; set; }

        public static bool IsValidNewBlock(Block newBlock, Block previousBlock)
        {
            if (previousBlock.Index + 1 != newBlock.Index)
            {
                Debug.WriteLine("Invalid block index", nameof(Block));
                return false;
            }

            if (previousBlock.Hash != newBlock.PreviousHash)
            {
                Debug.WriteLine("Invalid previous hash", nameof(Block));
                return false;
            }

            var calculatedHash = newBlock.CalculateHash();

            if (calculatedHash != newBlock.Hash)
            {
                Debug.WriteLine($"Invalid hash: {calculatedHash} vs. {newBlock.Hash}", nameof(Block));
                return false;
            }

            return true;
        }

        public byte[] CalculateHash()
        {
            using (var sha256 = new SHA256Managed())
            {
                MemoryStream stream = null;

                try
                {
                    stream = new MemoryStream();

                    using (var writer = new StreamWriter(stream, Encoding.ASCII))
                    {
                        stream = null;

                        writer.Write(BitConverter.GetBytes(this.Index));
                        writer.Write(this.PreviousHash);
                        writer.Write(BitConverter.GetBytes(this.TimeStamp.ToBinary()));
                        writer.Write(this.Data);

                        writer.Flush();
                        var hash = sha256.ComputeHash(writer.BaseStream);
                        return hash;
                    }
                }
                finally
                {
                    stream?.Dispose();
                }
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Block);
        }

        public bool Equals(Block obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            return this.Index == obj.Index && this.PreviousHash == obj.PreviousHash &&
                       this.TimeStamp == obj.TimeStamp && this.Data == obj.Data &&
                       this.Hash == obj.Hash;
        }

        public override string ToString()
        {
            string previousHash = PreviousHash == null ? "NULL" : string.Join(string.Empty, PreviousHash.Select(b => b.ToString("X2")));
            string data = (Data?.Length ?? 0) == 0 ? "NULL" : Encoding.Unicode.GetString(Data);
            string hash = Hash == null ? "NULL" : string.Join(string.Empty, Hash.Select(b => b.ToString("X2")));

            return $"[Block: Index={Index}, PreviousHash={previousHash}, TimeStamp={TimeStamp}, Data=\"{data}\", Hash={hash}]";
        }

        public int CompareTo(object obj)
        {
            return (this.CompareTo(obj as Block));
        }

        public int CompareTo(Block other)
        {
            if (this.Equals(other))
            {
                return 0;
            }

            if (object.ReferenceEquals(null, other))
            { 
                return 1; 
            }

            return this.Index.CompareTo(other.Index);
        }

        public static bool operator ==(Block obj1, Block obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(Block obj1, Block obj2)
        {
            return !obj1.Equals(obj2);
        }
    }
}

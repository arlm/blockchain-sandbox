using System;
using System.Diagnostics;
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
        public uint Nonce { get; set; }

        public string UnicodeData => Encoding.Unicode.GetString(this.Data ?? new byte[] { });
        public int Difficulty => this.Hash?.CalculateDifficulty() ?? 0;

        public Block()
        { }

        public Block(string data) : this()
        {
            this.Data = Encoding.Unicode.GetBytes(data);
        }

        public static bool IsValidHashDifficulty(byte[] hash, int difficulty) => (hash?.CalculateDifficulty() ?? 0) >= difficulty;

        public bool IsGenesisBlock(Chain blockchain) => this == blockchain.FirstBlock.Value;

        public bool IsValidChild(Block child) => IsValidNewBlock(child, this);

        public Block GenerateChild(string data, DateTime? timestamp =  null, uint nonce = 0, int? difficulty = null) => GenerateChild(Encoding.Unicode.GetBytes(data), timestamp, nonce, difficulty);

        public Block GenerateChild(byte[] data, DateTime? timestamp = null, uint nonce = 0, int? difficulty = null)
        {
            var block = new Block
            {
                Index = this.Index + 1,
                TimeStamp = timestamp ?? DateTime.Now,
                PreviousHash = this.Hash,
                Data = data,
                Nonce = nonce
            };

            var nextHash = block.CalculateHash();

            if (!IsValidHashDifficulty(nextHash, difficulty ?? Chain.STANDARD_DIFFICULTY))
            {
                block.Nonce = 0;
                nextHash = block.CalculateHash();

                while (!IsValidHashDifficulty(nextHash, difficulty ?? Chain.STANDARD_DIFFICULTY))
                {
                    Debug.WriteLine($"{block.Nonce}: {nextHash.Dump()}");
                    block.Nonce++;
                    nextHash = block.CalculateHash();
                }
            }

            Debug.WriteLine($"{block.Nonce}: {nextHash.Dump()}");
            block.Hash = nextHash;

            return block;
        }

        public static bool IsValidNewBlock(Block newBlock, Block previousBlock, int? difficulty = null)
        {
            if (previousBlock.Index + 1 != newBlock.Index)
            {
                Debug.WriteLine("Invalid block index", nameof(Block));
                return false;
            }

            if (!previousBlock.Hash.IsEqualsTo(newBlock.PreviousHash))
            {
                Debug.WriteLine("Invalid previous hash", nameof(Block));
                return false;
            }

            var calculatedHash = newBlock.CalculateHash();

            if (!calculatedHash.IsEqualsTo(newBlock.Hash))
            {
                Debug.WriteLine($"Invalid hash: {calculatedHash.Dump()} vs. {newBlock.Hash.Dump()}", nameof(Block));
                return false;
            }

            if (difficulty.HasValue)
            {
                if (!IsValidHashDifficulty(calculatedHash, difficulty.Value))
                {
                    Debug.WriteLine($"Invalid hash does not meet difficulty requirements:: {calculatedHash.Dump()}", nameof(Block));
                    return false;
                }
            }

            return true;
        }

        public byte[] CalculateHash()
        {
            using (var sha256 = new SHA256Managed())
            {
                sha256.Initialize();
                sha256.TransformBlock(this.Index);
                sha256.TransformBlock(this.PreviousHash);
                sha256.TransformBlock(this.TimeStamp.ToBinary());
                sha256.TransformBlock(this.Data);
                sha256.TransformFinalBlock(this.Nonce);

                return sha256.Hash;
            }
        }

        /// <summary>
        /// Calculates a hash value that should be immutable for each object.
        /// We need it to ensure that lists and filters on product are working
        /// perfectly on the comparisons and sortings
        /// </summary>
        /// <remarks>
        /// We should compute the hash only using immutable fields and
        /// we must ensure that the hash core of the object will not change while
        /// the object is contained in a collection that relies on its hash core.
        /// 
        /// See the guidelines and rules for GetHashCode from Eric Lippert:
        /// https://blogs.msdn.microsoft.com/ericlippert/2011/02/28/guidelines-and-rules-for-gethashcode/
        /// </remarks>
        /// <returns>The object hashcode</returns>
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap
            unchecked
            // We are using the Fowler/Noll/Vo (FNV) hash algorithm to ensure uniqueness:
            // http://www.isthe.com/chongo/tech/comp/fnv/ and http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx#fnv
            //
            //FNV hashes are designed to be fast while maintaining a low collision rate.
            // The FNV speed allows one to quickly hash lots of data while maintaining a reasonable collision rate.
            // The high dispersion of the FNV hashes makes them well suited for hashing nearly identical strings such as URLs, hostnames, filenames, text, IP addresses, etc.
            {
                // An offset to use as starting point of this algorithm (uint 2166136261)
                int hash = -2128831035;

                // A prime number is used to shift the data and add dispersion
                // For 32-bit hashes it is 2^24 + 2^8 + 0x93 = 16777619
                hash = (hash * 16777619) ^ (Hash?.Aggregate((byte1, byte2) => unchecked((byte)(byte1.GetHashCode() ^ byte2.GetHashCode()))) ?? 0);

                return hash;
            }
        }

        public override bool Equals(object obj) => this.Equals(obj as Block);

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

            return this.Index == obj.Index && this.PreviousHash.IsEqualsTo(obj.PreviousHash) &&
                       this.TimeStamp == obj.TimeStamp && this.Data.IsEqualsTo(obj.Data) &&
                       this.Hash.IsEqualsTo(obj.Hash);
        }

        public override string ToString()
        {
            string previousHash = this.PreviousHash == null ? "NULL" : this.PreviousHash.Dump();
            string data = (this.Data?.Length ?? 0) == 0 ? "NULL" : Encoding.Unicode.GetString(this.Data);
            string hash = this.Hash == null ? "NULL" : this.Hash.Dump();

            return $"[Block: Index={this.Index}, PreviousHash={previousHash}, TimeStamp={this.TimeStamp} ({this.TimeStamp.ToBinary()}), Data=\"{data}\", Hash={hash}, Nonce={this.Nonce}]";
        }

        public int CompareTo(object obj) => (this.CompareTo(obj as Block));

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

        public static bool operator ==(Block obj1, Block obj2) => obj1.Equals(obj2);

        public static bool operator !=(Block obj1, Block obj2) => !obj1.Equals(obj2);
    }
}

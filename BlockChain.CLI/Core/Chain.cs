using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BlockChain.Core
{
    public sealed class Chain
    {
        internal const int STANDARD_DIFFICULTY = 4;

        public static readonly Block GenesisBlock = new Block
        {
            Index = 0,
            PreviousHash = { },
            TimeStamp = new DateTime(1465154705, DateTimeKind.Utc),
            Data = Encoding.Unicode.GetBytes("Welcome to μNode!"),
            Hash = new byte[] { 0x81, 0x65, 0x34, 0x93, 0x2c, 0x2b, 0x71, 0x54, 0x83, 0x6d, 0xa6, 0xaf, 0xc3, 0x67, 0x69, 0x5e, 0x63, 0x37, 0xdb, 0x8a, 0x92, 0x18, 0x23, 0x78, 0x4c, 0x14, 0x37, 0x8a, 0xbe, 0xd4, 0xf7, 0xd7 }
        };

        private readonly LinkedList<Block> blockchain = new LinkedList<Block>();

        public int Difficulty { get; private set; } = STANDARD_DIFFICULTY;

        public int Length => this.blockchain.Count;
        public LinkedListNode<Block> LastBlock => this.blockchain.Last;
        public LinkedListNode<Block> FirstBlock => this.blockchain.First;

        public Chain() => this.blockchain.AddFirst(GenesisBlock);

        public Chain(IEnumerable<Block> blockchain)
        {
            if ((blockchain?.Count() ?? 0) == 0)
            {
                this.blockchain.AddFirst(GenesisBlock);
            }
            else
            {
                foreach (var block in blockchain)
                {
                    this.blockchain.AddLast(block);
                }
            }
        }

        public Chain Add(Block newBlock)
        {
            if (Block.IsValidNewBlock(newBlock, LastBlock.Value, this.Difficulty))
            {
                this.blockchain.AddLast(newBlock);
                //broadcast(responseLatestMsg());
            }

            return this;
        }

        internal Block Mine(string seed, DateTime timestamp, uint nonce)
        {
            var block = this.GenerateNextBlock(seed, timestamp, nonce);
            Add(block);
            return block;
        }

        public Block Mine(string seed)
        {
            var block = this.GenerateNextBlock(seed);
            Add(block);
            return block;
        }

        public Block Mine(byte[] seed)
        {
            var block = this.GenerateNextBlock(seed);
            Add(block);
            return block;
        }

        public Block GenerateNextBlock(string data, DateTime? timestamp = null, uint nonce = 0, int? difficulty = null) => this.blockchain.Last.Value.GenerateChild(data, timestamp, nonce, difficulty);

        public Block GenerateNextBlock(byte[] data, DateTime? timestamp = null, uint nonce = 0, int? difficulty = null) => this.blockchain.Last.Value.GenerateChild(data, timestamp, nonce, difficulty);

        public Chain ReplaceChain(IEnumerable<Block> newBlocks)
        {
            if (newBlocks.Validate() && newBlocks.Count() > this.blockchain.Count)
            {
                Debug.WriteLine("Received blockchain is valid. Replacing current blockchain with received blockchain...", nameof(Chain));

                this.blockchain.Clear();

                foreach (var block in newBlocks)
                {
                    this.blockchain.AddLast(block);
                }

                // Broadcast(responseLatestMsg());
            }
            else
            {
                Debug.WriteLine("Received invalid blockchain!", nameof(Chain));
            }

            return this;
        }

        public bool Validate()
        {
            if (this.FirstBlock.Value != GenesisBlock)
            {
                return false;
            }

            var block = this.FirstBlock;

            while (block.Next != null)
            {
                if (!Block.IsValidNewBlock(block.Next.Value, block.Value, this.Difficulty))
                {
                    return false;
                }

                block = block.Next;
            }

            return true;
        }

        public string Dump()
        {
            var sb = new StringBuilder();

            foreach (var block in this.blockchain)
            {
                sb.AppendLine(block.ToString());
            }

            return sb.ToString();
        }
    }
}

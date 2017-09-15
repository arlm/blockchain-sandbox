using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BlockChain.Core
{
    public sealed class Chain
    {
        public static readonly Block GenesisBlock = new Block
        {
            Index = 0,
            PreviousHash = { },
            TimeStamp = new DateTime(1465154705, DateTimeKind.Utc),
            Data = Encoding.Unicode.GetBytes("Welcome to μNode!"),
            Hash = new byte[] { 0x81, 0x65, 0x34, 0x93, 0x2c, 0x2b, 0x71, 0x54, 0x83, 0x6d, 0xa6, 0xaf, 0xc3, 0x67, 0x69, 0x5e, 0x63, 0x37, 0xdb, 0x8a, 0x92, 0x18, 0x23, 0x78, 0x4c, 0x14, 0x37, 0x8a, 0xbe, 0xd4, 0xf7, 0xd7 }
        };

        private readonly LinkedList<Block> blockchain = new LinkedList<Block>();

        public LinkedListNode<Block> LastBlock => this.blockchain.Last;
        public LinkedListNode<Block> FirstBlock => this.blockchain.First;

        public Chain()
        {
            this.blockchain.AddFirst(GenesisBlock);
        }

        public Chain Add(Block newBlock)
        {
            if (Block.IsValidNewBlock(newBlock, LastBlock.Value))
            {
                this.blockchain.AddLast(newBlock);
                //broadcast(responseLatestMsg());
            }

            return this;
        }

        public Block GenerateNextBlock(string data)
        {
            var bytes = Encoding.Unicode.GetBytes(data);
            return GenerateNextBlock(bytes);
        }

        public Block GenerateNextBlock(byte[] data)
        {
            var last = this.blockchain.Last.Value;

            var block = new Block
            {
                Index = last.Index + 1,
                TimeStamp = DateTime.Now,
                PreviousHash = last.PreviousHash,
                Data = data
            };

            var nextHash = block.CalculateHash();
            block.Hash = nextHash;

            return block;
        }

        public Chain ReplaceChain(IEnumerable<Block> newBlocks)
        {
            if (newBlocks.Validate() && newBlocks.Count() > this.blockchain.Count)
            {
                Debug.WriteLine("Received blockchain is valid. Replacing current blockchain with received blockchain...", nameof(Chain));

                this.blockchain.Clear();

                foreach(var block in newBlocks)
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
                if (!Block.IsValidNewBlock(block.Next.Value, block.Value))
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

            foreach(var block in this.blockchain)
            {
                sb.AppendLine(block.ToString());    
            }

            return sb.ToString();
        }
    }
}

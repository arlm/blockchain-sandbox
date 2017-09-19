using System;
using BlockChain.Core;
using NUnit.Framework;

namespace BlockChain.Test
{
    [TestFixture]
    public class ChainTests
    {
        [Test]
        public void ValidateChain()
        {
            Assert.IsFalse((new Block[] { new Block() }).Validate());
            Assert.IsTrue((new Block[] { Chain.GenesisBlock }).Validate());
            Assert.IsTrue((new Chain()).Validate());
        }

        [Test]
        public void AddBlock()
        {
            var blockchain = new Chain();
            var block = new Block("xyz") { TimeStamp = DateTime.FromBinary(12) };
            blockchain.Add(block);
            Assert.AreEqual(1, blockchain.Length);

            block = new Block("xyz")
            {
                Index = 1,
                PreviousHash = blockchain.LastBlock.Value.Hash,
                TimeStamp = DateTime.FromBinary(14),
                Hash = "00003D3F5EE3E356DDD87D172C70990405B33FE1A1E2A9AB16A2E9500EAEBB59".ToBytes(),
                Nonce = 9750
            };
            blockchain.Add(block);
            Assert.AreEqual(2, blockchain.Length);
        }

        [Test]
        public void ReplaceChain()
        {
            var blockchain = new Chain();
            blockchain.ReplaceChain(new Block[] { Chain.GenesisBlock, new Block { TimeStamp = DateTime.FromBinary(12) } });
            Assert.AreEqual(1, blockchain.Length);
            Assert.AreEqual(Chain.GenesisBlock, blockchain.LastBlock.Value);

            var secondBlock = Chain.GenesisBlock.GenerateChild("x", timestamp: DateTime.FromBinary(14), nonce: 73913);
            var originalChain = new Block[] { Chain.GenesisBlock, secondBlock };
            blockchain = new Chain(originalChain);

            blockchain.ReplaceChain(new Block[] { Chain.GenesisBlock });
            Assert.AreEqual(2, blockchain.Length);
            Assert.AreEqual(Chain.GenesisBlock, blockchain.FirstBlock.Value);
            Assert.AreEqual(secondBlock, blockchain.LastBlock.Value);

            secondBlock = Chain.GenesisBlock.GenerateChild("x", timestamp: DateTime.FromBinary(14), nonce: 73913);
            var thirdBlock = secondBlock.GenerateChild("x", timestamp: DateTime.FromBinary(16), nonce: 45483);
            var fourthBlock = thirdBlock.GenerateChild("x", timestamp: DateTime.FromBinary(18), nonce: 51546);
            originalChain = new Block[] { Chain.GenesisBlock, secondBlock };
            blockchain = new Chain(originalChain);

            var replacementChain = new Block[] { Chain.GenesisBlock, secondBlock, thirdBlock, fourthBlock };
            blockchain.ReplaceChain(replacementChain);
            Assert.AreEqual(4, blockchain.Length);
            Assert.AreEqual(Chain.GenesisBlock, blockchain.FirstBlock.Value);
            Assert.AreEqual(fourthBlock, blockchain.LastBlock.Value);
        }

        [Test]
        public void LastBlock()
        {
            var firstBlock = new Block("a") { TimeStamp = DateTime.FromBinary(12) };
            var secondBlock = firstBlock.GenerateChild("b", timestamp: DateTime.FromBinary(14));
            var originalChain = new Block[] { firstBlock, secondBlock };
            var blockchain = new Chain(originalChain);

            Assert.AreEqual(2, blockchain.Length);
            Assert.AreEqual(firstBlock, blockchain.FirstBlock.Value);
            Assert.AreEqual(secondBlock, blockchain.LastBlock.Value);
        }

        [Test]
        public void MiningValidBlock()
        {
            var secondBlock = new Block { TimeStamp = DateTime.FromBinary(14) };
            var mined = new Block("z") {
                Index = 1,
                TimeStamp = DateTime.FromBinary(20),
                Hash = "000027E98F45AA5DC3DB8EB8517EBC08B732572210D9D70DD3428752B6F6BEAC".ToBytes(),
                Nonce = 169468
            };

            var blockchain = new Chain(new Block[] { new Block("x") { TimeStamp = DateTime.FromBinary(12) }, secondBlock });
            blockchain.Mine("z", DateTime.FromBinary(20), 169468);
            Assert.AreEqual(3, blockchain.Length);
            Assert.AreEqual(mined, blockchain.LastBlock.Value);
        }

        [Test]
        public void MiningInvalidBlock()
        {
            var secondBlock = new Block { TimeStamp = DateTime.FromBinary(42) };
            var blockchain = new Chain(new Block[] { new Block("x") { TimeStamp = DateTime.FromBinary(12) }, secondBlock });
            blockchain.Mine("a");
            Assert.AreEqual(2, blockchain.Length);
        }
    }
}

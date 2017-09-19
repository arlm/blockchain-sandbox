using System.Text;
using NUnit.Framework;
using BlockChain.Core;

namespace BlockChain.Test
{
    [TestFixture]
    public class BlockTests
    {
        [Test]
        public void IsGenesisBlock()
        {
            var genesis = Chain.GenesisBlock;
            Assert.IsTrue(genesis.IsGenesisBlock(new Chain()));

            var block = new Block();
            Assert.IsFalse(block.IsGenesisBlock(new Chain()));
        }

        [Test]
        public void GenerateChild()
        {
            var seed = "xyz";
            var block = new Block();
            var next = block.GenerateChild(seed);

            Assert.AreEqual(seed, next.UnicodeData);
            Assert.IsTrue(Encoding.Unicode.GetBytes(seed).IsEqualsTo(next.Data));
            Assert.AreEqual(block.Hash, next.PreviousHash);
            Assert.AreEqual(block.Index + 1, next.Index);
            Assert.AreNotEqual(0, next.Nonce);
        }

        [Test]
        public void TestHash()
        {
            var hash = "AAF41EFC26A0DBE903A8BD2880BA9FE97513796C74DF5298041D65B6E1413555".ToBytes();
            var block = new Block("some string")
            {
                Index = 47,
                PreviousHash = "d34db33f".ToBytes(),
                TimeStamp = 9876543210.FromJavascriptUTC(),
                Hash = hash
            };
            Assert.AreEqual(hash, block.Hash);

            block = new Block("some string")
            {
                Index = 47,
                PreviousHash = "d34db33f".ToBytes(),
                TimeStamp = 9876543210.FromJavascriptUTC(),
            };
            Assert.AreEqual(hash, block.CalculateHash());
        }

        [Test]
        public void IsValidChild()
        {
            var parent = new Block
            {
                Index = 48
            };

            var childHash = "69e69e1bed9bb8788505b26b05ad8386a91eebeb84a1edaff1ded24c09b97030".ToBytes();

            var child = new Block
            {
                Index = 47,
                PreviousHash = parent.Hash,
                Hash = childHash
            };

            Assert.IsFalse(parent.IsValidChild(child));

            parent = new Block
            {
                Index = 47
            };

            child = new Block
            {
                Index = 47,
                PreviousHash = parent.Hash,
                Hash = childHash
            };

            Assert.IsFalse(parent.IsValidChild(child));

            parent = new Block
            {
                Index = 46
            };

            child = new Block
            {
                Index = 47,
                PreviousHash = parent.Hash,
                Hash = "FE8C037B4FBBFE844BD035E9A85CC22120EFB4B2BEFA30B64F573793387D999A".ToBytes()
            };

            Assert.IsTrue(parent.IsValidChild(child));

            parent = new Block
            {
                Index = 46
            };

            childHash = "1cbab710e77d15d1e0f0b0162f941591958627c2a0321563938793b78a969fa9".ToBytes();

            child = new Block
            {
                Index = 47,
                PreviousHash = "1".ToBytes(),
                Hash = childHash
            };

            Assert.IsFalse(parent.IsValidChild(child));

            parent = new Block
            {
                Index = 46
            };

            childHash = "FE8C037B4FBBFE844BD035E9A85CC22120EFB4B2BEFA30B64F573793387D999A".ToBytes();

            child = new Block
            {
                Index = 47,
                PreviousHash = parent.Hash,
                Hash = childHash
            };

            Assert.IsTrue(parent.IsValidChild(child));
        }
    }
}

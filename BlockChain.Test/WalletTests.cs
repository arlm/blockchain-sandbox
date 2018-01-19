using System;
using BlockChain.CLI.Bitcoin;
using BlockChain.Core;
using NUnit.Framework;

namespace BlockChain.Test
{
    [TestFixture]
    public class WalletTests
    {
        [Test, Category("Bitcoin")]
        [TestCaseSource("BitcoinTestData")]
        public void Base58Encoding(string privateKey, NetworkVersion.Type type, string preHashNetwork,
                                   string publicHash, string publicHash2x, string checksum,
                                   string wallet, string base58Address)
        {
            (byte[] testPreHashNetwork, byte[] testPublicHash, byte[] testPublicHash2x, byte[] testChecksum,
             byte[] testWallet, string testBase58Wallet) = Wallet.InternalCalculate(privateKey.ToBytes(), type, false);

            Assert.AreEqual(preHashNetwork.ToBytes(), testPreHashNetwork, "Invalid Pre-Hash Network");
            Assert.AreEqual(publicHash.ToBytes(), testPublicHash, "Invalid Public Key Hash");
            Assert.AreEqual(publicHash2x.ToBytes(), testPublicHash2x, "Invalid Second Public Key Hash");
            Assert.AreEqual(checksum.ToBytes(), testChecksum, "Invalid Checksum");
            Assert.AreEqual(wallet.ToBytes(), testWallet, "Invalid Public Address");
            Assert.AreEqual(base58Address, testBase58Wallet, "Invalid Public Base58Check Address");

            var walletObj = new Wallet(privateKey.ToBytes(), type, false);
            Assert.AreEqual(privateKey.ToBytes(), walletObj.PrivateKey.Key);
            Assert.AreEqual(type, walletObj.Type);
            Assert.IsFalse(walletObj.CompressedPubKey);
            Assert.AreEqual(base58Address, walletObj.Base58Check);

            walletObj = new Wallet(base58Address);
            Assert.AreEqual(privateKey.ToBytes(), walletObj.PrivateKey.Key);
            Assert.AreEqual(type, walletObj.Type);
            Assert.IsFalse(walletObj.CompressedPubKey);
            Assert.AreEqual(privateKey.ToBytes(), walletObj.PrivateKey.Key);
            Assert.AreEqual(base58Address, walletObj.Base58Check);

            (bool checksumOk, NetworkVersion.Type testType) = Wallet.Verify(base58Address);
            Assert.IsTrue(checksumOk);
            Assert.AreEqual(type, testType);
        }

        [Test, Category("Bitcoin")]
        [TestCaseSource("BitcoinMiniTestData")]
        public void MiniPrivateKey(string privateKey, string miniKey, string base58Wallet, string address, bool valid)
        {
            if (valid)
            {
                Assert.IsTrue(miniKey.ValidateMiniPrivKey());
            }
            else
            {
                Assert.IsFalse(miniKey.ValidateMiniPrivKey());
            }

            var testKey = miniKey.ExpandMiniPrivKey();

            Assert.AreEqual(privateKey.ToBytes(), testKey.Key, "Invalid Expanded Private Key");

            var testWallet = new Wallet(testKey, NetworkVersion.Type.MainNetworkPrivKey);

            Assert.AreEqual(base58Wallet, testWallet.Base58Check, "Invalid Expanded Base58 Wallet");

            (bool checksumOk, NetworkVersion.Type testType) = Wallet.Verify(base58Wallet);
            Assert.IsTrue(checksumOk);
            Assert.AreEqual(NetworkVersion.Type.MainNetworkPrivKey, testType);
        }

        // Data From: https://github.com/ThePiachu/Bitcoin-Unit-Tests/blob/master/Address/Address%20Generation%20Test%201.txt
        private static object[] BitcoinTestData =
        {
            new object[] {"0C28FCA386C7A227600B2FE50B7CAE11EC86D3BF1FBE471BE89827E19D72AA1D",NetworkVersion.Type.MainNetworkPrivKey,"800C28FCA386C7A227600B2FE50B7CAE11EC86D3BF1FBE471BE89827E19D72AA1D","8147786C4D15106333BF278D71DADAF1079EF2D2440A4DDE37D747DED5403592","507A5B8DFED0FC6FE8801743720CEDEC06AA5C6FCA72B07C49964492FB98A714","507A5B8D","800C28FCA386C7A227600B2FE50B7CAE11EC86D3BF1FBE471BE89827E19D72AA1D507A5B8D","5HueCGU8rMjxEXxiPuD5BDku4MkFqeZyd4dZ1jvhTVqvbTLvyTJ" },
        };

        private static object[] BitcoinMiniTestData =
        {
            new object[] {"4C7A9640C72DC2099F23715D0C8A0D8A35F8906E3CAB61DD3F78B67BF887C9AB","S6c56bnXQiBjk9mqSYE7ykVQ7NzrRy","5JPy8Zg7z4P7RSLsiqcqyeAF1935zjNUdMxcDeVrtU1oarrgnB7","1CciesT23BNionJeXrbxmjc7ywfiyM4oLW", true },
        };
    }
}

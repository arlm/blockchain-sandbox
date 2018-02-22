using System;
using System.Text;
using BlockChain.CLI.Bitcoin;
using BlockChain.CLI.Core;
using BlockChain.Core;
using NUnit.Framework;

namespace BlockChain.Test
{
    [TestFixture]
    public class WalletTests
    {
        [Test, Category("Bitcoin")]
        [TestCaseSource("BitcoinTestData")]
        public void Base58Encoding(string privateKey, NetworkVersion type, string preHashNetwork,
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
            Assert.AreEqual(privateKey.ToBytes(), walletObj.Key.Key);
            Assert.AreEqual(type, walletObj.Type);
            Assert.IsFalse(walletObj.IsCompressed);
            Assert.AreEqual(base58Address, walletObj.Base58Check);

            walletObj = new Wallet(base58Address);
            Assert.AreEqual(privateKey.ToBytes(), walletObj.Key.Key);
            Assert.AreEqual(type, walletObj.Type);
            Assert.IsFalse(walletObj.IsCompressed);
            Assert.AreEqual(privateKey.ToBytes(), walletObj.Key.Key);
            Assert.AreEqual(base58Address, walletObj.Base58Check);

            (bool checksumOk, NetworkVersion testType) = Wallet.Verify(base58Address);
            Assert.IsTrue(checksumOk);
            Assert.AreEqual(type, testType);
        }

        // Test from: https://github.com/bitcoin/bitcoin/blob/master/src/test/base58_tests.cpp#L179-L204
        [Test, Category("Bitcoin")]
        [TestCaseSource("InvalidKeysData")]
        public void InvalidKeys(string key)
        {
            (var isAddress, var addressType) = PublicAddress.Verify(key);
            (var isWallet, var walletType) = Wallet.Verify(key);

            Assert.IsFalse(isAddress && (NetworkType.Unknown != addressType.Network), "This address should be invalid");
            Assert.IsFalse(isWallet && (NetworkType.Unknown != walletType.Network), "This wallet should be invalid");
        }

        // Test from: https://github.com/bitcoin/bitcoin/blob/master/src/test/base58_tests.cpp#L77-L136
        // Test from: https://github.com/bitcoin/bitcoin/blob/master/src/test/base58_tests.cpp#L139-L175
        [Test, Category("Bitcoin")]
        [TestCaseSource("ValidKeysData")]
        public void ValidKeys(string base58Check, string key, bool isCompressed, NetworkVersion type, bool tryCaseFlip)
        {
            byte[] bytes = key.ToBytes();

            var address = NetworkVersion.CreateFromAddress(base58Check);
            (var isValid, var detectedType) = address.Verify();
            Assert.IsTrue(isValid, $"{detectedType.Type:G} address should be valid");
            Assert.AreEqual(base58Check, address.Base58Check);

            switch (type.Type)
            {
                case AddressType.Bip32PrivateKey:
                case AddressType.PrivateKey:
                    {
                        var wallet = new Wallet(base58Check);
                        Assert.AreEqual(isCompressed, wallet.IsCompressed, "Compressed mismatch");
                        Assert.AreEqual(bytes.Length, wallet.Key.Key.Length, "Key length mismatch");
                        Assert.AreEqual(bytes, wallet.Key.Key, "Key content mismatch");

                        (isValid, detectedType) = PublicAddress.Verify(base58Check);
                        Assert.IsFalse(isValid, "Public Key should not be valid");
                    }
                    break;
                case AddressType.Bip32PublicKey:
                case AddressType.PublicKey:
                    {
                        if (tryCaseFlip)
                        {
                            var flipped = FlipCase(base58Check);

                            (var isValidFlippedAddress, var detectedTypeFlippedAddress) = PublicAddress.Verify(flipped);
                            isValid |= isValidFlippedAddress;
                        }

                        Assert.IsTrue(isValid, "Public Key should be valid");

                        (isValid, detectedType) = Wallet.Verify(base58Check);
                        Assert.IsFalse(isValid, "Private Key should not be valid");
                    }
                    break;
                case AddressType.Script:
                    {
                        (var isScriptValid, var detectedScriptType) = Script.Verify(base58Check);

                        if (tryCaseFlip)
                        {
                            var flipped = FlipCase(base58Check);

                            (var isValidFlippedScript, var detectedTypeFlippedScript) = Script.Verify(flipped);
                            isScriptValid |= isValidFlippedScript;
                        }

                        Assert.IsTrue(isScriptValid, "Public Key should be valid");

                        (isScriptValid, detectedScriptType) = Wallet.Verify(base58Check);
                        Assert.IsFalse(isScriptValid, "Private Key should not be valid");
                    }
                    break;
                case AddressType.WitnessScript:
                case AddressType.WitnessPublicKey:
                    {
                        (var isAddressValid, var detectedAddressType) = PublicAddress.Verify(base58Check);
                        (var isScriptValid, var detectedScriptType) = Script.Verify(base58Check);

                        if (tryCaseFlip)
                        {
                            var flipped = FlipCase(base58Check);

                            (var isValidFlippedAddress, var detectedTypeFlippedAddress) = PublicAddress.Verify(flipped);
                            (var isValidFlippedScript, var detectedTypeFlippedScript) = Script.Verify(flipped);
                            isAddressValid |= isValidFlippedAddress;
                            isScriptValid |= isValidFlippedScript;
                        }

                        Assert.IsTrue(isAddressValid || isScriptValid, "Public Key should be valid");

                        (isAddressValid, detectedAddressType) = Wallet.Verify(base58Check);
                        Assert.IsFalse(isAddressValid, "Private Key should not be valid");
                    }
                    break;
                default:
                    Assert.Fail("Unknown address type");
                    break;
            }
        }

        private string FlipCase(string base58Check)
        {
            var sb = new StringBuilder(base58Check.Length);

            foreach (var @char in base58Check)
            {
                if (@char >= 'a' && @char <= 'z')
                {
                    sb.Append((@char - 'a') + 'A');
                }
                else if (@char >= 'A' && @char <= 'Z')
                {
                    sb.Append((@char - 'A') + 'a');
                }
            }

            return sb.ToString();
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

            var testWallet = new Wallet(testKey, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main));

            Assert.AreEqual(base58Wallet, testWallet.Base58Check, "Invalid Expanded Base58 Wallet");

            (bool checksumOk, NetworkVersion testType) = Wallet.Verify(base58Wallet);
            Assert.IsTrue(checksumOk);
            Assert.AreEqual(NetworkType.Main, testType.Network);
        }

        private static object[] BitcoinTestData =
        {
            new object[] {"0C28FCA386C7A227600B2FE50B7CAE11EC86D3BF1FBE471BE89827E19D72AA1D", new NetworkVersion(AddressType.PrivateKey, NetworkType.Main),"800C28FCA386C7A227600B2FE50B7CAE11EC86D3BF1FBE471BE89827E19D72AA1D","8147786C4D15106333BF278D71DADAF1079EF2D2440A4DDE37D747DED5403592","507A5B8DFED0FC6FE8801743720CEDEC06AA5C6FCA72B07C49964492FB98A714","507A5B8D","800C28FCA386C7A227600B2FE50B7CAE11EC86D3BF1FBE471BE89827E19D72AA1D507A5B8D","5HueCGU8rMjxEXxiPuD5BDku4MkFqeZyd4dZ1jvhTVqvbTLvyTJ" },
        };

        // Data From: https://github.com/bitcoin/bitcoin/blob/master/src/test/data/base58_keys_valid.json
        private static object[] ValidKeysData =
        {
            new object[] {"1AGNa15ZQXAZUgFiqJ2i7Z2DPU2J6hW62i", "76a91465a16059864a2fdbc7c99a4723a8395bc6f188eb88ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"3CMNFxN1oHBc4R1EpboAL5yzHGgE611Xou", "a91474f209f6ea907e2ea48f74fae05782ae8a66525787", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"mo9ncXisMeAoXwqcV5EWuyncbmCcQN4rVs", "76a91453c0307d6851aa0ce7825ba883c6bd9ad242b48688ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"mo9ncXisMeAoXwqcV5EWuyncbmCcQN4rVs", "76a91453c0307d6851aa0ce7825ba883c6bd9ad242b48688ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFlip */ false},
            new object[] {"2N2JD6wb56AfK4tfmM6PwdVmoYk2dCKf4Br", "a9146349a418fc4578d10a372b54b45c280cc8c4382f87", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"5Kd3NBUAdUnhyzenEwVLy9pBKxSwXvE9FMPyR4UKZvpe6E3AgLr", "eddbdc1168f1daeadbd3e44c1e3f8f5a284c2029f78ad26af98583a499de5b19", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"Kz6UJmQACJmLtaQj5A3JAge4kVTNQ8gbvXuwbmCj7bsaabudb3RD", "55c9bccb9ed68446d1b75273bbce89d7fe013a8acd1625514420fb2aca1a21c4", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"9213qJab2HNEpMpYNBa7wHGFKKbkDn24jpANDs2huN3yi4J11ko", "36cb93b9ab1bdabf7fb9f2c04f1b9cc879933530ae7842398eef5a63a56800c2", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"9213qJab2HNEpMpYNBa7wHGFKKbkDn24jpANDs2huN3yi4J11ko", "36cb93b9ab1bdabf7fb9f2c04f1b9cc879933530ae7842398eef5a63a56800c2", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFlip */ false},
            new object[] {"cTpB4YiyKiBcPxnefsDpbnDxFDffjqJob8wGCEDXxgQ7zQoMXJdH", "b9f4892c9e8282028fea1d2667c4dc5213564d41fc5783896a0d843fc15089f3", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"cTpB4YiyKiBcPxnefsDpbnDxFDffjqJob8wGCEDXxgQ7zQoMXJdH", "b9f4892c9e8282028fea1d2667c4dc5213564d41fc5783896a0d843fc15089f3", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFlip */ false},
            new object[] {"1Ax4gZtb7gAit2TivwejZHYtNNLT18PUXJ", "76a9146d23156cbbdcc82a5a47eee4c2c7c583c18b6bf488ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"3QjYXhTkvuj8qPaXHTTWb5wjXhdsLAAWVy", "a914fcc5460dd6e2487c7d75b1963625da0e8f4c597587", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"n3ZddxzLvAY9o7184TB4c6FJasAybsw4HZ", "76a914f1d470f9b02370fdec2e6b708b08ac431bf7a5f788ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"2NBFNJTktNa7GZusGbDbGKRZTxdK9VVez3n", "a914c579342c2c4c9220205e2cdc285617040c924a0a87", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"5K494XZwps2bGyeL71pWid4noiSNA2cfCibrvRWqcHSptoFn7rc", "a326b95ebae30164217d7a7f57d72ab2b54e3be64928a19da0210b9568d4015e", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"L1RrrnXkcKut5DEMwtDthjwRcTTwED36thyL1DebVrKuwvohjMNi", "7d998b45c219a1e38e99e7cbd312ef67f77a455a9b50c730c27f02c6f730dfb4", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"93DVKyFYwSN6wEo3E2fCrFPUp17FtrtNi2Lf7n4G3garFb16CRj", "d6bca256b5abc5602ec2e1c121a08b0da2556587430bcf7e1898af2224885203", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"cTDVKtMGVYWTHCb1AFjmVbEbWjvKpKqKgMaR3QJxToMSQAhmCeTN", "a81ca4e8f90181ec4b61b6a7eb998af17b2cb04de8a03b504b9e34c4c61db7d9", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"1C5bSj1iEGUgSTbziymG7Cn18ENQuT36vv", "76a9147987ccaa53d02c8873487ef919677cd3db7a691288ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"3AnNxabYGoTxYiTEZwFEnerUoeFXK2Zoks", "a91463bcc565f9e68ee0189dd5cc67f1b0e5f02f45cb87", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"n3LnJXCqbPjghuVs8ph9CYsAe4Sh4j97wk", "76a914ef66444b5b17f14e8fae6e7e19b045a78c54fd7988ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"2NB72XtkjpnATMggui83aEtPawyyKvnbX2o", "a914c3e55fceceaa4391ed2a9677f4a4d34eacd021a087", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"5KaBW9vNtWNhc3ZEDyNCiXLPdVPHCikRxSBWwV9NrpLLa4LsXi9", "e75d936d56377f432f404aabb406601f892fd49da90eb6ac558a733c93b47252", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"L1axzbSyynNYA8mCAhzxkipKkfHtAXYF4YQnhSKcLV8YXA874fgT", "8248bd0375f2f75d7e274ae544fb920f51784480866b102384190b1addfbaa5c", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"927CnUkUbasYtDwYwVn2j8GdTuACNnKkjZ1rpZd2yBB1CLcnXpo", "44c4f6a096eac5238291a94cc24c01e3b19b8d8cef72874a079e00a242237a52", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"cUcfCMRjiQf85YMzzQEk9d1s5A4K7xL5SmBCLrezqXFuTVefyhY7", "d1de707020a9059d6d3abaf85e17967c6555151143db13dbb06db78df0f15c69", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"1Gqk4Tv79P91Cc1STQtU3s1W6277M2CVWu", "76a914adc1cc2081a27206fae25792f28bbc55b831549d88ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"33vt8ViH5jsr115AGkW6cEmEz9MpvJSwDk", "a914188f91a931947eddd7432d6e614387e32b24470987", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"mhaMcBxNh5cqXm4aTQ6EcVbKtfL6LGyK2H", "76a9141694f5bc1a7295b600f40018a618a6ea48eeb49888ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"2MxgPqX1iThW3oZVk9KoFcE5M4JpiETssVN", "a9143b9b3fd7a50d4f08d1a5b0f62f644fa7115ae2f387", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"5HtH6GdcwCJA4ggWEL1B3jzBBUB8HPiBi9SBc5h9i4Wk4PSeApR", "091035445ef105fa1bb125eccfb1882f3fe69592265956ade751fd095033d8d0", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"L2xSYmMeVo3Zek3ZTsv9xUrXVAmrWxJ8Ua4cw8pkfbQhcEFhkXT8", "ab2b4bcdfc91d34dee0ae2a8c6b6668dadaeb3a88b9859743156f462325187af", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"92xFEve1Z9N8Z641KQQS7ByCSb8kGjsDzw6fAmjHN1LZGKQXyMq", "b4204389cef18bbe2b353623cbf93e8678fbc92a475b664ae98ed594e6cf0856", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"92xFEve1Z9N8Z641KQQS7ByCSb8kGjsDzw6fAmjHN1LZGKQXyMq", "b4204389cef18bbe2b353623cbf93e8678fbc92a475b664ae98ed594e6cf0856", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFlip */ false},
            new object[] {"cVM65tdYu1YK37tNoAyGoJTR13VBYFva1vg9FLuPAsJijGvG6NEA", "e7b230133f1b5489843260236b06edca25f66adb1be455fbd38d4010d48faeef", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"1JwMWBVLtiqtscbaRHai4pqHokhFCbtoB4", "76a914c4c1b72491ede1eedaca00618407ee0b772cad0d88ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"3QCzvfL4ZRvmJFiWWBVwxfdaNBT8EtxB5y", "a914f6fe69bcb548a829cce4c57bf6fff8af3a5981f987", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"mizXiucXRCsEriQCHUkCqef9ph9qtPbZZ6", "76a914261f83568a098a8638844bd7aeca039d5f2352c088ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"2NEWDzHWwY5ZZp8CQWbB7ouNMLqCia6YRda", "a914e930e1834a4d234702773951d627cce82fbb5d2e87", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"5KQmDryMNDcisTzRp3zEq9e4awRmJrEVU1j5vFRTKpRNYPqYrMg", "d1fab7ab7385ad26872237f1eb9789aa25cc986bacc695e07ac571d6cdac8bc0", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"L39Fy7AC2Hhj95gh3Yb2AU5YHh1mQSAHgpNixvm27poizcJyLtUi", "b0bbede33ef254e8376aceb1510253fc3550efd0fcf84dcd0c9998b288f166b3", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"91cTVUcgydqyZLgaANpf1fvL55FH53QMm4BsnCADVNYuWuqdVys", "037f4192c630f399d9271e26c575269b1d15be553ea1a7217f0cb8513cef41cb", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"cQspfSzsgLeiJGB2u8vrAiWpCU4MxUT6JseWo2SjXy4Qbzn2fwDw", "6251e205e8ad508bab5596bee086ef16cd4b239e0cc0c5d7c4e6035441e7d5de", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"19dcawoKcZdQz365WpXWMhX6QCUpR9SY4r", "76a9145eadaf9bb7121f0f192561a5a62f5e5f5421029288ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"37Sp6Rv3y4kVd1nQ1JV5pfqXccHNyZm1x3", "a9143f210e7277c899c3a155cc1c90f4106cbddeec6e87", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"myoqcgYiehufrsnnkqdqbp69dddVDMopJu", "76a914c8a3c2a09a298592c3e180f02487cd91ba3400b588ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"2N7FuwuUuoTBrDFdrAZ9KxBmtqMLxce9i1C", "a91499b31df7c9068d1481b596578ddbb4d3bd90baeb87", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"5KL6zEaMtPRXZKo1bbMq7JDjjo1bJuQcsgL33je3oY8uSJCR5b4", "c7666842503db6dc6ea061f092cfb9c388448629a6fe868d068c42a488b478ae", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"KwV9KAfwbwt51veZWNscRTeZs9CKpojyu1MsPnaKTF5kz69H1UN2", "07f0803fc5399e773555ab1e8939907e9badacc17ca129e67a2f5f2ff84351dd", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"93N87D6uxSBzwXvpokpzg8FFmfQPmvX4xHoWQe3pLdYpbiwT5YV", "ea577acfb5d1d14d3b7b195c321566f12f87d2b77ea3a53f68df7ebf8604a801", /* isCompressed */  false, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"cMxXusSihaX58wpJ3tNuuUcZEQGt6DKJ1wEpxys88FFaQCYjku9h", "0b3b34f0958d8a268193a9814da92c3e8b58b4a4378a542863e34ac289cd830c", /* isCompressed */  true, new NetworkVersion(AddressType.PrivateKey, NetworkType.Test), /* tryCaseFllip*/ false},
            new object[] {"13p1ijLwsnrcuyqcTvJXkq2ASdXqcnEBLE", "76a9141ed467017f043e91ed4c44b4e8dd674db211c4e688ac", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"3ALJH9Y951VCGcVZYAdpA3KchoP9McEj1G", "a9145ece0cadddc415b1980f001785947120acdb36fc87", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */ false},
            new object[] {"bc1qw508d6qejxtdg4y5r3zarvary0c5xw7kv8f3t4", "0014751e76e8199196d454941c45d1b3a323f1433bd6", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */  true},
            new object[] {"bcrt1qw508d6qejxtdg4y5r3zarvary0c5xw7kygt080", "0014751e76e8199196d454941c45d1b3a323f1433bd6", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFlip */  true},
            new object[] {"tb1qrp33g0q5c5txsp9arysrx4k6zdkfs4nce4xj0gdcccefvpysxf3q0sl5k7", "00201863143c14c5166804bd19203356da136c985678cd4d27a1b8c6329604903262", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFlip */  true},
            new object[] {"bc1pw508d6qejxtdg4y5r3zarvary0c5xw7kw508d6qejxtdg4y5r3zarvary0c5xw7k7grplx", "5128751e76e8199196d454941c45d1b3a323f1433bd6751e76e8199196d454941c45d1b3a323f1433bd6", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */  true},
            new object[] {"bc1sw50qa3jx3s", "6002751e", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */  true},
            new object[] {"bc1zw508d6qejxtdg4y5r3zarvaryvg6kdaj", "5210751e76e8199196d454941c45d1b3a323", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Main), /* tryCaseFlip */  true},
            new object[] {"tb1qqqqqp399et2xygdj5xreqhjjvcmzhxw4aywxecjdzew6hylgvsesrxh6hy", "0020000000c4a5cad46221b2a187905e5266362b99d5e91c6ce24d165dab93e86433", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFlip */  true},
            new object[] {"bcrt1qqqqqp399et2xygdj5xreqhjjvcmzhxw4aywxecjdzew6hylgvseswlauz7", "0020000000c4a5cad46221b2a187905e5266362b99d5e91c6ce24d165dab93e86433", /* isCompressed */ false, new NetworkVersion(AddressType.PublicKey, NetworkType.Test), /* tryCaseFlip */  true}
        };

        // Data From: https://github.com/bitcoin/bitcoin/blob/master/src/test/data/base58_keys_invalid.json
        private static object[] InvalidKeysData =
        {
            string.Empty,
            "x",
            "37qgekLpCCHrQuSjvX3fs496FWTGsHFHizjJAs6NPcR47aefnnCWECAhHV6E3g4YN7u7Yuwod5Y",
            "dzb7VV1Ui55BARxv7ATxAtCUeJsANKovDGWFVgpTbhq9gvPqP3yv",
            "MuNu7ZAEDFiHthiunm7dPjwKqrVNCM3mAz6rP9zFveQu14YA8CxExSJTHcVP9DErn6u84E6Ej7S",
            "rPpQpYknyNQ5AEHuY6H8ijJJrYc2nDKKk9jjmKEXsWzyAQcFGpDLU2Zvsmoi8JLR7hAwoy3RQWf",
            "4Uc3FmN6NQ6zLBK5QQBXRBUREaaHwCZYsGCueHauuDmJpZKn6jkEskMB2Zi2CNgtb5r6epWEFfUJq",
            "7aQgR5DFQ25vyXmqZAWmnVCjL3PkBcdVkBUpjrjMTcghHx3E8wb",
            "17QpPprjeg69fW1DV8DcYYCKvWjYhXvWkov6MJ1iTTvMFj6weAqW7wybZeH57WTNxXVCRH4veVs",
            "KxuACDviz8Xvpn1xAh9MfopySZNuyajYMZWz16Dv2mHHryznWUp3",
            "7nK3GSmqdXJQtdohvGfJ7KsSmn3TmGqExug49583bDAL91pVSGq5xS9SHoAYL3Wv3ijKTit65th",
            "cTivdBmq7bay3RFGEBBuNfMh2P1pDCgRYN2Wbxmgwr4ki3jNUL2va",
            "gjMV4vjNjyMrna4fsAr8bWxAbwtmMUBXJS3zL4NJt5qjozpbQLmAfK1uA3CquSqsZQMpoD1g2nk",
            "emXm1naBMoVzPjbk7xpeTVMFy4oDEe25UmoyGgKEB1gGWsK8kRGs",
            "7VThQnNRj1o3Zyvc7XHPRrjDf8j2oivPTeDXnRPYWeYGE4pXeRJDZgf28ppti5hsHWXS2GSobdqyo",
            "1G9u6oCVCPh2o8m3t55ACiYvG1y5BHewUkDSdiQarDcYXXhFHYdzMdYfUAhfxn5vNZBwpgUNpso",
            "31QQ7ZMLkScDiB4VyZjuptr7AEc9j1SjstF7pRoLhHTGkW4Q2y9XELobQmhhWxeRvqcukGd1XCq",
            "DHqKSnpxa8ZdQyH8keAhvLTrfkyBMQxqngcQA5N8LQ9KVt25kmGN",
            "2LUHcJPbwLCy9GLH1qXmfmAwvadWw4bp4PCpDfduLqV17s6iDcy1imUwhQJhAoNoN1XNmweiJP4i",
            "7USRzBXAnmck8fX9HmW7RAb4qt92VFX6soCnts9s74wxm4gguVhtG5of8fZGbNPJA83irHVY6bCos",
            "1DGezo7BfVebZxAbNT3XGujdeHyNNBF3vnficYoTSp4PfK2QaML9bHzAMxke3wdKdHYWmsMTJVu",
            "2D12DqDZKwCxxkzs1ZATJWvgJGhQ4cFi3WrizQ5zLAyhN5HxuAJ1yMYaJp8GuYsTLLxTAz6otCfb",
            "8AFJzuTujXjw1Z6M3fWhQ1ujDW7zsV4ePeVjVo7D1egERqSW9nZ",
            "163Q17qLbTCue8YY3AvjpUhotuaodLm2uqMhpYirsKjVqnxJRWTEoywMVY3NbBAHuhAJ2cF9GAZ",
            "2MnmgiRH4eGLyLc9eAqStzk7dFgBjFtUCtu",
            "461QQ2sYWxU7H2PV4oBwJGNch8XVTYYbZxU",
            "2UCtv53VttmQYkVU4VMtXB31REvQg4ABzs41AEKZ8UcB7DAfVzdkV9JDErwGwyj5AUHLkmgZeobs",
            "cSNjAsnhgtiFMi6MtfvgscMB2Cbhn2v1FUYfviJ1CdjfidvmeW6mn",
            "gmsow2Y6EWAFDFE1CE4Hd3Tpu2BvfmBfG1SXsuRARbnt1WjkZnFh1qGTiptWWbjsq2Q6qvpgJVj",
            "nksUKSkzS76v8EsSgozXGMoQFiCoCHzCVajFKAXqzK5on9ZJYVHMD5CKwgmX3S3c7M1U3xabUny",
            "L3favK1UzFGgdzYBF2oBT5tbayCo4vtVBLJhg2iYuMeePxWG8SQc",
            "7VxLxGGtYT6N99GdEfi6xz56xdQ8nP2dG1CavuXx7Rf2PrvNMTBNevjkfgs9JmkcGm6EXpj8ipyPZ",
            "2mbZwFXF6cxShaCo2czTRB62WTx9LxhTtpP",
            "dB7cwYdcPSgiyAwKWL3JwCVwSk6epU2txw",
            "HPhFUhUAh8ZQQisH8QQWafAxtQYju3SFTX",
            "4ctAH6AkHzq5ioiM1m9T3E2hiYEev5mTsB",
            "Hn1uFi4dNexWrqARpjMqgT6cX1UsNPuV3cHdGg9ExyXw8HTKadbktRDtdeVmY3M1BxJStiL4vjJ",
            "Sq3fDbvutABmnAHHExJDgPLQn44KnNC7UsXuT7KZecpaYDMU9Txs",
            "6TqWyrqdgUEYDQU1aChMuFMMEimHX44qHFzCUgGfqxGgZNMUVWJ",
            "giqJo7oWqFxNKWyrgcBxAVHXnjJ1t6cGoEffce5Y1y7u649Noj5wJ4mmiUAKEVVrYAGg2KPB3Y4",
            "cNzHY5e8vcmM3QVJUcjCyiKMYfeYvyueq5qCMV3kqcySoLyGLYUK",
            "37uTe568EYc9WLoHEd9jXEvUiWbq5LFLscNyqvAzLU5vBArUJA6eydkLmnMwJDjkL5kXc2VK7ig",
            "EsYbG4tWWWY45G31nox838qNdzksbPySWc",
            "nbuzhfwMoNzA3PaFnyLcRxE9bTJPDkjZ6Rf6Y6o2ckXZfzZzXBT",
            "cQN9PoxZeCWK1x56xnz6QYAsvR11XAce3Ehp3gMUdfSQ53Y2mPzx",
            "1Gm3N3rkef6iMbx4voBzaxtXcmmiMTqZPhcuAepRzYUJQW4qRpEnHvMojzof42hjFRf8PE2jPde",
            "2TAq2tuN6x6m233bpT7yqdYQPELdTDJn1eU",
            "ntEtnnGhqPii4joABvBtSEJG6BxjT2tUZqE8PcVYgk3RHpgxgHDCQxNbLJf7ardf1dDk2oCQ7Cf",
            "Ky1YjoZNgQ196HJV3HpdkecfhRBmRZdMJk89Hi5KGfpfPwS2bUbfd",
            "2A1q1YsMZowabbvta7kTy2Fd6qN4r5ZCeG3qLpvZBMzCixMUdkN2Y4dHB1wPsZAeVXUGD83MfRED",
            "tc1qw508d6qejxtdg4y5r3zarvary0c5xw7kg3g4ty",
            "bc1qw508d6qejxtdg4y5r3zarvary0c5xw7kv8f3t5",
            "BC13W508D6QEJXTDG4Y5R3ZARVARY0C5XW7KN40WF2",
            "bc1rw5uspcuh",
            "bc10w508d6qejxtdg4y5r3zarvary0c5xw7kw508d6qejxtdg4y5r3zarvary0c5xw7kw5rljs90",
            "BC1QR508D6QEJXTDG4Y5R3ZARVARYV98GJ9P",
            "tb1qrp33g0q5c5txsp9arysrx4k6zdkfs4nce4xj0gdcccefvpysxf3q0sL5k7",
            "bc1zw508d6qejxtdg4y5r3zarvaryvqyzf3du",
            "tb1qrp33g0q5c5txsp9arysrx4k6zdkfs4nce4xj0gdcccefvpysxf3pjxtptv",
            "bc1gmk9yu"
        };

        private static object[] BitcoinMiniTestData =
        {
            new object[] { "4C7A9640C72DC2099F23715D0C8A0D8A35F8906E3CAB61DD3F78B67BF887C9AB", "S6c56bnXQiBjk9mqSYE7ykVQ7NzrRy", "5JPy8Zg7z4P7RSLsiqcqyeAF1935zjNUdMxcDeVrtU1oarrgnB7", "1CciesT23BNionJeXrbxmjc7ywfiyM4oLW", true },
        };
    }
}
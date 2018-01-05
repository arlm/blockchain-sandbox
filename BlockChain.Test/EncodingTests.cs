﻿using System;
using System.Text;
using BlockChain.CLI.BitCoin;
using NUnit.Framework;

namespace BlockChain.Test
{
    [TestFixture]
    public class EncodingTests
    {
        private Random rnd;

        [OneTimeSetUp]
        public void Steup() => rnd = new Random();

        [Test, Category("Base64")]
        [TestCaseSource("Base64TestData")]
        public void Base64Encoding(string plainText, string encodedText)
        {
            Assert.AreEqual(encodedText, Encoding.UTF8.EncodeBase64(plainText));
            Assert.AreEqual(plainText, Encoding.UTF8.DecodeBase64(encodedText));
        }

        [Test, Category("Base64")]
        public void Base64Encoding()
        {
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64("  "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64("   "));
            Assert.IsEmpty(Encoding.UTF8.EncodeBase64(null));
        }

        [Test, Category("Base64")]
        public void TestPowerOf2()
        {
            var orig1024 = new byte[1024];
            rnd.NextBytes(orig1024);

            var orig2048 = new byte[2048];
            rnd.NextBytes(orig2048);

            var orig4096 = new byte[4096];
            rnd.NextBytes(orig4096);

            var orig8192 = new byte[8192];
            rnd.NextBytes(orig8192);

            var enc1024 = orig1024.EncodeBase64();
            var enc2048 = orig2048.EncodeBase64();
            var enc4096 = orig4096.EncodeBase64();
            var enc8192 = orig8192.EncodeBase64();

            var dec1024 = enc1024.DecodeBase64();
            var dec2048 = enc2048.DecodeBase64();
            var dec4096 = enc4096.DecodeBase64();
            var dec8192 = enc8192.DecodeBase64();

            Assert.AreEqual(orig1024, dec1024);
            Assert.AreEqual(orig2048, dec2048);
            Assert.AreEqual(orig4096, dec4096);
            Assert.AreEqual(orig8192, dec8192);
        }

        [Test, Category("Base64")]
        public void TestPowerOf2Plus1()
        {
            var orig1025 = new byte[1025];
            rnd.NextBytes(orig1025);

            var orig2049 = new byte[2049];
            rnd.NextBytes(orig2049);

            var orig4097 = new byte[4097];
            rnd.NextBytes(orig4097);

            var orig8193 = new byte[8193];
            rnd.NextBytes(orig8193);

            var enc1025 = orig1025.EncodeBase64();
            var enc2049 = orig2049.EncodeBase64();
            var enc4097 = orig4097.EncodeBase64();
            var enc8193 = orig8193.EncodeBase64();

            var dec1025 = enc1025.DecodeBase64();
            var dec2049 = enc2049.DecodeBase64();
            var dec4097 = enc4097.DecodeBase64();
            var dec8193 = enc8193.DecodeBase64();

            Assert.AreEqual(orig1025, dec1025);
            Assert.AreEqual(orig2049, dec2049);
            Assert.AreEqual(orig4097, dec4097);
            Assert.AreEqual(orig8193, dec8193);
        }

        [Test, Category("Base58")]
        [TestCaseSource("Base58TestData_uint")]
        public void Base58Encoding(uint number, string encodedData)
        {
            Assert.AreEqual(encodedData, number.EncodeBase58());
            Assert.AreEqual(number, encodedData.DecodeBase58Uint());
        }

        [Test, Category("Base58")]
        [TestCaseSource("Base58TestData")]
        public void Base58Encoding(byte[] plainText, string encodedData)
        {
            //Assert.AreEqual(encodedData, plainText.EncodeBase58());
            Assert.AreEqual(plainText, encodedData.DecodeBase58());
        }

        private static object[] Base64TestData =
        {
            new [] {string.Empty, string.Empty},
            new [] {" ", "IA=="},
            new [] {"  ", "ICA="},
            new [] {"   ", "ICAg"},
            new [] {"test1...", "dGVzdDEuLi4="},
            new [] {"f", "Zg=="},
            new [] {"fo", "Zm8="},
            new [] {"foo", "Zm9v"},
            new [] {"foob", "Zm9vYg=="},
            new [] {"fooba", "Zm9vYmE="},
            new [] {"foobar", "Zm9vYmFy"}
        };

        private static object[] Base58TestData_uint =
        {
            new object[] {(uint)299542347, "TUEWE"}
        };

        // Data From: https://github.com/ThePiachu/Bitcoin-Unit-Tests/blob/master/Address/Address%20Generation%20Test%201.txt
        private static object[] Base58TestData =
        {
            new object[] {"00010966776006953D5567439E5E39F86A0D273BEED61967F6".ToBytes(), "16UwLL9Risc3QfPqBUvKofHmBQ7wMtjvM"},
            new object[] {"003A91CC0AF51BE125369A25FAC9CE5A950EF491AB06B2C8FB".ToBytes(), "16LgrHNVKbrySfS97wegnWWA5P8fb62FQN"},
            new object[] {"00714076A39428B9B904F4007DCD1519EF97B8784775C992F5".ToBytes(), "1BKpag8kykZNTxR2mw5qTEwUwmZX71c3JU"},
            new object[] {"00B9E57DF33679BCAABF4F7318082B703E4859AFCEDC276FCE".ToBytes(), "1HwvvsZjbAATRm5V1mw6i7g8sZ8gRqQQfX"},
            new object[] {"00D3A0742714A3BE8A2005D5698464A3FF4BAA94F5FD87AF18".ToBytes(), "1LHynznJ1kq4QcSUmCSgSo1HfUYP7UWcKZ"},
            new object[] {"0069325C54C6C8AB9D1E35DCEE0241E37A8820E5C709F1DBFB".ToBytes(), "1AbEHGF2iFLR1MdpRzMmRfVzpMF32cWwGe"},
            new object[] {"00EBC4E370B8B4908B8387A9442D7D9BA69C8B43B92D14F43A".ToBytes(), "1NVdf8Twtk8CivHQDwR3PZGsHhiemCy7ay"},
            new object[] {"00B845CC57712326B953FF26A7F20763025DE3C16ABC114C14".ToBytes(), "1HoLwxm944HpHWgEi2B2RYK6QN2icAaq9m"},
            new object[] {"00E026053C1AE60FEABC4BCFC80D89900573197A10C26D4C1C".ToBytes(), "1MSBugGhNwy3imagYRHQV1hnAqcfjPTUou"},
            new object[] {"00B58EAE755587554B2C52D0155DFA670DA90E0A3090485A28".ToBytes(), "1HYzEQSgoJT58XqjjpSVEwpahqhVdazRFd"},
        };
    }
}
using System;
using System.Text;
using BlockChain.CLI.Bitcoin.Core;
using BlockChain.Core;
using NUnit.Framework;

namespace BlockChain.Test
{
    [TestFixture]
    public class EncodingTests
    {
        private Random rnd;

        [OneTimeSetUp]
        public void Steup() => rnd = new Random();

        #region Base 64
        [Test, Category("Base64")]
        [TestCaseSource("Base64TestData")]
        public void Base64Encoding(string plainText, string encodedText)
        {
            Assert.AreEqual(encodedText, Encoding.ASCII.EncodeBase64(plainText));
            Assert.AreEqual(plainText, Encoding.ASCII.DecodeBase64(encodedText));
        }

        [Test, Category("Base64")]
        [TestCaseSource("Base64ByteTestData")]
        public void Base64Encoding(byte[] data, string encodedText)
        {
            Assert.AreEqual(encodedText, data.EncodeBase64());
            Assert.AreEqual(data, encodedText.DecodeBase64());
        }

        [Test, Category("Base64")]
        public void Base64SkippedChars()
        {
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("A"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("A^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("AA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("AA="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("AA==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("AA=x"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("AAA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("AAA^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("SGVsbG{\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9}8gV29ybGQ="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("SGVsbG8gV29ybGQ=SGVsbG8gV29ybGQ="));

            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("AB="));
            Assert.AreNotEqual("AB=", Encoding.UTF8.EncodeBase64("\u0000"));
            Assert.AreEqual("\u0000\u0000", Encoding.UTF8.DecodeBase64("AAB="));
            Assert.AreNotEqual("AAB=", Encoding.UTF8.EncodeBase64("\u0000"));

            Assert.AreEqual("Send reinforcements", Encoding.UTF8.DecodeBase64("U2VuZCByZWluZm9yY2VtZW50cw==\n"));
            Assert.AreEqual("U2VuZCByZWluZm9yY2VtZW50cw==", Encoding.UTF8.EncodeBase64("Send reinforcements"));
            Assert.AreEqual("Now is the time for all good coders\nto learn Ruby", Encoding.UTF8.DecodeBase64("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4g\nUnVieQ==\n"));
            Assert.AreEqual("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4gUnVieQ==", Encoding.UTF8.EncodeBase64("Now is the time for all good coders\nto learn Ruby"));
            Assert.AreEqual("This is line one\nThis is line two\nThis is line three\nAnd so on...\n", Encoding.UTF8.DecodeBase64("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGlu\nZSB0aHJlZQpBbmQgc28gb24uLi4K\n"));
            Assert.AreEqual("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGluZSB0aHJlZQpBbmQgc28gb24uLi4K", Encoding.UTF8.EncodeBase64("This is line one\nThis is line two\nThis is line three\nAnd so on...\n"));
            Assert.AreEqual("テスト", Encoding.UTF8.DecodeBase64("44OG44K544OI"));
            Assert.AreEqual("44OG44K544OI", Encoding.UTF8.EncodeBase64("テスト"));

            Assert.AreEqual(DECODED, ENCODED_64_CHARS_PER_LINE.DecodeBase64());
            Assert.AreEqual(DECODED, ENCODED_76_CHARS_PER_LINE.DecodeBase64());

            Assert.IsEmpty(Encoding.UTF8.DecodeBase64("\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("=\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("=="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("==\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("===\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("===="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64("====\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64("  "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64("   "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64("   \n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64("   \r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64("   \n\r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64("   \r\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64("   \t"));
            Assert.IsEmpty(Encoding.UTF8.EncodeBase64(null));
        }

        [Test, Category("Base64")]
        public void Base64PowerOf2()
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
        public void Base64PowerOf2Plus1()
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

        [Test, Category("Base64")]
        public void Base64EncodeDecodeRandom()
        {
            for (int i = 1; i < 5; i++)
            {
                var data = new byte[rnd.Next(10000) + 1];
                rnd.NextBytes(data);

                var enc = data.EncodeBase64();
                Assert.AreEqual(data, enc.DecodeBase64(), $"Error decoding {data.Dump()}");
            }
        }

        [Test, Category("Base64")]
        public void Base64EncodeDecodeSmall()
        {
            for (int i = 0; i < 12; i++)
            {
                var data = new byte[i];
                rnd.NextBytes(data);

                var enc = data.EncodeBase64();
                Assert.AreEqual(data, enc.DecodeBase64(), $"Error decoding {data.Dump()}");
            }
        }
        #endregion

        #region Base 64 URL
        [Test, Category("Base64")]
        [TestCaseSource("Base64UrlTestData")]
        public void Base64UrlEncoding(string plainText, string encodedText)
        {
            Assert.AreEqual(encodedText, Encoding.ASCII.EncodeBase64Url(plainText));
            Assert.AreEqual(plainText, Encoding.ASCII.DecodeBase64Url(encodedText));
        }

        [Test, Category("Base64")]
        [TestCaseSource("Base64UrlByteTestData")]
        public void Base64UrlEncoding(byte[] data, string encodedText)
        {
            Assert.AreEqual(encodedText, data.EncodeBase64Url());
            Assert.AreEqual(data, encodedText.DecodeBase64Url());
        }

        [Test, Category("Base64")]
        public void Base64UrlSkippedChars()
        {
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("A"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("A^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("AA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("AA="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("AA==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("AA=x"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("AAA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("AAA^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("SGVsbG{\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9}8gV29ybGQ="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("SGVsbG8gV29ybGQ=SGVsbG8gV29ybGQ="));

            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("AB="));
            Assert.AreNotEqual("AB=", Encoding.UTF8.EncodeBase64Url("\u0000"));
            Assert.AreEqual("\u0000\u0000", Encoding.UTF8.DecodeBase64Url("AAB="));
            Assert.AreNotEqual("AAB=", Encoding.UTF8.EncodeBase64Url("\u0000"));

            Assert.AreEqual("Send reinforcements", Encoding.UTF8.DecodeBase64Url("U2VuZCByZWluZm9yY2VtZW50cw==\n"));
            Assert.AreEqual("U2VuZCByZWluZm9yY2VtZW50cw==", Encoding.UTF8.EncodeBase64Url("Send reinforcements"));
            Assert.AreEqual("Now is the time for all good coders\nto learn Ruby", Encoding.UTF8.DecodeBase64Url("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4g\nUnVieQ==\n"));
            Assert.AreEqual("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4gUnVieQ==", Encoding.UTF8.EncodeBase64Url("Now is the time for all good coders\nto learn Ruby"));
            Assert.AreEqual("This is line one\nThis is line two\nThis is line three\nAnd so on...\n", Encoding.UTF8.DecodeBase64Url("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGlu\nZSB0aHJlZQpBbmQgc28gb24uLi4K\n"));
            Assert.AreEqual("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGluZSB0aHJlZQpBbmQgc28gb24uLi4K", Encoding.UTF8.EncodeBase64Url("This is line one\nThis is line two\nThis is line three\nAnd so on...\n"));
            Assert.AreEqual("テスト", Encoding.UTF8.DecodeBase64Url("44OG44K544OI"));
            Assert.AreEqual("44OG44K544OI", Encoding.UTF8.EncodeBase64Url("テスト"));

            Assert.AreEqual(DECODED, ENCODED_64_CHARS_PER_LINE.DecodeBase64Url());
            Assert.AreEqual(DECODED, ENCODED_76_CHARS_PER_LINE.DecodeBase64Url());

            Assert.IsEmpty(Encoding.UTF8.DecodeBase64Url("\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("=\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("=="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("==\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("===\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("===="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase64Url("====\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64Url(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64Url(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64Url("  "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64Url("   "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64Url("   \n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64Url("   \r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64Url("   \n\r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64Url("   \r\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase64Url("   \t"));
            Assert.IsEmpty(Encoding.UTF8.EncodeBase64Url(null));
        }

        [Test, Category("Base64")]
        public void Base64UrlPowerOf2()
        {
            var orig1024 = new byte[1024];
            rnd.NextBytes(orig1024);

            var orig2048 = new byte[2048];
            rnd.NextBytes(orig2048);

            var orig4096 = new byte[4096];
            rnd.NextBytes(orig4096);

            var orig8192 = new byte[8192];
            rnd.NextBytes(orig8192);

            var enc1024 = orig1024.EncodeBase64Url();
            var enc2048 = orig2048.EncodeBase64Url();
            var enc4096 = orig4096.EncodeBase64Url();
            var enc8192 = orig8192.EncodeBase64Url();

            var dec1024 = enc1024.DecodeBase64Url();
            var dec2048 = enc2048.DecodeBase64Url();
            var dec4096 = enc4096.DecodeBase64Url();
            var dec8192 = enc8192.DecodeBase64Url();

            Assert.AreEqual(orig1024, dec1024);
            Assert.AreEqual(orig2048, dec2048);
            Assert.AreEqual(orig4096, dec4096);
            Assert.AreEqual(orig8192, dec8192);
        }

        [Test, Category("Base64")]
        public void Base64UrlPowerOf2Plus1()
        {
            var orig1025 = new byte[1025];
            rnd.NextBytes(orig1025);

            var orig2049 = new byte[2049];
            rnd.NextBytes(orig2049);

            var orig4097 = new byte[4097];
            rnd.NextBytes(orig4097);

            var orig8193 = new byte[8193];
            rnd.NextBytes(orig8193);

            var enc1025 = orig1025.EncodeBase64Url();
            var enc2049 = orig2049.EncodeBase64Url();
            var enc4097 = orig4097.EncodeBase64Url();
            var enc8193 = orig8193.EncodeBase64Url();

            var dec1025 = enc1025.DecodeBase64Url();
            var dec2049 = enc2049.DecodeBase64Url();
            var dec4097 = enc4097.DecodeBase64Url();
            var dec8193 = enc8193.DecodeBase64Url();

            Assert.AreEqual(orig1025, dec1025);
            Assert.AreEqual(orig2049, dec2049);
            Assert.AreEqual(orig4097, dec4097);
            Assert.AreEqual(orig8193, dec8193);
        }

        [Test, Category("Base64")]
        public void Base64UrlEncodeDecodeRandom()
        {
            for (int i = 1; i < 5; i++)
            {
                var data = new byte[rnd.Next(10000) + 1];
                rnd.NextBytes(data);

                var enc = data.EncodeBase64Url();
                Assert.AreEqual(data, enc.DecodeBase64Url(), $"Error decoding {data.Dump()}");
            }
        }

        [Test, Category("Base64")]
        public void Base64UrlEncodeDecodeSmall()
        {
            for (int i = 0; i < 12; i++)
            {
                var data = new byte[i];
                rnd.NextBytes(data);

                var enc = data.EncodeBase64Url();
                Assert.AreEqual(data, enc.DecodeBase64Url(), $"Error decoding {data.Dump()}");
            }
        }
        #endregion

        #region Base 58
        [Test, Category("Base58")]
        [TestCaseSource("Base58TestData_uint")]
        public void Base58Encoding(uint number, string encodedData)
        {
            Assert.AreEqual(encodedData, number.EncodeBase58());
            Assert.AreEqual(number, encodedData.DecodeBase58Uint());
        }

        [Test, Category("Base58")]
        [TestCaseSource("Base58TestData")]
        public void Base58Encoding(byte[] data, string encodedText)
        {
            Assert.AreEqual(encodedText, data.EncodeBase58());
            Assert.AreEqual(data, encodedText.DecodeBase58());
        }

        [Test, Category("Base58")]
        [TestCaseSource("Base58DecodeData")]
        public void Base58Decoding(string encodedData, byte[] plainText)
        {
            Assert.AreEqual(plainText, encodedData.DecodeBase58());
        }

        [Test, Category("Base58")]
        public void Base58SkippedChars()
        {
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("A"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("A^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("AA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("AA="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("AA==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("AA=x"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("AAA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("AAA^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("SGVsbG{\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9}8gV29ybGQ="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("SGVsbG8gV29ybGQ=SGVsbG8gV29ybGQ="));

            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("AB="));
            Assert.AreNotEqual("AB=", Encoding.UTF8.EncodeBase58("\u0000"));
            Assert.AreEqual("\u0000\u0000", Encoding.UTF8.DecodeBase58("AAB="));
            Assert.AreNotEqual("AAB=", Encoding.UTF8.EncodeBase58("\u0000"));

            Assert.AreEqual("Send reinforcements", Encoding.UTF8.DecodeBase58("U2VuZCByZWluZm9yY2VtZW50cw==\n"));
            Assert.AreEqual("U2VuZCByZWluZm9yY2VtZW50cw==", Encoding.UTF8.EncodeBase58("Send reinforcements"));
            Assert.AreEqual("Now is the time for all good coders\nto learn Ruby", Encoding.UTF8.DecodeBase58("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4g\nUnVieQ==\n"));
            Assert.AreEqual("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4gUnVieQ==", Encoding.UTF8.EncodeBase58("Now is the time for all good coders\nto learn Ruby"));
            Assert.AreEqual("This is line one\nThis is line two\nThis is line three\nAnd so on...\n", Encoding.UTF8.DecodeBase58("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGlu\nZSB0aHJlZQpBbmQgc28gb24uLi4K\n"));
            Assert.AreEqual("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGluZSB0aHJlZQpBbmQgc28gb24uLi4K", Encoding.UTF8.EncodeBase58("This is line one\nThis is line two\nThis is line three\nAnd so on...\n"));
            Assert.AreEqual("テスト", Encoding.UTF8.DecodeBase58("44OG44K544OI"));
            Assert.AreEqual("44OG44K544OI", Encoding.UTF8.EncodeBase58("テスト"));

            Assert.AreEqual(DECODED, ENCODED_64_CHARS_PER_LINE.DecodeBase58());
            Assert.AreEqual(DECODED, ENCODED_76_CHARS_PER_LINE.DecodeBase58());

            Assert.IsEmpty(Encoding.UTF8.DecodeBase58("\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("=\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("=="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("==\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("===\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("===="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase58("====\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase58(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase58(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase58("  "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase58("   "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase58("   \n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase58("   \r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase58("   \n\r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase58("   \r\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase58("   \t"));
            Assert.IsEmpty(Encoding.UTF8.EncodeBase58(null));
        }

        [Test, Category("Base58")]
        public void Base58PowerOf2()
        {
            var orig1024 = new byte[1024];
            rnd.NextBytes(orig1024);

            var orig2048 = new byte[2048];
            rnd.NextBytes(orig2048);

            var orig4096 = new byte[4096];
            rnd.NextBytes(orig4096);

            var orig8192 = new byte[8192];
            rnd.NextBytes(orig8192);

            var enc1024 = orig1024.EncodeBase58();
            var enc2048 = orig2048.EncodeBase58();
            var enc4096 = orig4096.EncodeBase58();
            var enc8192 = orig8192.EncodeBase58();

            var dec1024 = enc1024.DecodeBase58();
            var dec2048 = enc2048.DecodeBase58();
            var dec4096 = enc4096.DecodeBase58();
            var dec8192 = enc8192.DecodeBase58();

            Assert.AreEqual(orig1024, dec1024);
            Assert.AreEqual(orig2048, dec2048);
            Assert.AreEqual(orig4096, dec4096);
            Assert.AreEqual(orig8192, dec8192);
        }

        [Test, Category("Base58")]
        public void Base58PowerOf2Plus1()
        {
            var orig1025 = new byte[1025];
            rnd.NextBytes(orig1025);

            var orig2049 = new byte[2049];
            rnd.NextBytes(orig2049);

            var orig4097 = new byte[4097];
            rnd.NextBytes(orig4097);

            var orig8193 = new byte[8193];
            rnd.NextBytes(orig8193);

            var enc1025 = orig1025.EncodeBase58();
            var enc2049 = orig2049.EncodeBase58();
            var enc4097 = orig4097.EncodeBase58();
            var enc8193 = orig8193.EncodeBase58();

            var dec1025 = enc1025.DecodeBase58();
            var dec2049 = enc2049.DecodeBase58();
            var dec4097 = enc4097.DecodeBase58();
            var dec8193 = enc8193.DecodeBase58();

            Assert.AreEqual(orig1025, dec1025);
            Assert.AreEqual(orig2049, dec2049);
            Assert.AreEqual(orig4097, dec4097);
            Assert.AreEqual(orig8193, dec8193);
        }

        [Test, Category("Base58")]
        public void Base58EncodeDecodeRandom()
        {
            for (int i = 1; i < 5; i++)
            {
                var data = new byte[rnd.Next(10000) + 1];
                rnd.NextBytes(data);

                var enc = data.EncodeBase58();
                Assert.AreEqual(data, enc.DecodeBase58(), $"Error decoding {data.Dump()}");
            }
        }

        [Test, Category("Base58")]
        public void Base58EncodeDecodeSmall()
        {
            for (int i = 0; i < 12; i++)
            {
                var data = new byte[i];
                rnd.NextBytes(data);

                var enc = data.EncodeBase58();
                Assert.AreEqual(data, enc.DecodeBase58(), $"Error decoding {data.Dump()}");
            }
        }
        #endregion

        #region Base 32
        [Test, Category("Base32")]
        [TestCaseSource("Base32TestData_uint")]
        public void Base32Encoding(uint number, string encodedData)
        {
            Assert.AreEqual(encodedData, number.EncodeBase32());
            Assert.AreEqual(number, encodedData.DecodeBase32Uint());
        }

        [Test, Category("Base32")]
        [TestCaseSource("Base32TestData")]
        public void Base32Encoding(string plainText, string encodedText)
        {
            Assert.AreEqual(encodedText, Encoding.UTF8.EncodeBase32(plainText));
            Assert.AreEqual(plainText, Encoding.UTF8.DecodeBase32(encodedText));
        }

        [Test, Category("Base32")]
        [TestCaseSource("Base32ByteTestData")]
        public void Base32Encoding(byte[] data, string encodedText)
        {
            Assert.AreEqual(encodedText, data.EncodeBase32());
            Assert.AreEqual(data, encodedText.DecodeBase32());
        }

        [Test, Category("Base32")]
        public void Base32SkippedChars()
        {
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("A"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("A^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("AA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("AA="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("AA==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("AA=x"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("AAA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("AAA^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("SGVsbG{\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9}8gV29ybGQ="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("SGVsbG8gV29ybGQ=SGVsbG8gV29ybGQ="));

            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("AB="));
            Assert.AreNotEqual("AB=", Encoding.UTF8.EncodeBase32("\u0000"));
            Assert.AreEqual("\u0000\u0000", Encoding.UTF8.DecodeBase32("AAB="));
            Assert.AreNotEqual("AAB=", Encoding.UTF8.EncodeBase32("\u0000"));

            Assert.AreEqual("Send reinforcements", Encoding.UTF8.DecodeBase32("U2VuZCByZWluZm9yY2VtZW50cw==\n"));
            Assert.AreEqual("U2VuZCByZWluZm9yY2VtZW50cw==", Encoding.UTF8.EncodeBase32("Send reinforcements"));
            Assert.AreEqual("Now is the time for all good coders\nto learn Ruby", Encoding.UTF8.DecodeBase32("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4g\nUnVieQ==\n"));
            Assert.AreEqual("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4gUnVieQ==", Encoding.UTF8.EncodeBase32("Now is the time for all good coders\nto learn Ruby"));
            Assert.AreEqual("This is line one\nThis is line two\nThis is line three\nAnd so on...\n", Encoding.UTF8.DecodeBase32("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGlu\nZSB0aHJlZQpBbmQgc28gb24uLi4K\n"));
            Assert.AreEqual("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGluZSB0aHJlZQpBbmQgc28gb24uLi4K", Encoding.UTF8.EncodeBase32("This is line one\nThis is line two\nThis is line three\nAnd so on...\n"));
            Assert.AreEqual("テスト", Encoding.UTF8.DecodeBase32("44OG44K544OI"));
            Assert.AreEqual("44OG44K544OI", Encoding.UTF8.EncodeBase32("テスト"));

            Assert.AreEqual(DECODED, ENCODED_64_CHARS_PER_LINE.DecodeBase32());
            Assert.AreEqual(DECODED, ENCODED_76_CHARS_PER_LINE.DecodeBase32());

            Assert.IsEmpty(Encoding.UTF8.DecodeBase32("\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("=\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("=="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("==\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("===\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("===="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32("====\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32("  "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32("   "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32("   \n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32("   \r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32("   \n\r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32("   \r\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32("   \t"));
            Assert.IsEmpty(Encoding.UTF8.EncodeBase32(null));
        }

        [Test, Category("Base32")]
        public void Base32PowerOf2()
        {
            var orig1024 = new byte[1024];
            rnd.NextBytes(orig1024);

            var orig2048 = new byte[2048];
            rnd.NextBytes(orig2048);

            var orig4096 = new byte[4096];
            rnd.NextBytes(orig4096);

            var orig8192 = new byte[8192];
            rnd.NextBytes(orig8192);

            var enc1024 = orig1024.EncodeBase32();
            var enc2048 = orig2048.EncodeBase32();
            var enc4096 = orig4096.EncodeBase32();
            var enc8192 = orig8192.EncodeBase32();

            var dec1024 = enc1024.DecodeBase32();
            var dec2048 = enc2048.DecodeBase32();
            var dec4096 = enc4096.DecodeBase32();
            var dec8192 = enc8192.DecodeBase32();

            Assert.AreEqual(orig1024, dec1024);
            Assert.AreEqual(orig2048, dec2048);
            Assert.AreEqual(orig4096, dec4096);
            Assert.AreEqual(orig8192, dec8192);
        }

        [Test, Category("Base32")]
        public void Base32PowerOf2Plus1()
        {
            var orig1025 = new byte[1025];
            rnd.NextBytes(orig1025);

            var orig2049 = new byte[2049];
            rnd.NextBytes(orig2049);

            var orig4097 = new byte[4097];
            rnd.NextBytes(orig4097);

            var orig8193 = new byte[8193];
            rnd.NextBytes(orig8193);

            var enc1025 = orig1025.EncodeBase32();
            var enc2049 = orig2049.EncodeBase32();
            var enc4097 = orig4097.EncodeBase32();
            var enc8193 = orig8193.EncodeBase32();

            var dec1025 = enc1025.DecodeBase32();
            var dec2049 = enc2049.DecodeBase32();
            var dec4097 = enc4097.DecodeBase32();
            var dec8193 = enc8193.DecodeBase32();

            Assert.AreEqual(orig1025, dec1025);
            Assert.AreEqual(orig2049, dec2049);
            Assert.AreEqual(orig4097, dec4097);
            Assert.AreEqual(orig8193, dec8193);
        }

        [Test, Category("Base32")]
        public void Base32EncodeDecodeRandom()
        {
            for (int i = 1; i < 5; i++)
            {
                var data = new byte[rnd.Next(10000) + 1];
                rnd.NextBytes(data);

                var enc = data.EncodeBase32();
                Assert.AreEqual(data, enc.DecodeBase32(), $"Error decoding {data.Dump()}");
            }
        }

        [Test, Category("Base32")]
        public void Base32EncodeDecodeSmall()
        {
            for (int i = 0; i < 12; i++)
            {
                var data = new byte[i];
                rnd.NextBytes(data);

                var enc = data.EncodeBase32();
                Assert.AreEqual(data, enc.DecodeBase32(), $"Error decoding {data.Dump()}");
            }
        }
        #endregion

        #region Base 32 Hexa
        [Test, Category("Base32")]
        [TestCaseSource("Base32HexaTestData_uint")]
        public void Base32HexaEncoding(uint number, string encodedData)
        {
            Assert.AreEqual(encodedData, number.EncodeBase32Hexa());
            Assert.AreEqual(number, encodedData.DecodeBase32HexaUint());
        }

        [Test, Category("Base32")]
        [TestCaseSource("Base32HexaTestData")]
        public void Base32HexaEncoding(string plainText, string encodedText)
        {
            Assert.AreEqual(encodedText, Encoding.UTF8.EncodeBase32Hexa(plainText));
            Assert.AreEqual(plainText, Encoding.UTF8.DecodeBase32Hexa(encodedText));
        }

        [Test, Category("Base32")]
        [TestCaseSource("Base32HexaByteTestData")]
        public void Base32HexaEncoding(byte[] data, string encodedText)
        {
            Assert.AreEqual(encodedText, data.EncodeBase32Hexa());
            Assert.AreEqual(data, encodedText.DecodeBase32Hexa());
        }

        [Test, Category("Base32")]
        public void Base32HexaSkippedChars()
        {
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("A"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("A^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("AA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("AA="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("AA==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("AA=x"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("AAA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("AAA^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("SGVsbG{\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9}8gV29ybGQ="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("SGVsbG8gV29ybGQ=SGVsbG8gV29ybGQ="));

            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("AB="));
            Assert.AreNotEqual("AB=", Encoding.UTF8.EncodeBase32Hexa("\u0000"));
            Assert.AreEqual("\u0000\u0000", Encoding.UTF8.DecodeBase32Hexa("AAB="));
            Assert.AreNotEqual("AAB=", Encoding.UTF8.EncodeBase32Hexa("\u0000"));

            Assert.AreEqual("Send reinforcements", Encoding.UTF8.DecodeBase32Hexa("U2VuZCByZWluZm9yY2VtZW50cw==\n"));
            Assert.AreEqual("U2VuZCByZWluZm9yY2VtZW50cw==", Encoding.UTF8.EncodeBase32Hexa("Send reinforcements"));
            Assert.AreEqual("Now is the time for all good coders\nto learn Ruby", Encoding.UTF8.DecodeBase32Hexa("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4g\nUnVieQ==\n"));
            Assert.AreEqual("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4gUnVieQ==", Encoding.UTF8.EncodeBase32Hexa("Now is the time for all good coders\nto learn Ruby"));
            Assert.AreEqual("This is line one\nThis is line two\nThis is line three\nAnd so on...\n", Encoding.UTF8.DecodeBase32Hexa("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGlu\nZSB0aHJlZQpBbmQgc28gb24uLi4K\n"));
            Assert.AreEqual("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGluZSB0aHJlZQpBbmQgc28gb24uLi4K", Encoding.UTF8.EncodeBase32Hexa("This is line one\nThis is line two\nThis is line three\nAnd so on...\n"));
            Assert.AreEqual("テスト", Encoding.UTF8.DecodeBase32Hexa("44OG44K544OI"));
            Assert.AreEqual("44OG44K544OI", Encoding.UTF8.EncodeBase32Hexa("テスト"));

            Assert.AreEqual(DECODED, ENCODED_64_CHARS_PER_LINE.DecodeBase32Hexa());
            Assert.AreEqual(DECODED, ENCODED_76_CHARS_PER_LINE.DecodeBase32Hexa());

            Assert.IsEmpty(Encoding.UTF8.DecodeBase32Hexa("\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("=\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("=="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("==\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("===\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("===="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase32Hexa("====\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32Hexa(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32Hexa(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32Hexa("  "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32Hexa("   "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32Hexa("   \n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32Hexa("   \r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32Hexa("   \n\r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32Hexa("   \r\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase32Hexa("   \t"));
            Assert.IsEmpty(Encoding.UTF8.EncodeBase32Hexa(null));
        }

        [Test, Category("Base32")]
        public void Base32HexaPowerOf2()
        {
            var orig1024 = new byte[1024];
            rnd.NextBytes(orig1024);

            var orig2048 = new byte[2048];
            rnd.NextBytes(orig2048);

            var orig4096 = new byte[4096];
            rnd.NextBytes(orig4096);

            var orig8192 = new byte[8192];
            rnd.NextBytes(orig8192);

            var enc1024 = orig1024.EncodeBase32Hexa();
            var enc2048 = orig2048.EncodeBase32Hexa();
            var enc4096 = orig4096.EncodeBase32Hexa();
            var enc8192 = orig8192.EncodeBase32Hexa();

            var dec1024 = enc1024.DecodeBase32Hexa();
            var dec2048 = enc2048.DecodeBase32Hexa();
            var dec4096 = enc4096.DecodeBase32Hexa();
            var dec8192 = enc8192.DecodeBase32Hexa();

            Assert.AreEqual(orig1024, dec1024);
            Assert.AreEqual(orig2048, dec2048);
            Assert.AreEqual(orig4096, dec4096);
            Assert.AreEqual(orig8192, dec8192);
        }

        [Test, Category("Base32")]
        public void Base32HexaPowerOf2Plus1()
        {
            var orig1025 = new byte[1025];
            rnd.NextBytes(orig1025);

            var orig2049 = new byte[2049];
            rnd.NextBytes(orig2049);

            var orig4097 = new byte[4097];
            rnd.NextBytes(orig4097);

            var orig8193 = new byte[8193];
            rnd.NextBytes(orig8193);

            var enc1025 = orig1025.EncodeBase32Hexa();
            var enc2049 = orig2049.EncodeBase32Hexa();
            var enc4097 = orig4097.EncodeBase32Hexa();
            var enc8193 = orig8193.EncodeBase32Hexa();

            var dec1025 = enc1025.DecodeBase32Hexa();
            var dec2049 = enc2049.DecodeBase32Hexa();
            var dec4097 = enc4097.DecodeBase32Hexa();
            var dec8193 = enc8193.DecodeBase32Hexa();

            Assert.AreEqual(orig1025, dec1025);
            Assert.AreEqual(orig2049, dec2049);
            Assert.AreEqual(orig4097, dec4097);
            Assert.AreEqual(orig8193, dec8193);
        }

        [Test, Category("Base32")]
        public void Base32HexaEncodeDecodeRandom()
        {
            for (int i = 1; i < 5; i++)
            {
                var data = new byte[rnd.Next(10000) + 1];
                rnd.NextBytes(data);

                var enc = data.EncodeBase32Hexa();
                Assert.AreEqual(data, enc.DecodeBase32Hexa(), $"Error decoding {data.Dump()}");
            }
        }

        [Test, Category("Base32")]
        public void Base32HexaEncodeDecodeSmall()
        {
            for (int i = 0; i < 12; i++)
            {
                var data = new byte[i];
                rnd.NextBytes(data);

                var enc = data.EncodeBase32Hexa();
                Assert.AreEqual(data, enc.DecodeBase32Hexa(), $"Error decoding {data.Dump()}");
            }
        }
        #endregion

        #region ZBase 32
        [Test, Category("ZBase32")]
        [TestCaseSource("ZBase32TestData_uint")]
      
        public void ZBase32Encoding(uint number, string encodedData)
        {
            Assert.AreEqual(encodedData, number.EncodeZBase32());
            Assert.AreEqual(number, encodedData.DecodeZBase32Uint());
        }

        [Test, Category("ZBase32")]
        [TestCaseSource("ZBase32TestData")]
        public void ZBase32Encoding(string plainText, string encodedText)
        {
            Assert.AreEqual(encodedText, Encoding.UTF8.EncodeZBase32(plainText));
            Assert.AreEqual(plainText, Encoding.UTF8.DecodeZBase32(encodedText));
        }

        [Test, Category("ZBase32")]
        [TestCaseSource("ZBase32ByteTestData")]
        public void ZBase32Encoding(byte[] data, string encodedText)
        {
            Assert.AreEqual(encodedText, data.EncodeZBase32());
            Assert.AreEqual(data, encodedText.DecodeZBase32());
        }

        [Test, Category("ZBase32")]
        public void ZBase32SkippedChars()
        {
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("A"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("A^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("AA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("AA="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("AA==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("AA=x"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("AAA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("AAA^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("SGVsbG{\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9}8gV29ybGQ="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("SGVsbG8gV29ybGQ=SGVsbG8gV29ybGQ="));

            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("AB="));
            Assert.AreNotEqual("AB=", Encoding.UTF8.EncodeZBase32("\u0000"));
            Assert.AreEqual("\u0000\u0000", Encoding.UTF8.DecodeZBase32("AAB="));
            Assert.AreNotEqual("AAB=", Encoding.UTF8.EncodeZBase32("\u0000"));

            Assert.AreEqual("Send reinforcements", Encoding.UTF8.DecodeZBase32("U2VuZCByZWluZm9yY2VtZW50cw==\n"));
            Assert.AreEqual("U2VuZCByZWluZm9yY2VtZW50cw==", Encoding.UTF8.EncodeZBase32("Send reinforcements"));
            Assert.AreEqual("Now is the time for all good coders\nto learn Ruby", Encoding.UTF8.DecodeZBase32("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4g\nUnVieQ==\n"));
            Assert.AreEqual("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4gUnVieQ==", Encoding.UTF8.EncodeZBase32("Now is the time for all good coders\nto learn Ruby"));
            Assert.AreEqual("This is line one\nThis is line two\nThis is line three\nAnd so on...\n", Encoding.UTF8.DecodeZBase32("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGlu\nZSB0aHJlZQpBbmQgc28gb24uLi4K\n"));
            Assert.AreEqual("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGluZSB0aHJlZQpBbmQgc28gb24uLi4K", Encoding.UTF8.EncodeZBase32("This is line one\nThis is line two\nThis is line three\nAnd so on...\n"));
            Assert.AreEqual("テスト", Encoding.UTF8.DecodeZBase32("44OG44K544OI"));
            Assert.AreEqual("44OG44K544OI", Encoding.UTF8.EncodeZBase32("テスト"));

            Assert.AreEqual(DECODED, ENCODED_64_CHARS_PER_LINE.DecodeZBase32());
            Assert.AreEqual(DECODED, ENCODED_76_CHARS_PER_LINE.DecodeZBase32());

            Assert.IsEmpty(Encoding.UTF8.DecodeZBase32("\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("=\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("=="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("==\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("===\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("===="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeZBase32("====\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeZBase32(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeZBase32(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeZBase32("  "));
            Assert.IsEmpty(Encoding.UTF8.DecodeZBase32("   "));
            Assert.IsEmpty(Encoding.UTF8.DecodeZBase32("   \n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeZBase32("   \r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeZBase32("   \n\r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeZBase32("   \r\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeZBase32("   \t"));
            Assert.IsEmpty(Encoding.UTF8.EncodeZBase32(null));
        }

        [Test, Category("ZBase32")]
        public void ZBase32PowerOf2()
        {
            var orig1024 = new byte[1024];
            rnd.NextBytes(orig1024);

            var orig2048 = new byte[2048];
            rnd.NextBytes(orig2048);

            var orig4096 = new byte[4096];
            rnd.NextBytes(orig4096);

            var orig8192 = new byte[8192];
            rnd.NextBytes(orig8192);

            var enc1024 = orig1024.EncodeZBase32();
            var enc2048 = orig2048.EncodeZBase32();
            var enc4096 = orig4096.EncodeZBase32();
            var enc8192 = orig8192.EncodeZBase32();

            var dec1024 = enc1024.DecodeZBase32();
            var dec2048 = enc2048.DecodeZBase32();
            var dec4096 = enc4096.DecodeZBase32();
            var dec8192 = enc8192.DecodeZBase32();

            Assert.AreEqual(orig1024, dec1024);
            Assert.AreEqual(orig2048, dec2048);
            Assert.AreEqual(orig4096, dec4096);
            Assert.AreEqual(orig8192, dec8192);
        }

        [Test, Category("ZBase32")]
        public void ZBase32PowerOf2Plus1()
        {
            var orig1025 = new byte[1025];
            rnd.NextBytes(orig1025);

            var orig2049 = new byte[2049];
            rnd.NextBytes(orig2049);

            var orig4097 = new byte[4097];
            rnd.NextBytes(orig4097);

            var orig8193 = new byte[8193];
            rnd.NextBytes(orig8193);

            var enc1025 = orig1025.EncodeZBase32();
            var enc2049 = orig2049.EncodeZBase32();
            var enc4097 = orig4097.EncodeZBase32();
            var enc8193 = orig8193.EncodeZBase32();

            var dec1025 = enc1025.DecodeZBase32();
            var dec2049 = enc2049.DecodeZBase32();
            var dec4097 = enc4097.DecodeZBase32();
            var dec8193 = enc8193.DecodeZBase32();

            Assert.AreEqual(orig1025, dec1025);
            Assert.AreEqual(orig2049, dec2049);
            Assert.AreEqual(orig4097, dec4097);
            Assert.AreEqual(orig8193, dec8193);
        }

        [Test, Category("ZBase32")]
        public void ZBase32EncodeDecodeRandom()
        {
            for (int i = 1; i < 5; i++)
            {
                var data = new byte[rnd.Next(10000) + 1];
                rnd.NextBytes(data);

                var enc = data.EncodeZBase32();
                Assert.AreEqual(data, enc.DecodeZBase32(), $"Error decoding {data.Dump()}");
            }
        }

        [Test, Category("ZBase32")]
        public void ZBase32EncodeDecodeSmall()
        {
            for (int i = 0; i < 12; i++)
            {
                var data = new byte[i];
                rnd.NextBytes(data);

                var enc = data.EncodeZBase32();
                Assert.AreEqual(data, enc.DecodeZBase32(), $"Error decoding {data.Dump()}");
            }
        }
        #endregion

        #region Base 16
        [Test, Category("Base16")]
        [TestCaseSource("Base16TestData_uint")]
        public void Base16Encoding(uint number, string encodedData)
        {
            Assert.AreEqual(encodedData, number.EncodeBase16());
            Assert.AreEqual(number, encodedData.DecodeBase16Uint());
        }

        [Test, Category("Base16")]
        [TestCaseSource("Base16TestData")]
        public void Base16Encoding(string plainText, string encodedText)
        {
            Assert.AreEqual(encodedText, Encoding.UTF8.EncodeBase16(plainText));
            Assert.AreEqual(plainText, Encoding.UTF8.DecodeBase16(encodedText));
        }

        [Test, Category("Base16")]
        public void Base16SkippedChars()
        {
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("A"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("A^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("AA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("AA="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("AA==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("AA=x"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("AAA"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("AAA^"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("SGVsbG{\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9}8gV29ybGQ="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("SGVsbG8gV29ybGQ=SGVsbG8gV29ybGQ="));

            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("AB="));
            Assert.AreNotEqual("AB=", Encoding.UTF8.EncodeBase16("\u0000"));
            Assert.AreEqual("\u0000\u0000", Encoding.UTF8.DecodeBase16("AAB="));
            Assert.AreNotEqual("AAB=", Encoding.UTF8.EncodeBase16("\u0000"));

            Assert.AreEqual("Send reinforcements", Encoding.UTF8.DecodeBase16("U2VuZCByZWluZm9yY2VtZW50cw==\n"));
            Assert.AreEqual("U2VuZCByZWluZm9yY2VtZW50cw==", Encoding.UTF8.EncodeBase16("Send reinforcements"));
            Assert.AreEqual("Now is the time for all good coders\nto learn Ruby", Encoding.UTF8.DecodeBase16("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4g\nUnVieQ==\n"));
            Assert.AreEqual("Tm93IGlzIHRoZSB0aW1lIGZvciBhbGwgZ29vZCBjb2RlcnMKdG8gbGVhcm4gUnVieQ==", Encoding.UTF8.EncodeBase16("Now is the time for all good coders\nto learn Ruby"));
            Assert.AreEqual("This is line one\nThis is line two\nThis is line three\nAnd so on...\n", Encoding.UTF8.DecodeBase16("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGlu\nZSB0aHJlZQpBbmQgc28gb24uLi4K\n"));
            Assert.AreEqual("VGhpcyBpcyBsaW5lIG9uZQpUaGlzIGlzIGxpbmUgdHdvClRoaXMgaXMgbGluZSB0aHJlZQpBbmQgc28gb24uLi4K", Encoding.UTF8.EncodeBase16("This is line one\nThis is line two\nThis is line three\nAnd so on...\n"));
            Assert.AreEqual("テスト", Encoding.UTF8.DecodeBase16("44OG44K544OI"));
            Assert.AreEqual("44OG44K544OI", Encoding.UTF8.EncodeBase16("テスト"));

            Assert.AreEqual(DECODED, ENCODED_64_CHARS_PER_LINE.DecodeBase16());
            Assert.AreEqual(DECODED, ENCODED_76_CHARS_PER_LINE.DecodeBase16());

            Assert.IsEmpty(Encoding.UTF8.DecodeBase16("\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("=\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("=="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("==\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("==="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("===\n"));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("===="));
            Assert.Catch<FormatException>(() => Encoding.UTF8.DecodeBase16("====\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase16(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase16(" "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase16("  "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase16("   "));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase16("   \n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase16("   \r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase16("   \n\r"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase16("   \r\n"));
            Assert.IsEmpty(Encoding.UTF8.DecodeBase16("   \t"));
            Assert.IsEmpty(Encoding.UTF8.EncodeBase16(null));
        }

        [Test, Category("Base16")]
        public void Base16PowerOf2()
        {
            var orig1024 = new byte[1024];
            rnd.NextBytes(orig1024);

            var orig2048 = new byte[2048];
            rnd.NextBytes(orig2048);

            var orig4096 = new byte[4096];
            rnd.NextBytes(orig4096);

            var orig8192 = new byte[8192];
            rnd.NextBytes(orig8192);

            var enc1024 = orig1024.EncodeBase16();
            var enc2048 = orig2048.EncodeBase16();
            var enc4096 = orig4096.EncodeBase16();
            var enc8192 = orig8192.EncodeBase16();

            var dec1024 = enc1024.DecodeBase16();
            var dec2048 = enc2048.DecodeBase16();
            var dec4096 = enc4096.DecodeBase16();
            var dec8192 = enc8192.DecodeBase16();

            Assert.AreEqual(orig1024, dec1024);
            Assert.AreEqual(orig2048, dec2048);
            Assert.AreEqual(orig4096, dec4096);
            Assert.AreEqual(orig8192, dec8192);
        }

        [Test, Category("Base16")]
        public void Base16PowerOf2Plus1()
        {
            var orig1025 = new byte[1025];
            rnd.NextBytes(orig1025);

            var orig2049 = new byte[2049];
            rnd.NextBytes(orig2049);

            var orig4097 = new byte[4097];
            rnd.NextBytes(orig4097);

            var orig8193 = new byte[8193];
            rnd.NextBytes(orig8193);

            var enc1025 = orig1025.EncodeBase16();
            var enc2049 = orig2049.EncodeBase16();
            var enc4097 = orig4097.EncodeBase16();
            var enc8193 = orig8193.EncodeBase16();

            var dec1025 = enc1025.DecodeBase16();
            var dec2049 = enc2049.DecodeBase16();
            var dec4097 = enc4097.DecodeBase16();
            var dec8193 = enc8193.DecodeBase16();

            Assert.AreEqual(orig1025, dec1025);
            Assert.AreEqual(orig2049, dec2049);
            Assert.AreEqual(orig4097, dec4097);
            Assert.AreEqual(orig8193, dec8193);
        }

        [Test, Category("Base16")]
        public void Base16EncodeDecodeRandom()
        {
            for (int i = 1; i < 5; i++)
            {
                var data = new byte[rnd.Next(10000) + 1];
                rnd.NextBytes(data);

                var enc = data.EncodeBase16();
                Assert.AreEqual(data, enc.DecodeBase16(), $"Error decoding {data.Dump()}");
            }
        }

        [Test, Category("Base16")]
        public void Base16EncodeDecodeSmall()
        {
            for (int i = 0; i < 12; i++)
            {
                var data = new byte[i];
                rnd.NextBytes(data);

                var enc = data.EncodeBase16();
                Assert.AreEqual(data, enc.DecodeBase16(), $"Error decoding {data.Dump()}");
            }
        }
        #endregion

        private static object[] Base64ByteTestData =
        {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
            new object[] {string.Empty.ToBytes(), string.Empty},
            new object[] {"00".ToBytes(), "AA=="},
            new object[] {"0000".ToBytes(), "AAA="},
            new object[] {"000000".ToBytes(), "AAAA"},
            new object[] {"FF".ToBytes(), "/w=="},
            new object[] {"FFFF".ToBytes(), "//8="},
            new object[] {"FFFFFF".ToBytes(), "////"},
            new object[] {"FFEF".ToBytes(), "/+8="},
            new object[] {"00003E".ToBytes(), "AAA+"},
            new object[] {"00003F".ToBytes(), "AAA/"}
        };

        private static object[] Base64TestData =
        {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
            new [] {string.Empty, string.Empty},
            new [] {"A", "QQ=="},
            new [] {"AA", "QUE="},
            new [] {"AAA", "QUFB"},
            new [] {" ", "IA=="},
            new [] {"  ", "ICA="},
            new [] {"   ", "ICAg"},
            new [] {"test1...", "dGVzdDEuLi4="},
            new [] {"f", "Zg=="},
            new [] {"fo", "Zm8="},
            new [] {"foo", "Zm9v"},
            new [] {"foob", "Zm9vYg=="},
            new [] {"fooba", "Zm9vYmE="},
            new [] {"foobar", "Zm9vYmFy"},
            new [] {"Hello World", "SGVsbG8gV29ybGQ="}
        };

        private static object[] Base64UrlByteTestData =
        {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
            new object[] {string.Empty.ToBytes(), string.Empty},
            new object[] {"00".ToBytes(), "AA=="},
            new object[] {"0000".ToBytes(), "AAA="},
            new object[] {"000000".ToBytes(), "AAAA"},
            new object[] {"FF".ToBytes(), "_w=="},
            new object[] {"FFFF".ToBytes(), "__8="},
            new object[] {"FFFFFF".ToBytes(), "____"},
            new object[] {"FFEF".ToBytes(), "_-8="},
            new object[] {"00003E".ToBytes(), "AAA-"},
            new object[] {"00003F".ToBytes(), "AAA_"}
        };

        private static object[] Base64UrlTestData =
        {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
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
            new object[] {(uint)299542347, "TUEWE"},
            new object[] {(uint)5069456, "SyyR"},
            new object[] {(uint)46789, "Eui"},
            new object[] {(uint)3379468205, "69dfAC"}
        };

        private static object[] Base58DecodeData =
        {
            // Data From: https://github.com/bitcoin/bitcoin/blob/master/src/test/base58_tests.cpp#L69-L73
            new object[] {" \t\n\v\f\r skip \r\f\v\n\t ", "971a55".ToBytes()},
        };

        private static object[] Base58TestData =
        {
            // Data From: https://github.com/bitcoin/bitcoin/blob/master/src/test/data/base58_encode_decode.json
            new object[] {"61".ToBytes(), "2g"},
            new object[] {"626262".ToBytes(), "a3gV"},
            new object[] {"636363".ToBytes(), "aPEr"},
            new object[] {"73696d706c792061206c6f6e6720737472696e67".ToBytes(), "2cFupjhnEsSn59qHXstmK2ffpLv2"},
            new object[] {"00eb15231dfceb60925886b67d065299925915aeb172c06647".ToBytes(), "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"},
            new object[] {"516b6fcd0f".ToBytes(), "ABnLTmg"},
            new object[] {"bf4f89001e670274dd".ToBytes(), "3SEo3LWLoPntC"},
            new object[] {"572e4794".ToBytes(), "3EFU7m"},
            new object[] {"ecac89cad93923c02321".ToBytes(), "EJDM8drfXA6uyA"},
            new object[] {"10c8511e".ToBytes(), "Rt5zm"},
            new object[] {"00000000000000000000".ToBytes(), "1111111111"},

			// Data From: https://github.com/ThePiachu/Bitcoin-Unit-Tests/blob/master/Address/Address%20Generation%20Test%201.txt
            new object[] {"00010966776006953D5567439E5E39F86A0D273BEED61967F6".ToBytes(), "16UwLL9Risc3QfPqBUvKofHmBQ7wMtjvM"},
            new object[] {"003A91CC0AF51BE125369A25FAC9CE5A950EF491AB06B2C8FB".ToBytes(), "16LgrHNVKbrySfS97wegnWWA5P8fb62FQN"},
            new object[] {"00714076A39428B9B904F4007DCD1519EF97B8784775C992F5".ToBytes(), "1BKpag8kykZNTxR2mw5qTEwUwmZX71c3JU"},
            new object[] {"00B9E57DF33679BCAABF4F7318082B703E4859AFCEDC276FCE".ToBytes(), "1HwvvsZjbAATRm5V1mw6i7g8sZ8gRqQQfX"},
            new object[] {"00D3A0742714A3BE8A2005D5698464A3FF4BAA94F5FD87AF18".ToBytes(), "1LHynznJ1kq4QcSUmCSgSo1HfUYP7UWcKZ"},
            new object[] {"0069325C54C6C8AB9D1E35DCEE0241E37A8820E5C709F1DBFB".ToBytes(), "1AbEHGF2iFLR1MdpRzMmRfVzpMF32cWwGe"},
            new object[] {"00EBC4E370B8B4908B8387A9442D7D9BA69C8B43B92D14F43A".ToBytes(), "1NVdf8Twtk8CivHQDwR3PZGsHhiemCy7ay"},
            new object[] {"00B845CC57712326B953FF26A7F20763025DE3C16ABC114C14".ToBytes(), "1HoLwxm944HpHWgEi2B2RYK6QN2icAaq9m"},
            new object[] {"00E026053C1AE60FEABC4BCFC80D89900573197A10C26D4C1C".ToBytes(), "1MSBugGhNwy3imagYRHQV1hnAqcfjPTUou"},
            new object[] {"00B58EAE755587554B2C52D0155DFA670DA90E0A3090485A28".ToBytes(), "1HYzEQSgoJT58XqjjpSVEwpahqhVdazRFd"},            new object[] {"003B3AC2F3ED6DCD58A249316B6DCE9BD655F43434C307ED7D".ToBytes(), "16QBG9BJ4XGKCt5zgKUDo6Hwimg1ZukQbN"},            new object[] {"00F9E7C2E6C0343120AA7FEB14D62650C87FFC06DB71332791".ToBytes(), "1PnNtpt5qLMoSEHHA5nq9T4N2hgRpqR4GY"},            new object[] {"005F7CACB3D4A78BC61E34A9823F6B11EA31CC133F6E94A1A9".ToBytes(), "19htYmZW8dLtWtoX7ZsxLWSS7Ugh47aBiG"},            new object[] {"002B2DD9C6DD0EAE871553DB582B8E6537F25F24C32BDEEEE3".ToBytes(), "14wJz1cUbJhPLxQRopfQG1KBuerudq1PHk"},            new object[] {"0029FA1AC9EDC3E161E5EF726E68A2D7B545A2CB22409D6C5E".ToBytes(), "14pxKSYJdKpSft4HjDFZFmfzYfSjFvy8aR"},            new object[] {"003AF7CFE671C5562CCF81AAD70670E390D8BA77AD89E43BAF".ToBytes(), "16No4QymhRhPuDu5Gi14jaNWN7aPZtEoP4"},            new object[] {"005AB3ACE64A6B1165689191B8EEA3A77109CA654A7975132D".ToBytes(), "19Gb4L7rNtM8mc95A8fakX7aCbdVae7pCt"},            new object[] {"00195F24BE5D681C10F07A32D9CB3E46301400ABBF8693255A".ToBytes(), "13K9sV5WgQsuZsAagXEph2z7batg5xKv4y"},            new object[] {"00308604F32F304A66D793F6E8B2D04D5801C8862100F653C5".ToBytes(), "15RZz31EToZB93v1tvKrbGMk4VfpWs4gkL"},            new object[] {"0086181EC307CF9BC2B9B46CB31F84A73438A518868FA455B0".ToBytes(), "1DE2Sc1YuHzQjYqdVmUnxWRj61uYctYfpB"},            new object[] {"000A08201462985DF5255E4A6C9D493C932FAC98EF791E2F22".ToBytes(), "1v3VUYGogXD7S1E8kipahj7QXgC568dz1"},            new object[] {"001E4BC2766F178E2A489E1C6A4DF4B01385B97F259C318AEB".ToBytes(), "13mC2dHQtUkChaVtR9sf6aZuWsDZJxVomC"},            new object[] {"0072DE1F2270445982A2F8E4A40A61C3988E98BD72079D8F37".ToBytes(), "1BUN89fNJ5re89HsoE45EsqTYP7NpnNiMt"},            new object[] {"0092ED8B99FA31F1D9B95811BC46591FF4A5E5465320CF0D3E".ToBytes(), "1EPtDwq1JzPVC5QmkJgzt52t84ox2iaNPw"},            new object[] {"0013E945F6293C717D0F8C2ACE6F0A787FFA2BD378DE448867".ToBytes(), "12pHHkuKTvHhDWfVTjAXYJ5eevehwsig62"},            new object[] {"00066C0B8995C7464E89F6760900EA6978DF18157388421561".ToBytes(), "1axVFjCkMWDFCHjQHf99AsszXTuzxLxxg"},            new object[] {"00806CD4DA88637EBC986E50F7055BC2D7D3B9D795DE06342D".ToBytes(), "1Ci3sJCziZVp5upNMWM1YPK7w2g3hyTB6L"},            new object[] {"00A2AEE0CAA44720194D1AC0CBE8122AE2E60F2CA75167D5CD".ToBytes(), "1FqBxpXQBzwLt653CP1iawqw8zSFmbBsW4"},            new object[] {"00594525A547114F148B478A81799C929ED3B093F5CAE0B494".ToBytes(), "1991yVg8QC5YVZP8b9eXUEVyQK2w8Bnrjd"},            new object[] {"00577FF59B067A4CB8F8568A71C2854BB6A2F5401D3DE90BBB".ToBytes(), "18yf5TuteMFxMQ8sL3gDL3heNw8E1BNFPY"},            new object[] {"00A5ED461BA0DC730C86BED50CD2E349B75060D10D4D35D9A0".ToBytes(), "1G8LjjGaQi1af22v6w1LoshS8RU2i7BfcB"},            new object[] {"006D3A1066DB55B3C60D9377EC06C8ADB289B17BED44221FAD".ToBytes(), "1AxYDGmJREWVqiQ9ByFdfMMrx7Nmt5vyDn"},            new object[] {"005BC8F038248421B30E6E6623CA6B02D74240E84445D91A32".ToBytes(), "19NKCu73shKmkUvEn2FQiQbQm8vqX7gKXf"},            new object[] {"00626C03CA222E4C80C6AB17FE998AE7B1ECDC614635AF1E25".ToBytes(), "19yQcoNR5Qd5K9hqsttpNZGVWdDZ9qpjNc"},            new object[] {"00169EBCCE898C48B5C00C18936176E6EC3272428C5FEEFDA5".ToBytes(), "134c2VpoHLvP4keud1o19cDxa2o3j8HR7z"},            new object[] {"0046D3CBA0FD26896F8F48F1BA3B9D60891B56CBE582C4CE6D".ToBytes(), "17TW2EUKsGaFtCmH7qQ7tm9wkhwxNbJWS8"},            new object[] {"00A6CF233CAE76EFB882A61EA64C2603FD3DA09569A2E22885".ToBytes(), "1GD1K29KMPJVVxgrQdk5LoojP3eS2ewuh6"},            new object[] {"0018DAD6B70ED6A04EF95C495680A25C487C4730AE60A72CCA".ToBytes(), "13GRNnZSopMpRiPM5zF2zcLF3No1T3coLm"},            new object[] {"001D781AC1CE8810959A266DC9B8B7D4C827BC13B33D64818A".ToBytes(), "13gpUa9EUGQ1yCqMaoMCkUT2n2RQHXjSkR"},            new object[] {"009308239688F8207B2DE68A60E5E0E7AB0BDC4E2E2310CD87".ToBytes(), "1EQS5i433M1qcLXjLfsc6QepF4m3F6UavS"},            new object[] {"00216638B2991F207396B3260850AFA09DDF735DA2D9D26B74".ToBytes(), "143bkoaLqFZiYco2qq7DuQ6KdgzpktJoqd"},            new object[] {"005F0A37BFA06A585FB757BA8AC97D2E4F75F81642B8285738".ToBytes(), "19fXSAgFDFfBRKXkUfpq5KDLRAbAJHKiDD"},            new object[] {"00E353C6B07DDA0F8B090528224975AEBC6731A388137063C7".ToBytes(), "1MizkQxgEnxobrqvCapigaGTDgcHcojwFt"},            new object[] {"00C915743E0414524144290104B97C1A7D986224604B8CCD66".ToBytes(), "1KLEXDMGKdGtofsgrxkKzKWZAhnbzLmFWm"},            new object[] {"009DC1F34CE031CE342B4D9975300E584B068A78BE88B7DC79".ToBytes(), "1FP9S2ZJrkH8aC8cvDCbAfdP3waRKhjwmS"},            new object[] {"0042DCFA01FF087324C3D8905DE2E9C861D6417B1E70F11248".ToBytes(), "176YKPFcmkGY6yKyTVPTvDKq9QJZHm4v5u"},            new object[] {"00B768BE416DFD0A2C677662981D4BFBEE2FFDDBE033B2B517".ToBytes(), "1Hin8nmZRVPvg8DLqUcmYyXRPC7KYZPBsC"},            new object[] {"00FDEA19A53C34647A6D3717F8B7175F887339095AB388B1E0".ToBytes(), "1Q9aQ6ieKu6TXbY6qZBkC94FuzyHdPm8vb"},            new object[] {"00B557AF58BECDCA88E850F7FCE53F892A065C47BB36F7ED93".ToBytes(), "1HXrMBLnFHTzzaNjzZvx4RxcWUtw4523mC"},            new object[] {"004A6295042BD40907C37DF5B8D3954F8D4FF664E17260F134".ToBytes(), "17nK6oNULuoJ4JeNq6AUC8AAg3eNdu92QT"},            new object[] {"00EA14A277F1B153C1DCB6FE0398D0C50ECE82F4E77DF87D70".ToBytes(), "1NLhqcHkbBSiWz46qB48LyV8uqN2ks96Mu"},            new object[] {"00D285891CFB0535BCD6C22F529C68C5CB6666B9CDE3668668".ToBytes(), "1LC8sW2SKeAY4yEN5dSipi9s6xxcK2HYzK"},            new object[] {"0023A330ABCF3401FDA55FA55038CC8A83112ADFE3D6DF2758".ToBytes(), "14FS9N2Rqfc3jDfZbuZ7FXBewEh8i3TGxw"},            new object[] {"004CEB9C62F7683B0C2F34174F5A72425CD0334618BB0901A8".ToBytes(), "181ic7QQ87gaeHBguQVXTrV5PggL8DwhdH"},            new object[] {"00DBDBC96E594A9971E6A9E11CAE63599970A2BE3387551C11".ToBytes(), "1M3WH4xWzR3wPqNiPNes2q1f8nsZabd7kU"},            new object[] {"0073F4F8FDC294A60B8221D975C6FFED352D6EF39B4D18681B".ToBytes(), "1Ba8B4HfYBzt9KiMqMYU9TR9Lq45yzFxzv"},            new object[] {"00F3A2EB49A0126D627BBA601DF40CCCB64622A64F08697F16".ToBytes(), "1PDENS3TqMob3C74JTE38hzrTdBFgTxgxD"},            new object[] {"0099A07C6E98015C1A697BC77F66FD39918E73785EE5F85179".ToBytes(), "1F1Je7o9EQwWgwT9tj7TgRWQjMosrVH3uA"},            new object[] {"00296E9647426DD41793F7493002E10A8FAEDE8D5312FBBDFD".ToBytes(), "14n5Bb16wGDbQUxwt3BLfhESWhhBkhNLmr"},            new object[] {"00D355A8D25F4A11B1CDC84C10CA3B1A72CEC0B3346DFF3EE9".ToBytes(), "1LGSCCbGYHWigLoPvCKABMNhHaBWXWuWXS"},            new object[] {"009C62F60CFEAE9D3191683284580FA8C428E0AAC5CA6DCFD0".ToBytes(), "1FFtxsDZbGYy81z5fTtjRp5CwpqisSSLhV"},            new object[] {"00DC6B63B26C091D95DD425BC4364B4675D90A57CCBEE21DFC".ToBytes(), "1M6UJkcKtMSS2fkQStuJMK8G4FTbksNXm5"},            new object[] {"00481BB6F9970DF701A740B0FCB21167B69FAE5096C51E0FCC".ToBytes(), "17aGrU2xAi1ZXxjGwCtSYMrj55RdY2VwiT"},            new object[] {"0059265249A1E3EA920E4E06121CE7D8DD645B5AC6E686EF6E".ToBytes(), "198P3hC8GaTYvxjLVKmkP4GeKKGVCtNH6u"},            new object[] {"00EB96CE905D65F30522CD0AE781174DDEFBD814D4E8B60DF4".ToBytes(), "1NUgTKwddyHSjCWAwdv9p6tien5LdBPohV"},            new object[] {"00311693C6C8F52765561CA55F9A02F79C020C93BEAC42AB1A".ToBytes(), "15UZA6VRzFAL9zzWq3XDn4ffaEW1ZnnbJZ"},            new object[] {"004CBD61A95BE05E28CEB6825334AF3A14B2DAF469F386FEE5".ToBytes(), "17zmE35GixgEnee1W7cL4hYBiHfDC6FgzY"},            new object[] {"005D5055A3F9651A5CBF3BBA87FE78BE8041333AF510659B63".ToBytes(), "19WQ5aj2FMvWPfZ1Np9HqVw5LFie8BXD6a"},            new object[] {"0086BA80A266D2F66E44A69EFEA5F513DCE84D2F7F09A3BDC5".ToBytes(), "1DHNy8EFMdTYmY4swL4ET6HsdX5S7pbQnQ"},            new object[] {"0042388BE358449C334DC2743B919ACE957068C44A0EBF7FCF".ToBytes(), "1739LaRGvqKCSWmmiWXjjuTKSbjRePHbv6"},            new object[] {"008138F4B6D62066DE9637817D36B80C6F8D29572A723B753A".ToBytes(), "1CnGQ7XLUtaSZ8YqMC3Go2NiMvJtatPDmK"},            new object[] {"00BD21AD78AB0730F8290AE15187B42BE6D0B2BCF7FA8D072C".ToBytes(), "1JF34Dey5zTQRQo3hckJnwEJKRNMs1z1j1"},            new object[] {"0030F80BC8BBC465EB78F012AE98B5952CDCC8F5AA799EB50E".ToBytes(), "15TvakQEGf6pnvUYr5oWWCuXBYLVEPrF7K"},            new object[] {"00FDDE6A534D657DF2F9770D0E6E562D6A4754802ABC010026".ToBytes(), "1Q9LQDqmvmPGGxWofMFqVyKACzsrDK21zu"},            new object[] {"009C2E4CC5445D078A73D2DCD96C887A0AFFC818C37E3DD988".ToBytes(), "1FEosu5j5xnVhT2Z5UhhSDkFg4Eo73cfSo"},            new object[] {"00E6630D42BE63EB7C113C49E35AA01734696BF3B9A1796BCA".ToBytes(), "1N1B5MvNKjkJoJJnnWEWUf5ZNrXnYVX1b7"},            new object[] {"00BD65A9A0334AC2D0B938D306C6E70436E68B9D2A9F5AD136".ToBytes(), "1JGSVu3TFzG5aRHRJ42vMbXLEbsQnf7S7f"},            new object[] {"00E23583535EED6C6357E1D75F357A32F6F7D376D6E067AE8F".ToBytes(), "1Md5pW5vt8oojMzqWkNTRoA4jPtP5aTGyc"},            new object[] {"0073821DC6D8FD9674836F175C9299CD5B2992A3F3422D83E6".ToBytes(), "1BXkahdmviiWTVCyBPFH3chVbVF3PVVXL1"},            new object[] {"00D98AC92610EC6ACA0B40C1546BCED67296C7D700056353B2".ToBytes(), "1LqFtdVrdWsqwGkr6qbh8LuX1ktnTJdXbP"},            new object[] {"009C22A12FA7D0AA54AD11CCC54CC509030DA66BC6D79C4E23".ToBytes(), "1FEZu31b22hL6e6HQFthCM6ckMw1mLj7cW"},            new object[] {"00606EC321732057A3D7C62DD8D6B8E090389F89B25168359B".ToBytes(), "19ntZJzv8YvQ2KH8Kgvo2c5agmUZC6vibk"},            new object[] {"00432AD84BBCF97A0A827A291937E7FFF21C29C10E6DC192EC".ToBytes(), "1789bmbVNaCCCXzzqsYS4ZX1EugPxU8AVh"},            new object[] {"004012E11CA9DDCEE85CCA1411BC9CF40B555C27967F6C1743".ToBytes(), "16qns39G7VQRq54j1nNaNsaVBq25Us38Bt"},            new object[] {"009204F202D1B6F50B3CA874D40BF8858E3D1BEA2EABF93C4C".ToBytes(), "1EK5acm8XMa9F79tNPt2TbLeNuDV1bDHPH"},            new object[] {"00C5B11821216BE3A9AABE7797229FEFF417F684DEA066F1B1".ToBytes(), "1K2JGWt5TLppCTtzksQgovDBDWukcBr84Y"},            new object[] {"007180D1411FF9794A82AB30A858CADAAE3149FDCA07063C00".ToBytes(), "1BM9g4sNicWp6ZARz3kLFvX7bVYoxwt4pw"},            new object[] {"00F974296D3BC2F71852DCA4B920B18DECAA81453187AFB6F1".ToBytes(), "1PjzQqBb1ugxoZXXgMqPfVuwjfcC3NmWYg"},            new object[] {"002DD5937F1B024013D67E25EF7468107B5D0F621A95E3C860".ToBytes(), "15BMGAWgmDA1BKXzGVzz57R9sX7GeCRErb"},            new object[] {"00E86AC97AF8BBDBFEA78E3C42D7203962415B4F0C0E568046".ToBytes(), "1NBuhCUGgQtPJveBEJ8UjiEKPbq6gvkRrd"},            new object[] {"00744B4B83A9C07194DCA464A2654D0B5FD9E8B205F0188AF5".ToBytes(), "1BbuaqfjUTDSPNMXQKg2BWWiL2ZxqqMZaU"},            new object[] {"0038017F2CE3CB04F94F552D6361034A60B86E88AC4901F9FF".ToBytes(), "1678djx5jWoV5mBiJjUKcTTxyDfGFA18Nr"},            new object[] {"005E6E2C40D9B24B89186B38970DB98E6650116AFBC20AF6F4".ToBytes(), "19cJVyfFXR8YmLUCaGxBSMy7AVPua94JzF"},            new object[] {"005520E1E2BBE0B2103D44293793B81BE55893E7606E8CC51A".ToBytes(), "18m7q1io9qRX6zM2663gTsDhNnfiLGEsE9"},            new object[] {"00B2143A1A63E11B6F676D9DA9B2F2A38A0EBA52536E3C9B19".ToBytes(), "1HEbWYKYG1R1XbwhCV3KST1DWMSQSLSPqi"},            new object[] {"0091FF610A0133B8EF154A224FFABF5B1D85D5265E6D44CE5A".ToBytes(), "1EJxus5oozeZZ6KatBhQ9a1fmUM57Us95K"},            new object[] {"00498D652344FD760A86853CB75F2AB9A592406A78E28768D7".ToBytes(), "17huiJhwuvADPyXQ9zchokjdp7ch48w64v"},            new object[] {"00AD6997CFD7A9679266926836B26D30C6B6679EEEFA0873B9".ToBytes(), "1GovPvF5c1vpYJcFx8LmydHKeT5viUXewA"},            new object[] {"00EF7CA4B03BA1400CEC173A3F3B6918AFDE950562262F4AC9".ToBytes(), "1NqHpDaBaSKTpxUbYPbT8nqgcnX7HY83E8"},            new object[] {"004002133980DFF9E3BB4FF6F96A43294BC501F35D25ED96E8".ToBytes(), "16qSjTXYtgcGQYefbBJhzhk9Xskhvfy5tB"}
        };

        private static object[] Base32ByteTestData =
        {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
            new object[] {"00".ToBytes(), "AA======"},
            new object[] {"0000".ToBytes(), "AAAA===="},
            new object[] {"000000".ToBytes(), "AAAAA==="},
            new object[] {"00000000".ToBytes(), "AAAAAAA="},
            new object[] {"0000000000".ToBytes(), "AAAAAAAA"},
            new object[] {"FF".ToBytes(), "74======"},
            new object[] {"FFFF".ToBytes(), "777Q===="},
            new object[] {"FFFFFF".ToBytes(), "77776==="},
            new object[] {"FFFFFFFF".ToBytes(), "777777Y="},
            new object[] {"FFFFFFFFFF".ToBytes(), "77777777"},
            new object[] {"FFEF".ToBytes(), "77XQ===="},
            new object[] {"00003E".ToBytes(), "AAAD4==="},
            new object[] {"00003F".ToBytes(), "AAAD6==="},
            new object[] {"2128C1AD".ToBytes(), "EEUMDLI="},
            new object[] {"C2A642F8".ToBytes(), "YKTEF6A="}
        };

        private static object[] Base32TestData_uint =
        {
            new object[] {(uint)299542347, "I5VJ2L"},
            new object[] {(uint)3379468205, "DEW5F5N"},
            new object[] {(uint)10138912, "JVNJA"},
            new object[] {(uint)5069456, "E2WUQ"},
            new object[] {(uint)46789, "BNWF"}
        };

        private static object[] Base32TestData =
        {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
            new [] {string.Empty, string.Empty},
            new [] {" ", "EA======"},
            new [] {"  ", "EAQA===="},
            new [] {"   ", "EAQCA==="},
            new [] {"    ", "EAQCAIA="},
            new [] {"     ", "EAQCAIBA"},
            new [] {"test1...", "ORSXG5BRFYXC4==="},
            new [] {"f", "MY======"},
            new [] {"fo", "MZXQ===="},
            new [] {"foo", "MZXW6==="},
            new [] {"foob", "MZXW6YQ="},
            new [] {"fooba", "MZXW6YTB"},
            new [] {"foobar", "MZXW6YTBOI======"}
        };

        private static object[] Base32HexaByteTestData =
       {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
            new object[] {string.Empty.ToBytes(), string.Empty},
            new object[] {"00".ToBytes(), "00======"},
            new object[] {"0000".ToBytes(), "0000===="},
            new object[] {"000000".ToBytes(), "00000==="},
            new object[] {"00000000".ToBytes(), "0000000="},
            new object[] {"0000000000".ToBytes(), "00000000"},
            new object[] {"FF".ToBytes(), "VS======"},
            new object[] {"FFFF".ToBytes(), "VVVG===="},
            new object[] {"FFFFFF".ToBytes(), "VVVVU==="},
            new object[] {"FFFFFFFF".ToBytes(), "VVVVVVO="},
            new object[] {"FFFFFFFFFF".ToBytes(), "VVVVVVVV"},
            new object[] {"FFEF".ToBytes(), "VVNG===="},
            new object[] {"00003E".ToBytes(), "0003S==="},
            new object[] {"00003F".ToBytes(), "0003U==="},
            new object[] {"5F1E45D0".ToBytes(), "BSF4BK0="},
            new object[] {"E6E21ABB".ToBytes(), "SRH1LEO="}
        };

        private static object[] Base32HexaTestData_uint =
        {
            new object[] {(uint)299542347, "8TL9QB"},
            new object[] {(uint)3379468205, "34MT5TD"},
            new object[] {(uint)10138912, "9LD90"},
            new object[] {(uint)5069456, "4QMKG"},
            new object[] {(uint)46789, "1DM5"}
        };

        private static object[] Base32HexaTestData =
        {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
            new [] {string.Empty, string.Empty},
            new [] {" ", "40======"},
            new [] {"  ", "40G0===="},
            new [] {"   ", "40G20==="},
            new [] {"    ", "40G2080="},
            new [] {"     ", "40G20810"},
            new [] {"test1...", "EHIN6T1H5ON2S==="},
            new [] {"f", "CO======"},
            new [] {"fo", "CPNG===="},
            new [] {"foo", "CPNMU==="},
            new [] {"foob", "CPNMUOG="},
            new [] {"fooba", "CPNMUOJ1"},
            new [] {"foobar", "CPNMUOJ1E8======"}
        };

        private static object[] ZBase32ByteTestData =
       {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
            new object[] {string.Empty.ToBytes(), string.Empty},
            new object[] {"00".ToBytes(), "yy"},
            new object[] {"0000".ToBytes(), "yyyy"},
            new object[] {"000000".ToBytes(), "yyyyy"},
            new object[] {"FF".ToBytes(), "9h"},
            new object[] {"FFFF".ToBytes(), "999o"},
            new object[] {"FFFFFF".ToBytes(), "99996"},
            new object[] {"FFEF".ToBytes(), "99zo"},
            new object[] {"00003E".ToBytes(), "yyydh"},
            new object[] {"00003F".ToBytes(), "yyyd6"},
            new object[] {"62EF26A7".ToBytes(), "cmz1pja"},
            new object[] {"6CF060F3C01E9102B9".ToBytes(), "puagbh6yd4eofqe"}
        };

        private static object[] ZBase32TestData_uint =
        {
            new object[] {(uint)299542347, "e7ij4m"},
            new object[] {(uint)3379468205, "drs7f7p"},
            new object[] {(uint)10138912, "jipjy"},
            new object[] {(uint)5069456, "r4swo"},
            new object[] {(uint)46789, "bpsf"}
        };

        private static object[] ZBase32TestData =
        {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
            new [] {string.Empty, string.Empty},
            new [] {" ", "ry"},
            new [] {"  ", "ryoy"},
            new [] {"   ", "ryony"},
            new [] {"    ", "ryonyey"},
            new [] {"     ", "ryonyeby"},
            new [] {"test1...", "qt1zg7btfaznh"},
            new [] {"f", "ca"},
            new [] {"fo", "c3zo"},
            new [] {"foo", "c3zs6"},
            new [] {"foob", "c3zs6ao"},
            new [] {"fooba", "c3zs6aub"},
            new [] {"foobar", "c3zs6aubqe"}
        };

        private static object[] Base16TestData_uint =
        {
            new object[] {(uint)299542347, "11DAA74B"},
            new object[] {(uint)3379468205, "C96E97AD"},
            new object[] {(uint)10138912, "9AB520"},
            new object[] {(uint)5069456, "4D5A90"},
            new object[] {(uint)46789, "B6C5"}
        };

        private static object[] Base16TestData =
        {
            // Data From: https://tools.ietf.org/html/rfc4648#page-12
            new [] {string.Empty, string.Empty},
            new [] {" ", "20"},
            new [] {"  ", "2020"},
            new [] {"   ", "202020"},
            new [] {"test1...", "74657374312E2E2E"},
            new [] {"f", "66"},
            new [] {"fo", "666F"},
            new [] {"foo", "666F6F"},
            new [] {"foob", "666F6F62"},
            new [] {"fooba", "666F6F6261"},
            new [] {"foobar", "666F6F626172"}
        };

        const string ENCODED_64_CHARS_PER_LINE =
            "9IPNKwUvdLiIAp6ctz12SiQmOGstWyYvSPeevufDhrzaws65voykKjbIj33YWTa9\n" +
            "xA7c/FHypWclrZhQ7onfc3JE93BJ5fT4R9zAEdjbjy1hv4ZYNnET4WJeXMLJ/5p+\n" +
            "qBpTsPpepW8DNVYy1c02/1wyC+kgA6CvRUd9cSr/lt88AEdsTV4GMCn1+EwuAiYd\n" +
            "ivxuzn+cLM8q2jewqlI52tP9J7Cs8vqG71s6+WAELKvm/UovvyaOi+OdMUfjQ0JL\n" +
            "iLkHu6p9OwUgvQqiDKzEv/Augo0dTPZzYGEyCP5GVrle3QQdgciIHnpdd4VUTPGR\n" +
            "UbXeKbh++U3fbJIng/sQXM3IYByMZ7xt9HWS1LUcRdQ7Prwn/IlQWxOMeq+KZJSo\n" +
            "AviWtdserXyHbIEa//hmr4p/j80k0g9q35hq1ayGM9984ALTSaZ8WeyFbZx1CxC/\n" +
            "Qoqf92UH/ylBRnSJNn4sS0oa3uUbNvOnpkB4D9V7Ut9atinCJrw+wiJcMl+9kp25\n" +
            "1IUxBGA4cUxh0eaxk3ODWnwI95EktmWOKwCSP0xjWwIMxDjygwAG5R8fk9H9bVi1\n" +
            "thMavm4nDc4vaNoSE1RnZNYwbiUVlVPM9EclvJWTWd6igWeA0MxHAA8iOM5Vnmqp\n" +
            "/WGM7UDq59rBIdNQCoeTJaAkEtAuLL5zogOa5e+MzVjvB5MYQlOlaaTtQrRApXa5\n" +
            "Z4VfEanu9UK2fi1T8jJPFC2PmXebxp0bnO+VW+bgyEdIIkIQCaZq1MKWC3KuiOS9\n" +
            "BJ1t7O0A2JKJKvoE4UNulzV2TGCC+KAnmjRqQBqXlJmgjHQAoHNZKOma/uIQOsvf\n" +
            "DnqicYdDmfyCYuV89HjA1H8tiDJ85VfsrFHdcbPAoNCpi65awJSHfdPO1NDONOK+\n" +
            "+S7Y0VXUgoYYrBV4Y7YbC8wg/nqcimr3lm3tRyp+QsgKzdREbfNRk0F5PLyLfsUE\n" +
            "lepjs1QdV3fEV1LJtiywA3ubVNQJRxhbYxa/C/Xy2qxpm6vvdL92l3q1ccev35Ic\n" +
            "aOiSx7Im+/GxV2lVKdaOvYVGDD1zBRe6Y2CwQb9p088l3/93qGR5593NCiuPPWcs\n" +
            "DWwUShM1EyW0FNX1F8bnzHnYijoyE/jf4s/l9bBd7yJdRWRCyih2WcypAiOIEkBs\n" +
            "H+dCTgalu8sRDoMh4ZIBBdgHfoZUycLqReQFLZZ4Sl4zSmzt5vQxQFhEKb9+ff/4\n" +
            "rb1KAo6wifengxVfIsa2b5ljXzAqXs7JkPvmC6fa7X4ZZndRokaxYlu3cg8OV+uG\n" +
            "/6YAHZilo8at0OpkkNdNFuhwuGlkBqrZKNUj/gSiYYc06gF/r/z6iWAjpXJRW1qq\n" +
            "3CLZXdZFZ/VrqXeVjtOAu2A=\n";

        const string ENCODED_76_CHARS_PER_LINE =
            "9IPNKwUvdLiIAp6ctz12SiQmOGstWyYvSPeevufDhrzaws65voykKjbIj33YWTa9xA7c/FHypWcl\n" +
            "rZhQ7onfc3JE93BJ5fT4R9zAEdjbjy1hv4ZYNnET4WJeXMLJ/5p+qBpTsPpepW8DNVYy1c02/1wy\n" +
            "C+kgA6CvRUd9cSr/lt88AEdsTV4GMCn1+EwuAiYdivxuzn+cLM8q2jewqlI52tP9J7Cs8vqG71s6\n" +
            "+WAELKvm/UovvyaOi+OdMUfjQ0JLiLkHu6p9OwUgvQqiDKzEv/Augo0dTPZzYGEyCP5GVrle3QQd\n" +
            "gciIHnpdd4VUTPGRUbXeKbh++U3fbJIng/sQXM3IYByMZ7xt9HWS1LUcRdQ7Prwn/IlQWxOMeq+K\n" +
            "ZJSoAviWtdserXyHbIEa//hmr4p/j80k0g9q35hq1ayGM9984ALTSaZ8WeyFbZx1CxC/Qoqf92UH\n" +
            "/ylBRnSJNn4sS0oa3uUbNvOnpkB4D9V7Ut9atinCJrw+wiJcMl+9kp251IUxBGA4cUxh0eaxk3OD\n" +
            "WnwI95EktmWOKwCSP0xjWwIMxDjygwAG5R8fk9H9bVi1thMavm4nDc4vaNoSE1RnZNYwbiUVlVPM\n" +
            "9EclvJWTWd6igWeA0MxHAA8iOM5Vnmqp/WGM7UDq59rBIdNQCoeTJaAkEtAuLL5zogOa5e+MzVjv\n" +
            "B5MYQlOlaaTtQrRApXa5Z4VfEanu9UK2fi1T8jJPFC2PmXebxp0bnO+VW+bgyEdIIkIQCaZq1MKW\n" +
            "C3KuiOS9BJ1t7O0A2JKJKvoE4UNulzV2TGCC+KAnmjRqQBqXlJmgjHQAoHNZKOma/uIQOsvfDnqi\n" +
            "cYdDmfyCYuV89HjA1H8tiDJ85VfsrFHdcbPAoNCpi65awJSHfdPO1NDONOK++S7Y0VXUgoYYrBV4\n" +
            "Y7YbC8wg/nqcimr3lm3tRyp+QsgKzdREbfNRk0F5PLyLfsUElepjs1QdV3fEV1LJtiywA3ubVNQJ\n" +
            "RxhbYxa/C/Xy2qxpm6vvdL92l3q1ccev35IcaOiSx7Im+/GxV2lVKdaOvYVGDD1zBRe6Y2CwQb9p\n" +
            "088l3/93qGR5593NCiuPPWcsDWwUShM1EyW0FNX1F8bnzHnYijoyE/jf4s/l9bBd7yJdRWRCyih2\n" +
            "WcypAiOIEkBsH+dCTgalu8sRDoMh4ZIBBdgHfoZUycLqReQFLZZ4Sl4zSmzt5vQxQFhEKb9+ff/4\n" +
            "rb1KAo6wifengxVfIsa2b5ljXzAqXs7JkPvmC6fa7X4ZZndRokaxYlu3cg8OV+uG/6YAHZilo8at\n" +
            "0OpkkNdNFuhwuGlkBqrZKNUj/gSiYYc06gF/r/z6iWAjpXJRW1qq3CLZXdZFZ/VrqXeVjtOAu2A=\n";

        readonly byte[] DECODED = Array.ConvertAll(new sbyte[] {
            -12, -125, -51, 43, 5, 47, 116, -72, -120, 2, -98, -100, -73, 61, 118, 74, 36, 38, 56, 107, 45, 91, 38,
            47, 72, -9, -98, -66, -25, -61, -122, -68, -38, -62, -50, -71, -66, -116, -92, 42, 54, -56, -113, 125,
            -40, 89, 54, -67, -60, 14, -36, -4, 81, -14, -91, 103, 37, -83, -104, 80, -18, -119, -33, 115, 114, 68,
            -9, 112, 73, -27, -12, -8, 71, -36, -64, 17, -40, -37, -113, 45, 97, -65, -122, 88, 54, 113, 19, -31, 98,
            94, 92, -62, -55, -1, -102, 126, -88, 26, 83, -80, -6, 94, -91, 111, 3, 53, 86, 50, -43, -51, 54, -1, 92,
            50, 11, -23, 32, 3, -96, -81, 69, 71, 125, 113, 42, -1, -106, -33, 60, 0, 71, 108, 77, 94, 6, 48, 41, -11,
            -8, 76, 46, 2, 38, 29, -118, -4, 110, -50, 127, -100, 44, -49, 42, -38, 55, -80, -86, 82, 57, -38, -45,
            -3, 39, -80, -84, -14, -6, -122, -17, 91, 58, -7, 96, 4, 44, -85, -26, -3, 74, 47, -65, 38, -114, -117,
            -29, -99, 49, 71, -29, 67, 66, 75, -120, -71, 7, -69, -86, 125, 59, 5, 32, -67, 10, -94, 12, -84, -60, -65,
            -16, 46, -126, -115, 29, 76, -10, 115, 96, 97, 50, 8, -2, 70, 86, -71, 94, -35, 4, 29, -127, -56, -120,
            30, 122, 93, 119, -123, 84, 76, -15, -111, 81, -75, -34, 41, -72, 126, -7, 77, -33, 108, -110, 39, -125,
            -5, 16, 92, -51, -56, 96, 28, -116, 103, -68, 109, -12, 117, -110, -44, -75, 28, 69, -44, 59, 62, -68,
            39, -4, -119, 80, 91, 19, -116, 122, -81, -118, 100, -108, -88, 2, -8, -106, -75, -37, 30, -83, 124, -121,
            108, -127, 26, -1, -8, 102, -81, -118, 127, -113, -51, 36, -46, 15, 106, -33, -104, 106, -43, -84, -122,
            51, -33, 124, -32, 2, -45, 73, -90, 124, 89, -20, -123, 109, -100, 117, 11, 16, -65, 66, -118, -97, -9,
            101, 7, -1, 41, 65, 70, 116, -119, 54, 126, 44, 75, 74, 26, -34, -27, 27, 54, -13, -89, -90, 64, 120, 15,
            -43, 123, 82, -33, 90, -74, 41, -62, 38, -68, 62, -62, 34, 92, 50, 95, -67, -110, -99, -71, -44, -123,
            49, 4, 96, 56, 113, 76, 97, -47, -26, -79, -109, 115, -125, 90, 124, 8, -9, -111, 36, -74, 101, -114, 43,
            0, -110, 63, 76, 99, 91, 2, 12, -60, 56, -14, -125, 0, 6, -27, 31, 31, -109, -47, -3, 109, 88, -75, -74,
            19, 26, -66, 110, 39, 13, -50, 47, 104, -38, 18, 19, 84, 103, 100, -42, 48, 110, 37, 21, -107, 83, -52,
            -12, 71, 37, -68, -107, -109, 89, -34, -94, -127, 103, -128, -48, -52, 71, 0, 15, 34, 56, -50, 85, -98,
            106, -87, -3, 97, -116, -19, 64, -22, -25, -38, -63, 33, -45, 80, 10, -121, -109, 37, -96, 36, 18, -48,
            46, 44, -66, 115, -94, 3, -102, -27, -17, -116, -51, 88, -17, 7, -109, 24, 66, 83, -91, 105, -92, -19,
            66, -76, 64, -91, 118, -71, 103, -123, 95, 17, -87, -18, -11, 66, -74, 126, 45, 83, -14, 50, 79, 20, 45,
            -113, -103, 119, -101, -58, -99, 27, -100, -17, -107, 91, -26, -32, -56, 71, 72, 34, 66, 16, 9, -90, 106,
            -44, -62, -106, 11, 114, -82, -120, -28, -67, 4, -99, 109, -20, -19, 0, -40, -110, -119, 42, -6, 4, -31,
            67, 110, -105, 53, 118, 76, 96, -126, -8, -96, 39, -102, 52, 106, 64, 26, -105, -108, -103, -96, -116,
            116, 0, -96, 115, 89, 40, -23, -102, -2, -30, 16, 58, -53, -33, 14, 122, -94, 113, -121, 67, -103, -4,
            -126, 98, -27, 124, -12, 120, -64, -44, 127, 45, -120, 50, 124, -27, 87, -20, -84, 81, -35, 113, -77,
            -64, -96, -48, -87, -117, -82, 90, -64, -108, -121, 125, -45, -50, -44, -48, -50, 52, -30, -66, -7, 46,
            -40, -47, 85, -44, -126, -122, 24, -84, 21, 120, 99, -74, 27, 11, -52, 32, -2, 122, -100, -118, 106, -9,
            -106, 109, -19, 71, 42, 126, 66, -56, 10, -51, -44, 68, 109, -13, 81, -109, 65, 121, 60, -68, -117, 126,
            -59, 4, -107, -22, 99, -77, 84, 29, 87, 119, -60, 87, 82, -55, -74, 44, -80, 3, 123, -101, 84, -44, 9, 71,
            24, 91, 99, 22, -65, 11, -11, -14, -38, -84, 105, -101, -85, -17, 116, -65, 118, -105, 122, -75, 113,
            -57, -81, -33, -110, 28, 104, -24, -110, -57, -78, 38, -5, -15, -79, 87, 105, 85, 41, -42, -114, -67,
            -123, 70, 12, 61, 115, 5, 23, -70, 99, 96, -80, 65, -65, 105, -45, -49, 37, -33, -1, 119, -88, 100, 121,
            -25, -35, -51, 10, 43, -113, 61, 103, 44, 13, 108, 20, 74, 19, 53, 19, 37, -76, 20, -43, -11, 23, -58, -25,
            -52, 121, -40, -118, 58, 50, 19, -8, -33, -30, -49, -27, -11, -80, 93, -17, 34, 93, 69, 100, 66, -54, 40,
            118, 89, -52, -87, 2, 35, -120, 18, 64, 108, 31, -25, 66, 78, 6, -91, -69, -53, 17, 14, -125, 33, -31, -110,
            1, 5, -40, 7, 126, -122, 84, -55, -62, -22, 69, -28, 5, 45, -106, 120, 74, 94, 51, 74, 108, -19, -26, -12,
            49, 64, 88, 68, 41, -65, 126, 125, -1, -8, -83, -67, 74, 2, -114, -80, -119, -9, -89, -125, 21, 95, 34,
            -58, -74, 111, -103, 99, 95, 48, 42, 94, -50, -55, -112, -5, -26, 11, -89, -38, -19, 126, 25, 102, 119,
            81, -94, 70, -79, 98, 91, -73, 114, 15, 14, 87, -21, -122, -1, -90, 0, 29, -104, -91, -93, -58, -83, -48,
            -22, 100, -112, -41, 77, 22, -24, 112, -72, 105, 100, 6, -86, -39, 40, -43, 35, -2, 4, -94, 97, -121, 52,
            -22, 1, 127, -81, -4, -6, -119, 96, 35, -91, 114, 81, 91, 90, -86, -36, 34, -39, 93, -42, 69, 103, -11,
            107, -87, 119, -107, -114, -45, -128, -69, 96
        }, (input) => unchecked((byte)input));
    }
}

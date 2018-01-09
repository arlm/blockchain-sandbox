using System;
using System.Text;
using BlockChain.CLI.Bitcoin;
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
            Assert.AreEqual(encodedData, plainText.EncodeBase58());
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
            new object[] {"00B58EAE755587554B2C52D0155DFA670DA90E0A3090485A28".ToBytes(), "1HYzEQSgoJT58XqjjpSVEwpahqhVdazRFd"},            new object[] {"003B3AC2F3ED6DCD58A249316B6DCE9BD655F43434C307ED7D".ToBytes(), "16QBG9BJ4XGKCt5zgKUDo6Hwimg1ZukQbN"},            new object[] {"00F9E7C2E6C0343120AA7FEB14D62650C87FFC06DB71332791".ToBytes(), "1PnNtpt5qLMoSEHHA5nq9T4N2hgRpqR4GY"},            new object[] {"005F7CACB3D4A78BC61E34A9823F6B11EA31CC133F6E94A1A9".ToBytes(), "19htYmZW8dLtWtoX7ZsxLWSS7Ugh47aBiG"},            new object[] {"002B2DD9C6DD0EAE871553DB582B8E6537F25F24C32BDEEEE3".ToBytes(), "14wJz1cUbJhPLxQRopfQG1KBuerudq1PHk"},            new object[] {"0029FA1AC9EDC3E161E5EF726E68A2D7B545A2CB22409D6C5E".ToBytes(), "14pxKSYJdKpSft4HjDFZFmfzYfSjFvy8aR"},            new object[] {"003AF7CFE671C5562CCF81AAD70670E390D8BA77AD89E43BAF".ToBytes(), "16No4QymhRhPuDu5Gi14jaNWN7aPZtEoP4"},            new object[] {"005AB3ACE64A6B1165689191B8EEA3A77109CA654A7975132D".ToBytes(), "19Gb4L7rNtM8mc95A8fakX7aCbdVae7pCt"},            new object[] {"00195F24BE5D681C10F07A32D9CB3E46301400ABBF8693255A".ToBytes(), "13K9sV5WgQsuZsAagXEph2z7batg5xKv4y"},            new object[] {"00308604F32F304A66D793F6E8B2D04D5801C8862100F653C5".ToBytes(), "15RZz31EToZB93v1tvKrbGMk4VfpWs4gkL"},            new object[] {"0086181EC307CF9BC2B9B46CB31F84A73438A518868FA455B0".ToBytes(), "1DE2Sc1YuHzQjYqdVmUnxWRj61uYctYfpB"},            new object[] {"000A08201462985DF5255E4A6C9D493C932FAC98EF791E2F22".ToBytes(), "1v3VUYGogXD7S1E8kipahj7QXgC568dz1"},            new object[] {"001E4BC2766F178E2A489E1C6A4DF4B01385B97F259C318AEB".ToBytes(), "13mC2dHQtUkChaVtR9sf6aZuWsDZJxVomC"},            new object[] {"0072DE1F2270445982A2F8E4A40A61C3988E98BD72079D8F37".ToBytes(), "1BUN89fNJ5re89HsoE45EsqTYP7NpnNiMt"},            new object[] {"0092ED8B99FA31F1D9B95811BC46591FF4A5E5465320CF0D3E".ToBytes(), "1EPtDwq1JzPVC5QmkJgzt52t84ox2iaNPw"},            new object[] {"0013E945F6293C717D0F8C2ACE6F0A787FFA2BD378DE448867".ToBytes(), "12pHHkuKTvHhDWfVTjAXYJ5eevehwsig62"},            new object[] {"00066C0B8995C7464E89F6760900EA6978DF18157388421561".ToBytes(), "1axVFjCkMWDFCHjQHf99AsszXTuzxLxxg"},            new object[] {"00806CD4DA88637EBC986E50F7055BC2D7D3B9D795DE06342D".ToBytes(), "1Ci3sJCziZVp5upNMWM1YPK7w2g3hyTB6L"},            new object[] {"00A2AEE0CAA44720194D1AC0CBE8122AE2E60F2CA75167D5CD".ToBytes(), "1FqBxpXQBzwLt653CP1iawqw8zSFmbBsW4"},            new object[] {"00594525A547114F148B478A81799C929ED3B093F5CAE0B494".ToBytes(), "1991yVg8QC5YVZP8b9eXUEVyQK2w8Bnrjd"},            new object[] {"00577FF59B067A4CB8F8568A71C2854BB6A2F5401D3DE90BBB".ToBytes(), "18yf5TuteMFxMQ8sL3gDL3heNw8E1BNFPY"},            new object[] {"00A5ED461BA0DC730C86BED50CD2E349B75060D10D4D35D9A0".ToBytes(), "1G8LjjGaQi1af22v6w1LoshS8RU2i7BfcB"},            new object[] {"006D3A1066DB55B3C60D9377EC06C8ADB289B17BED44221FAD".ToBytes(), "1AxYDGmJREWVqiQ9ByFdfMMrx7Nmt5vyDn"},            new object[] {"005BC8F038248421B30E6E6623CA6B02D74240E84445D91A32".ToBytes(), "19NKCu73shKmkUvEn2FQiQbQm8vqX7gKXf"},            new object[] {"00626C03CA222E4C80C6AB17FE998AE7B1ECDC614635AF1E25".ToBytes(), "19yQcoNR5Qd5K9hqsttpNZGVWdDZ9qpjNc"},            new object[] {"00169EBCCE898C48B5C00C18936176E6EC3272428C5FEEFDA5".ToBytes(), "134c2VpoHLvP4keud1o19cDxa2o3j8HR7z"},            new object[] {"0046D3CBA0FD26896F8F48F1BA3B9D60891B56CBE582C4CE6D".ToBytes(), "17TW2EUKsGaFtCmH7qQ7tm9wkhwxNbJWS8"},            new object[] {"00A6CF233CAE76EFB882A61EA64C2603FD3DA09569A2E22885".ToBytes(), "1GD1K29KMPJVVxgrQdk5LoojP3eS2ewuh6"},            new object[] {"0018DAD6B70ED6A04EF95C495680A25C487C4730AE60A72CCA".ToBytes(), "13GRNnZSopMpRiPM5zF2zcLF3No1T3coLm"},            new object[] {"001D781AC1CE8810959A266DC9B8B7D4C827BC13B33D64818A".ToBytes(), "13gpUa9EUGQ1yCqMaoMCkUT2n2RQHXjSkR"},            new object[] {"009308239688F8207B2DE68A60E5E0E7AB0BDC4E2E2310CD87".ToBytes(), "1EQS5i433M1qcLXjLfsc6QepF4m3F6UavS"},            new object[] {"00216638B2991F207396B3260850AFA09DDF735DA2D9D26B74".ToBytes(), "143bkoaLqFZiYco2qq7DuQ6KdgzpktJoqd"},            new object[] {"005F0A37BFA06A585FB757BA8AC97D2E4F75F81642B8285738".ToBytes(), "19fXSAgFDFfBRKXkUfpq5KDLRAbAJHKiDD"},            new object[] {"00E353C6B07DDA0F8B090528224975AEBC6731A388137063C7".ToBytes(), "1MizkQxgEnxobrqvCapigaGTDgcHcojwFt"},            new object[] {"00C915743E0414524144290104B97C1A7D986224604B8CCD66".ToBytes(), "1KLEXDMGKdGtofsgrxkKzKWZAhnbzLmFWm"},            new object[] {"009DC1F34CE031CE342B4D9975300E584B068A78BE88B7DC79".ToBytes(), "1FP9S2ZJrkH8aC8cvDCbAfdP3waRKhjwmS"},            new object[] {"0042DCFA01FF087324C3D8905DE2E9C861D6417B1E70F11248".ToBytes(), "176YKPFcmkGY6yKyTVPTvDKq9QJZHm4v5u"},            new object[] {"00B768BE416DFD0A2C677662981D4BFBEE2FFDDBE033B2B517".ToBytes(), "1Hin8nmZRVPvg8DLqUcmYyXRPC7KYZPBsC"},            new object[] {"00FDEA19A53C34647A6D3717F8B7175F887339095AB388B1E0".ToBytes(), "1Q9aQ6ieKu6TXbY6qZBkC94FuzyHdPm8vb"},            new object[] {"00B557AF58BECDCA88E850F7FCE53F892A065C47BB36F7ED93".ToBytes(), "1HXrMBLnFHTzzaNjzZvx4RxcWUtw4523mC"},            new object[] {"004A6295042BD40907C37DF5B8D3954F8D4FF664E17260F134".ToBytes(), "17nK6oNULuoJ4JeNq6AUC8AAg3eNdu92QT"},            new object[] {"00EA14A277F1B153C1DCB6FE0398D0C50ECE82F4E77DF87D70".ToBytes(), "1NLhqcHkbBSiWz46qB48LyV8uqN2ks96Mu"},            new object[] {"00D285891CFB0535BCD6C22F529C68C5CB6666B9CDE3668668".ToBytes(), "1LC8sW2SKeAY4yEN5dSipi9s6xxcK2HYzK"},            new object[] {"0023A330ABCF3401FDA55FA55038CC8A83112ADFE3D6DF2758".ToBytes(), "14FS9N2Rqfc3jDfZbuZ7FXBewEh8i3TGxw"},            new object[] {"004CEB9C62F7683B0C2F34174F5A72425CD0334618BB0901A8".ToBytes(), "181ic7QQ87gaeHBguQVXTrV5PggL8DwhdH"},            new object[] {"00DBDBC96E594A9971E6A9E11CAE63599970A2BE3387551C11".ToBytes(), "1M3WH4xWzR3wPqNiPNes2q1f8nsZabd7kU"},            new object[] {"0073F4F8FDC294A60B8221D975C6FFED352D6EF39B4D18681B".ToBytes(), "1Ba8B4HfYBzt9KiMqMYU9TR9Lq45yzFxzv"},            new object[] {"00F3A2EB49A0126D627BBA601DF40CCCB64622A64F08697F16".ToBytes(), "1PDENS3TqMob3C74JTE38hzrTdBFgTxgxD"},            new object[] {"0099A07C6E98015C1A697BC77F66FD39918E73785EE5F85179".ToBytes(), "1F1Je7o9EQwWgwT9tj7TgRWQjMosrVH3uA"},            new object[] {"00296E9647426DD41793F7493002E10A8FAEDE8D5312FBBDFD".ToBytes(), "14n5Bb16wGDbQUxwt3BLfhESWhhBkhNLmr"},            new object[] {"00D355A8D25F4A11B1CDC84C10CA3B1A72CEC0B3346DFF3EE9".ToBytes(), "1LGSCCbGYHWigLoPvCKABMNhHaBWXWuWXS"},            new object[] {"009C62F60CFEAE9D3191683284580FA8C428E0AAC5CA6DCFD0".ToBytes(), "1FFtxsDZbGYy81z5fTtjRp5CwpqisSSLhV"},            new object[] {"00DC6B63B26C091D95DD425BC4364B4675D90A57CCBEE21DFC".ToBytes(), "1M6UJkcKtMSS2fkQStuJMK8G4FTbksNXm5"},            new object[] {"00481BB6F9970DF701A740B0FCB21167B69FAE5096C51E0FCC".ToBytes(), "17aGrU2xAi1ZXxjGwCtSYMrj55RdY2VwiT"},            new object[] {"0059265249A1E3EA920E4E06121CE7D8DD645B5AC6E686EF6E".ToBytes(), "198P3hC8GaTYvxjLVKmkP4GeKKGVCtNH6u"},            new object[] {"00EB96CE905D65F30522CD0AE781174DDEFBD814D4E8B60DF4".ToBytes(), "1NUgTKwddyHSjCWAwdv9p6tien5LdBPohV"},            new object[] {"00311693C6C8F52765561CA55F9A02F79C020C93BEAC42AB1A".ToBytes(), "15UZA6VRzFAL9zzWq3XDn4ffaEW1ZnnbJZ"},            new object[] {"004CBD61A95BE05E28CEB6825334AF3A14B2DAF469F386FEE5".ToBytes(), "17zmE35GixgEnee1W7cL4hYBiHfDC6FgzY"},            new object[] {"005D5055A3F9651A5CBF3BBA87FE78BE8041333AF510659B63".ToBytes(), "19WQ5aj2FMvWPfZ1Np9HqVw5LFie8BXD6a"},            new object[] {"0086BA80A266D2F66E44A69EFEA5F513DCE84D2F7F09A3BDC5".ToBytes(), "1DHNy8EFMdTYmY4swL4ET6HsdX5S7pbQnQ"},            new object[] {"0042388BE358449C334DC2743B919ACE957068C44A0EBF7FCF".ToBytes(), "1739LaRGvqKCSWmmiWXjjuTKSbjRePHbv6"},            new object[] {"008138F4B6D62066DE9637817D36B80C6F8D29572A723B753A".ToBytes(), "1CnGQ7XLUtaSZ8YqMC3Go2NiMvJtatPDmK"},            new object[] {"00BD21AD78AB0730F8290AE15187B42BE6D0B2BCF7FA8D072C".ToBytes(), "1JF34Dey5zTQRQo3hckJnwEJKRNMs1z1j1"},            new object[] {"0030F80BC8BBC465EB78F012AE98B5952CDCC8F5AA799EB50E".ToBytes(), "15TvakQEGf6pnvUYr5oWWCuXBYLVEPrF7K"},            new object[] {"00FDDE6A534D657DF2F9770D0E6E562D6A4754802ABC010026".ToBytes(), "1Q9LQDqmvmPGGxWofMFqVyKACzsrDK21zu"},            new object[] {"009C2E4CC5445D078A73D2DCD96C887A0AFFC818C37E3DD988".ToBytes(), "1FEosu5j5xnVhT2Z5UhhSDkFg4Eo73cfSo"},            new object[] {"00E6630D42BE63EB7C113C49E35AA01734696BF3B9A1796BCA".ToBytes(), "1N1B5MvNKjkJoJJnnWEWUf5ZNrXnYVX1b7"},            new object[] {"00BD65A9A0334AC2D0B938D306C6E70436E68B9D2A9F5AD136".ToBytes(), "1JGSVu3TFzG5aRHRJ42vMbXLEbsQnf7S7f"},            new object[] {"00E23583535EED6C6357E1D75F357A32F6F7D376D6E067AE8F".ToBytes(), "1Md5pW5vt8oojMzqWkNTRoA4jPtP5aTGyc"},            new object[] {"0073821DC6D8FD9674836F175C9299CD5B2992A3F3422D83E6".ToBytes(), "1BXkahdmviiWTVCyBPFH3chVbVF3PVVXL1"},            new object[] {"00D98AC92610EC6ACA0B40C1546BCED67296C7D700056353B2".ToBytes(), "1LqFtdVrdWsqwGkr6qbh8LuX1ktnTJdXbP"},            new object[] {"009C22A12FA7D0AA54AD11CCC54CC509030DA66BC6D79C4E23".ToBytes(), "1FEZu31b22hL6e6HQFthCM6ckMw1mLj7cW"},            new object[] {"00606EC321732057A3D7C62DD8D6B8E090389F89B25168359B".ToBytes(), "19ntZJzv8YvQ2KH8Kgvo2c5agmUZC6vibk"},            new object[] {"00432AD84BBCF97A0A827A291937E7FFF21C29C10E6DC192EC".ToBytes(), "1789bmbVNaCCCXzzqsYS4ZX1EugPxU8AVh"},            new object[] {"004012E11CA9DDCEE85CCA1411BC9CF40B555C27967F6C1743".ToBytes(), "16qns39G7VQRq54j1nNaNsaVBq25Us38Bt"},            new object[] {"009204F202D1B6F50B3CA874D40BF8858E3D1BEA2EABF93C4C".ToBytes(), "1EK5acm8XMa9F79tNPt2TbLeNuDV1bDHPH"},            new object[] {"00C5B11821216BE3A9AABE7797229FEFF417F684DEA066F1B1".ToBytes(), "1K2JGWt5TLppCTtzksQgovDBDWukcBr84Y"},            new object[] {"007180D1411FF9794A82AB30A858CADAAE3149FDCA07063C00".ToBytes(), "1BM9g4sNicWp6ZARz3kLFvX7bVYoxwt4pw"},            new object[] {"00F974296D3BC2F71852DCA4B920B18DECAA81453187AFB6F1".ToBytes(), "1PjzQqBb1ugxoZXXgMqPfVuwjfcC3NmWYg"},            new object[] {"002DD5937F1B024013D67E25EF7468107B5D0F621A95E3C860".ToBytes(), "15BMGAWgmDA1BKXzGVzz57R9sX7GeCRErb"},            new object[] {"00E86AC97AF8BBDBFEA78E3C42D7203962415B4F0C0E568046".ToBytes(), "1NBuhCUGgQtPJveBEJ8UjiEKPbq6gvkRrd"},            new object[] {"00744B4B83A9C07194DCA464A2654D0B5FD9E8B205F0188AF5".ToBytes(), "1BbuaqfjUTDSPNMXQKg2BWWiL2ZxqqMZaU"},            new object[] {"0038017F2CE3CB04F94F552D6361034A60B86E88AC4901F9FF".ToBytes(), "1678djx5jWoV5mBiJjUKcTTxyDfGFA18Nr"},            new object[] {"005E6E2C40D9B24B89186B38970DB98E6650116AFBC20AF6F4".ToBytes(), "19cJVyfFXR8YmLUCaGxBSMy7AVPua94JzF"},            new object[] {"005520E1E2BBE0B2103D44293793B81BE55893E7606E8CC51A".ToBytes(), "18m7q1io9qRX6zM2663gTsDhNnfiLGEsE9"},            new object[] {"00B2143A1A63E11B6F676D9DA9B2F2A38A0EBA52536E3C9B19".ToBytes(), "1HEbWYKYG1R1XbwhCV3KST1DWMSQSLSPqi"},            new object[] {"0091FF610A0133B8EF154A224FFABF5B1D85D5265E6D44CE5A".ToBytes(), "1EJxus5oozeZZ6KatBhQ9a1fmUM57Us95K"},            new object[] {"00498D652344FD760A86853CB75F2AB9A592406A78E28768D7".ToBytes(), "17huiJhwuvADPyXQ9zchokjdp7ch48w64v"},            new object[] {"00AD6997CFD7A9679266926836B26D30C6B6679EEEFA0873B9".ToBytes(), "1GovPvF5c1vpYJcFx8LmydHKeT5viUXewA"},            new object[] {"00EF7CA4B03BA1400CEC173A3F3B6918AFDE950562262F4AC9".ToBytes(), "1NqHpDaBaSKTpxUbYPbT8nqgcnX7HY83E8"},            new object[] {"004002133980DFF9E3BB4FF6F96A43294BC501F35D25ED96E8".ToBytes(), "16qSjTXYtgcGQYefbBJhzhk9Xskhvfy5tB"}
        };
    }
}

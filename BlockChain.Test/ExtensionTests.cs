using BlockChain.Core;
using NUnit.Framework;

namespace BlockChain.Test
{
    [TestFixture]
    public class ExtensionTests
    {
        [Test]
        public void CalculateDifficultyTest()
        {
            Assert.AreEqual(0, "63e1bfa22a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(0, "60e1bfa22a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(0, "63e1bfa22a309803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(0, "6001bfa22a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(0, "63e1bfa22a340803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(0, "60e1bfa22a349803216946f9a6e50faad3da1a2e871e3af06476df6e2076b770".ToBytes().CalculateDifficulty());
            Assert.AreEqual(1, "03e1bfa22a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(2, "00e1bfa22a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(3, "0001bfa22a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(4, "0000bfa22a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(5, "00000fa22a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(6, "000000a22a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(7, "000000022a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
            Assert.AreEqual(8, "000000002a349803216946f9a6e58faad3da1a2e871e3af86476df6e2976b779".ToBytes().CalculateDifficulty());
        }
    }
}

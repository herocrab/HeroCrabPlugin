using HeroCrabPlugin.Core;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Core
{
    [TestFixture]
    public class NetLoggerBufferTests
    {
        [Test]
        public void Write_AddThreeLogEntriesToEntryBuffer_CountIs3()
        {
            var networkLoggerBuffer = new NetLoggerBuffer(1000);
            networkLoggerBuffer.Write(this, "Test1");
            networkLoggerBuffer.Write(this, "Test2");
            networkLoggerBuffer.Write(this, "Test3");

            Assert.That(networkLoggerBuffer.Length, Is.EqualTo(3));
        }

        [Test]
        public void Write_AddTooManyEntriesToEntryBuffer_CountIs1000()
        {
            var networkLoggerBuffer = new NetLoggerBuffer(1000);
            for (var i = 0; i < 1005; i++) {
                networkLoggerBuffer.Write(this, i.ToString());
            }

            Assert.That(networkLoggerBuffer.Length, Is.EqualTo(1000));
        }
    }
}

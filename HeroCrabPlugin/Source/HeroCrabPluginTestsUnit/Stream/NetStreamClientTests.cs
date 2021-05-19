using NUnit.Framework;

namespace HeroCrabPlugin.Tests.Unit.Stream
{
    [TestFixture]
    public class NetStreamClientTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetConfig(NetRole.Server));
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }
    }
}

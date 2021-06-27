using HeroCrabPlugin.Core;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Stream
{
    [TestFixture]
    public class NetStreamClientTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }
    }
}

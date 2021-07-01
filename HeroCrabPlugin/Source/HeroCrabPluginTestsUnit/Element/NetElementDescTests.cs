// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Element
{
    [TestFixture]
    public class NetElementDescTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Constructor_CreateDescriptionSerializeDeserialize_VerifyPropertiesAreSame()
        {
            var elementDesc = new NetElementDesc(int.MaxValue, "Testing...", uint.MaxValue, uint.MaxValue);

            var serializedElementDesc = elementDesc.Serialize();
            var rxQueue = new NetByteQueue();
            rxQueue.WriteRaw(serializedElementDesc);

            var rxElementDesc = NetElementDesc.Deserialize(rxQueue);

            Assert.That(rxElementDesc.Id, Is.EqualTo(int.MaxValue));
            Assert.That(rxElementDesc.Name, Is.EqualTo("Testing..."));
            Assert.That(rxElementDesc.AuthorId, Is.EqualTo(uint.MaxValue));
            Assert.That(rxElementDesc.AssetId, Is.EqualTo(uint.MaxValue));
        }

        [Test]
        public void Constructor_CreateDescriptionSerializeDeserializeCreateElement_VerifyDescription()
        {
            var elementDesc = new NetElementDesc(int.MaxValue, "Testing...", uint.MaxValue, uint.MaxValue);

            var serializedElementDesc = elementDesc.Serialize();
            var rxQueue = new NetByteQueue();
            rxQueue.WriteRaw(serializedElementDesc);

            var rxElementDesc = NetElementDesc.Deserialize(rxQueue);
            var rxElement = new NetElement(rxElementDesc);

            Assert.That(rxElement.Description, Is.EqualTo(rxElementDesc));
        }
    }
}

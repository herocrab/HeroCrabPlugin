// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Field;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Field
{
    [TestFixture]
    public class NetFIeldDescTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Constructor_CreateDescriptionSerializeDeserialize_CompareDescPropertiesAreSame()
        {
            var count = 0;
            var lastValue = string.Empty;

            void Callback(string value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldString(byte.MaxValue, "Test", false, Callback);
            field.Set("Jeremy");
            field.Process();

            var bytes = field.Description.Serialize();
            var byteQueue = new NetByteQueue();
            byteQueue.WriteRaw(bytes);

            var receivedDesc = NetFieldDesc.Deserialize(byteQueue);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(lastValue, Is.EqualTo("Jeremy"));
            Assert.That(receivedDesc.Index, Is.EqualTo(byte.MaxValue));
            Assert.That(receivedDesc.Name, Is.EqualTo("Test"));
            Assert.That(receivedDesc.Type, Is.EqualTo(NetFieldDesc.TypeCode.String));
        }

        [Test]
        public void CreateElement_CreateADescSerializeItUnserializeAndCreateElement_VerifyCreatedElementFields()
        {
            var elementDesc = new NetElementDesc(uint.MaxValue, "Test", uint.MaxValue, uint.MaxValue);
            var serializedDesc = elementDesc.Serialize();

            var rxQueue = new NetByteQueue();
            rxQueue.WriteRaw(serializedDesc);

            var deserializedElementDesc = NetElementDesc.Deserialize(rxQueue);
            var element = new NetElement(deserializedElementDesc);

            Assert.That(element.Description.Id, Is.EqualTo(uint.MaxValue));
            Assert.That(element.Description.Name, Is.EqualTo("Test"));
        }

        [Test]
        public void Deserialze_CreateElementWithFieldsSerializeDescCreateANewElementFromDesc_VerifyElementHasFields()
        {
            void Callback(byte value)
            {
            }

            var initialDesc = new NetElementDesc(uint.MaxValue, "Test", uint.MaxValue, uint.MaxValue);
            var initialElement = new NetElement(initialDesc);
            initialElement.AddByte("Byte1", false);
            initialElement.AddByte("Byte2", false);

            var serializedDesc = initialElement.Description.Serialize();

            var rxQueue = new NetByteQueue();
            rxQueue.WriteRaw(serializedDesc);

            var deserializedElementDesc = NetElementDesc.Deserialize(rxQueue);
            var element = new NetElement(deserializedElementDesc);
            var setA = element.SetActionByte("Byte1", Callback);
            var setB = element.SetActionByte("Byte2", Callback);

            Assert.That(element.FieldCount, Is.EqualTo(2));
            Assert.That(setA, Is.True);
            Assert.That(setB, Is.True);
        }
    }
}

using System;
using System.Collections.Generic;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Field;
using HeroCrabPlugin.Stream;
using NUnit.Framework; // ReSharper disable ObjectCreationAsStatement

namespace HeroCrabPluginTestsUnit.Element
{
    [TestFixture]
    public class NetElementTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Constructor_SetupElementAddFieldsAndCount_VerifyFieldCountA()
        {
            var stream = new NetStreamServer();

            var element = stream.CreateElement("Player", 0);
            element.AddByte("Byte", false, null);
            element.AddBytes("Bytes", false, null);
            element.AddFloat("Float", false, null);
            element.AddInt("Int", false, null);
            element.AddLong("Long", false, null);
            element.AddString("String", false, null);
            element.AddUInt("UInt", false, null);
            element.AddUShort("UShort", false, null);

            // The order here does not matter, this is just for testing
            Assert.That(element.FieldCount, Is.EqualTo(8));
        }

        [Test]
        public void Constructor_SetupElementAddAllFieldsTwice_VerifyFieldCount()
        {
            var stream = new NetStreamServer();

            var element = stream.CreateElement("Player", 0);
            element.AddByte("Byte", false, null);
            element.AddBytes("Bytes", false, null);
            element.AddFloat("Float", false, null);
            element.AddInt("Int", false, null);
            element.AddLong("Long", false, null);
            element.AddString("String", false, null);
            element.AddUInt("UInt", false, null);
            element.AddUShort("UShort", false, null);

            element.AddByte("Byte", false, null);
            element.AddBytes("Bytes", false, null);
            element.AddFloat("Float", false, null);
            element.AddInt("Int", false, null);
            element.AddLong("Long", false, null);
            element.AddString("String", false, null);
            element.AddUInt("UInt", false, null);
            element.AddUShort("UShort", false, null);

            // The order here does not matter, this is just for testing
            Assert.That(element.FieldCount, Is.EqualTo(8));
        }

        [Test]
        public void IsReliable_SetupElementAndReliableField_VerifyIsReliableIsTrue()
        {
            var stream = new NetStreamServer();

            var element = stream.CreateElement("Player", 0);
            element.AddByte("Byte", true, null);

            Assert.That(element.IsReliable, Is.EqualTo(true));
        }

        [Test]
        public void Constructor_CreateNetElementAddAFieldSerializeAndDeserialize_VerifyFieldIsUpdated()
        {
            var count = 0;
            var lastValue = byte.MinValue;
            void Callback(byte value)
            {
                count++;
                lastValue = value;
            }

            var elementDesc = new NetElementDesc(uint.MaxValue,"Test", uint.MaxValue, uint.MaxValue);

            var sendingElement = new NetElement(elementDesc);
            var byteField = sendingElement.AddByte("Byte", false);
            byteField.Set(byte.MaxValue);
            byteField.Set(byte.MaxValue / 2);

            sendingElement.PrepareDelta();
            var serializedElement = sendingElement.Serialize();

            // Simulate network
            var rxQueue = new NetByteQueue();
            rxQueue.WriteRaw(serializedElement);

            // Creation was handled by "add" section of stream (Delete/Add/Modify)
            var receivingElement = new NetElement(elementDesc);
            var setA = receivingElement.SetActionByte("Byte", Callback);

            // Routing to the element would strip off the index
            rxQueue.ReadUInt();

            receivingElement.Apply(rxQueue);
            receivingElement.Process();
            receivingElement.Process();

            Assert.That(receivingElement.Description.AuthorId, Is.EqualTo(uint.MaxValue));
            Assert.That(receivingElement.FieldCount, Is.EqualTo(1));
            Assert.That(setA, Is.True);
            Assert.That(count, Is.EqualTo(2));
            Assert.That(lastValue, Is.EqualTo(byte.MaxValue / 2));
        }

        [Test]
        public void SetFieldAction_CreateNetElementWithAllFieldsSerializeDeserialize_VerifySetFieldActionTrueAll()
        {
            var elementDesc = new NetElementDesc(uint.MaxValue,"Test", uint.MaxValue, uint.MaxValue);

            var sendingElement = new NetElement(elementDesc);
            sendingElement.AddByte("Byte", false);
            sendingElement.AddBytes("Bytes", false);
            sendingElement.AddFloat("Float", false);
            sendingElement.AddInt("Int", false);
            sendingElement.AddLong("Long", false);
            sendingElement.AddString("String", false);
            sendingElement.AddUInt("UInt", false);
            sendingElement.AddUShort("UShort", false);

            sendingElement.PrepareDelta();
            var serializedElement = sendingElement.Serialize();

            // Simulate network
            var rxQueue = new NetByteQueue();
            rxQueue.WriteRaw(serializedElement);

            // Creation was handled by "add" section of stream (Delete/Add/Modify)
            var receivingElement = new NetElement(elementDesc);
            var byteFieldSet = receivingElement.SetActionByte("Byte", delegate { });
            var bytesFieldSet = receivingElement.SetActionBytes("Bytes", delegate { });
            var floatFieldSet = receivingElement.SetActionFloat("Float", delegate { });
            var intFieldSet = receivingElement.SetActionInt("Int", delegate { });
            var longFieldSet = receivingElement.SetActionLong("Long", delegate { });
            var stringFieldSet = receivingElement.SetActionString("String", delegate { });
            var uintFieldSet = receivingElement.SetActionUInt("UInt", delegate { });
            var ushortFieldSet = receivingElement.SetActionUShort("UShort", delegate { });

            Assert.That(byteFieldSet, Is.True);
            Assert.That(bytesFieldSet, Is.True);
            Assert.That(floatFieldSet, Is.True);
            Assert.That(intFieldSet, Is.True);
            Assert.That(longFieldSet, Is.True);
            Assert.That(stringFieldSet, Is.True);
            Assert.That(uintFieldSet, Is.True);
            Assert.That(ushortFieldSet, Is.True);
        }

        [Test]
        public void SetFieldAction_CreateNetElementWithNoFieldsSerializeDeserializeAttemptToSetField_VerifySetFieldActionFalseAll()
        {
            var elementDesc = new NetElementDesc(uint.MaxValue,"Test", uint.MaxValue, uint.MaxValue);
            var sendingElement = new NetElement(elementDesc);

            sendingElement.PrepareDelta();
            var serializedElement = sendingElement.Serialize();

            // Simulate network
            var rxQueue = new NetByteQueue();
            rxQueue.WriteRaw(serializedElement);

            // Creation was handled by "add" section of stream (Delete/Add/Modify)
            var receivingElement = new NetElement(elementDesc);
            var byteFieldSet = receivingElement.SetActionByte("Byte", delegate { });
            var bytesFieldSet = receivingElement.SetActionBytes("Bytes", delegate { });
            var floatFieldSet = receivingElement.SetActionFloat("Float", delegate { });
            var intFieldSet = receivingElement.SetActionInt("Int", delegate { });
            var longFieldSet = receivingElement.SetActionLong("Long", delegate { });
            var stringFieldSet = receivingElement.SetActionString("String", delegate { });
            var uintFieldSet = receivingElement.SetActionUInt("UInt", delegate { });
            var ushortFieldSet = receivingElement.SetActionUShort("UShort", delegate { });

            Assert.That(byteFieldSet, Is.False);
            Assert.That(bytesFieldSet, Is.False);
            Assert.That(floatFieldSet, Is.False);
            Assert.That(intFieldSet, Is.False);
            Assert.That(longFieldSet, Is.False);
            Assert.That(stringFieldSet, Is.False);
            Assert.That(uintFieldSet, Is.False);
            Assert.That(ushortFieldSet, Is.False);
        }

        [Test]
        public void SetFieldAction_CreateNetElementWithAllFieldsSerializeDeserializeAttemptToSetInaccurately_VerifySetFieldActionFalseAll()
        {
            var elementDesc = new NetElementDesc(uint.MaxValue,"Test", uint.MaxValue, uint.MaxValue);
            var sendingElement = new NetElement(elementDesc);
            sendingElement.AddByte("Byte", false);
            sendingElement.AddBytes("Bytes", false);
            sendingElement.AddFloat("Float", false);
            sendingElement.AddInt("Int", false);
            sendingElement.AddLong("Long", false);
            sendingElement.AddString("String", false);
            sendingElement.AddUInt("UInt", false);
            sendingElement.AddUShort("UShort", false);

            sendingElement.PrepareDelta();
            var serializedElement = sendingElement.Serialize();

            // Simulate network
            var rxQueue = new NetByteQueue();
            rxQueue.WriteRaw(serializedElement);

            // Creation was handled by "add" section of stream (Delete/Add/Modify)
            var receivingElement = new NetElement(elementDesc);

            // Attempt to set a field action incorrectly
            var byteFieldSet = receivingElement.SetActionByte("Bytes", delegate { });
            var bytesFieldSet = receivingElement.SetActionBytes("Byte", delegate { });
            var floatFieldSet = receivingElement.SetActionFloat("Int", delegate { });
            var intFieldSet = receivingElement.SetActionInt("Float", delegate { });
            var longFieldSet = receivingElement.SetActionLong("String", delegate { });
            var stringFieldSet = receivingElement.SetActionString("Long", delegate { });
            var uintFieldSet = receivingElement.SetActionUInt("UShort", delegate { });
            var ushortFieldSet = receivingElement.SetActionUShort("UInt", delegate { });

            Assert.That(byteFieldSet, Is.False);
            Assert.That(bytesFieldSet, Is.False);
            Assert.That(floatFieldSet, Is.False);
            Assert.That(intFieldSet, Is.False);
            Assert.That(longFieldSet, Is.False);
            Assert.That(stringFieldSet, Is.False);
            Assert.That(uintFieldSet, Is.False);
            Assert.That(ushortFieldSet, Is.False);
        }

        [Test]
        public void Constructor_CreateNetElementWithInvalidTypeCodeForField_ThrowsException()
        {
            var brokenLedger = new SortedDictionary<byte, NetFieldDesc>
            {
                {byte.MaxValue, new NetFieldDesc(byte.MaxValue, "Fail", false, NetFieldDesc.TypeCode.Unknown)}
            };

            var elementDesc = new NetElementDesc(uint.MaxValue, "Test", uint.MaxValue, uint.MaxValue);
            elementDesc.WriteLedger(brokenLedger);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new NetElement(elementDesc);
            });
        }

        [Test]
        public void Constructor_CreateNetElementAddAFieldSerializeAndDeserializeIntoWrongElement_VerifyNoDamageIsDone()
        {
            void Callback(byte value) { }

            var elementDescTx = new NetElementDesc(uint.MaxValue,"Test", uint.MaxValue, uint.MaxValue);
            var elementDescRx = new NetElementDesc(uint.MaxValue,"Test", uint.MaxValue, uint.MaxValue);

            var sendingElement = new NetElement(elementDescTx);
            var byteField = sendingElement.AddByte("Byte", false);
            byteField.Set(byte.MaxValue);
            byteField.Set(byte.MaxValue / 2);

            sendingElement.PrepareDelta();
            var serializedElement = sendingElement.Serialize();

            // Simulate network
            var rxQueue = new NetByteQueue();
            rxQueue.WriteRaw(serializedElement);

            // Creation was handled by "add" section of stream (Delete/Add/Modify)
            var receivingElement = new NetElement(elementDescRx);
            var setA = receivingElement.SetActionByte("Byte", Callback);

            // Routing to the element would strip off the index
            rxQueue.ReadUInt();

            receivingElement.Apply(rxQueue);
            receivingElement.Process();
            receivingElement.Process();

            Assert.That(receivingElement.FieldCount, Is.EqualTo(0));
            Assert.That(setA, Is.False);
        }

        [Test]
        public void Get_CreateNetElementWithAllFields_VerifyGetWorks()
        {
            var elementDesc = new NetElementDesc(uint.MaxValue,"Test", uint.MaxValue, uint.MaxValue);
            var sendingElement = new NetElement(elementDesc);
            sendingElement.AddByte("Byte", false);
            sendingElement.AddBytes("Bytes", false);
            sendingElement.AddFloat("Float", false);
            sendingElement.AddInt("Int", false);
            sendingElement.AddLong("Long", false);
            sendingElement.AddString("String", false);
            sendingElement.AddUInt("UInt", false);
            sendingElement.AddUShort("UShort", false);

            var receivingElement = new NetElement(elementDesc);

            var byteFieldSet = receivingElement.GetByte("Byte");
            var bytesFieldSet = receivingElement.GetBytes("Bytes");
            var floatFieldSet = receivingElement.GetFloat("Float");
            var intFieldSet = receivingElement.GetInt("Int");
            var longFieldSet = receivingElement.GetLong("Long");
            var stringFieldSet = receivingElement.GetString("String");
            var uintFieldSet = receivingElement.GetUInt("UInt");
            var ushortFieldSet = receivingElement.GetUShort("UShort");

            Assert.That(byteFieldSet, Is.Not.Null);
            Assert.That(bytesFieldSet, Is.Not.Null);
            Assert.That(floatFieldSet, Is.Not.Null);
            Assert.That(intFieldSet, Is.Not.Null);
            Assert.That(longFieldSet, Is.Not.Null);
            Assert.That(stringFieldSet, Is.Not.Null);
            Assert.That(uintFieldSet, Is.Not.Null);
            Assert.That(ushortFieldSet, Is.Not.Null);
        }

        [Test]
        public void Get_CreateNetElementWithAllFieldsTryToRetrieveIncorrectly_VerifyGetIsNull()
        {
            var elementDesc = new NetElementDesc(uint.MaxValue,"Test", uint.MaxValue, uint.MaxValue);
            var sendingElement = new NetElement(elementDesc);
            sendingElement.AddByte("Byte", false);
            sendingElement.AddBytes("Bytes", false);
            sendingElement.AddFloat("Float", false);
            sendingElement.AddInt("Int", false);
            sendingElement.AddLong("Long", false);
            sendingElement.AddString("String", false);
            sendingElement.AddUInt("UInt", false);
            sendingElement.AddUShort("UShort", false);

            var receivingElement = new NetElement(elementDesc);

            var byteFieldSet = receivingElement.GetByte("1Byte");
            var bytesFieldSet = receivingElement.GetBytes("1Bytes");
            var floatFieldSet = receivingElement.GetFloat("1Float");
            var intFieldSet = receivingElement.GetInt("1Int");
            var longFieldSet = receivingElement.GetLong("1Long");
            var stringFieldSet = receivingElement.GetString("1String");
            var uintFieldSet = receivingElement.GetUInt("1UInt");
            var ushortFieldSet = receivingElement.GetUShort("1UShort");

            Assert.That(byteFieldSet, Is.Null);
            Assert.That(bytesFieldSet, Is.Null);
            Assert.That(floatFieldSet, Is.Null);
            Assert.That(intFieldSet, Is.Null);
            Assert.That(longFieldSet, Is.Null);
            Assert.That(stringFieldSet, Is.Null);
            Assert.That(uintFieldSet, Is.Null);
            Assert.That(ushortFieldSet, Is.Null);
        }
    }
}

// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using HeroCrabPlugin.Core;
using HeroCrabPlugin.Field;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Field
{
    [TestFixture]
    public class NetFieldStringTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Set_ConstructAndSetValueThenProcess_VerifyCountAndValueIsCorrect()
        {
            var count = 0;
            var lastValue = string.Empty;

            void Callback(string value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldString(0, "Test", false, Callback);
            field.Set("Test string...");
            field.Process();

            Assert.That(count, Is.EqualTo(1));
            Assert.That(lastValue, Is.EqualTo("Test string..."));
        }

        [Test]
        public void Serialize_SerializeAndDeserialize_CompareResultsAreEqual()
        {
            var count = 0;
            var lastValue = string.Empty;

            void Callback(string value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldString(0, "Test", false);
            field.Set("Test string...");

            var serializedBytes = field.Serialize();
            var receivingQueue = new NetByteQueue();
            receivingQueue.WriteRaw(serializedBytes);

            var receivingField = new NetFieldString(field.Description, Callback);
            receivingField.Deserialize(receivingQueue);
            receivingField.Process();

            Assert.That(count, Is.EqualTo(1));
            Assert.That(lastValue, Is.EqualTo("Test string..."));
        }

        [Test]
        public void Serialize_SetFieldThreeTimesSerializeAndDeserialize_CompareTheCountAndLastResult()
        {
            var count = 0;
            var lastValue = string.Empty;

            void Callback(string value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldString(0, "Test", false);
            field.Set("Test string...");
            field.Set("0");
            field.Set("1");

            var serializedBytes = field.Serialize();
            var receivingQueue = new NetByteQueue();
            receivingQueue.WriteRaw(serializedBytes);

            var receivingField = new NetFieldString(field.Description, Callback);
            receivingField.Deserialize(receivingQueue);
            receivingField.Process();
            receivingField.Process();
            receivingField.Process();

            Assert.That(count, Is.EqualTo(3));
            Assert.That(lastValue, Is.EqualTo("1"));
        }
    }
}

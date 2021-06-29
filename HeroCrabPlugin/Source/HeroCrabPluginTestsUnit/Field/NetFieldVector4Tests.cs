using FlaxEngine;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Field;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Field
{
    [TestFixture]
    public class NetFieldVector4Tests
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
            var lastValue = Vector4.Zero;
            void Callback(Vector4 value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldVector4(0, "Test", false, Callback);
            field.Set(Vector4.Maximum);
            field.Process();

            Assert.That(count, Is.EqualTo(1));
            Assert.That(lastValue, Is.EqualTo(Vector4.Maximum));
        }

        [Test]
        public void Serialize_SerializeAndDeserialize_CompareResultsAreEqual()
        {
            var count = 0;
            var lastValue = Vector4.Zero;
            void Callback(Vector4 value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldVector4(0, "Test", false);
            field.Set(Vector4.Maximum);

            var serializedBytes = field.Serialize();
            var receivingQueue = new NetByteQueue();
            receivingQueue.WriteRaw(serializedBytes);

            var receivingField = new NetFieldVector4(field.Description, Callback);
            receivingField.Deserialize(receivingQueue);
            receivingField.Process();

            Assert.That(count, Is.EqualTo(1));
            Assert.That(lastValue, Is.EqualTo(Vector4.Maximum));
        }

        [Test]
        public void Serialize_SetFieldThreeTimesSerializeAndDeserialize_CompareTheCountAndLastResult()
        {
            var count = 0;
            var lastValue = Vector4.Zero;
            void Callback(Vector4 value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldVector4(0, "Test", false);
            field.Set(Vector4.Maximum);
            field.Set(Vector4.Zero);
            field.Set(Vector4.One);

            var serializedBytes = field.Serialize();
            var receivingQueue = new NetByteQueue();
            receivingQueue.WriteRaw(serializedBytes);

            var receivingField = new NetFieldVector4(field.Description, Callback);
            receivingField.Deserialize(receivingQueue);
            receivingField.Process();
            receivingField.Process();
            receivingField.Process();

            Assert.That(count, Is.EqualTo(3));
            Assert.That(lastValue, Is.EqualTo(Vector4.One));
        }
    }
}

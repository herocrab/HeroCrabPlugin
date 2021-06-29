using FlaxEngine;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Field;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Field
{
    [TestFixture]
    public class NetFieldQuaternionTests
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
            var lastValue = Quaternion.Zero;
            void Callback(Quaternion value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldQuaternion(0, "Test", false, Callback);
            field.Set(Quaternion.One);
            field.Process();

            Assert.That(count, Is.EqualTo(1));
            Assert.That(lastValue, Is.EqualTo(Quaternion.One));
        }

        [Test]
        public void Serialize_SerializeAndDeserialize_CompareResultsAreEqual()
        {
            var count = 0;
            var lastValue = Quaternion.Zero;
            void Callback(Quaternion value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldQuaternion(0, "Test", false);
            field.Set(Quaternion.One);

            var serializedBytes = field.Serialize();
            var receivingQueue = new NetByteQueue();
            receivingQueue.WriteRaw(serializedBytes);

            var receivingField = new NetFieldQuaternion(field.Description, Callback);
            receivingField.Deserialize(receivingQueue);
            receivingField.Process();

            Assert.That(count, Is.EqualTo(1));
            Assert.That(lastValue, Is.EqualTo(Quaternion.One));
        }

        [Test]
        public void Serialize_SetFieldThreeTimesSerializeAndDeserialize_CompareTheCountAndLastResult()
        {
            var count = 0;
            var lastValue = Quaternion.Zero;
            void Callback(Quaternion value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldQuaternion(0, "Test", false);
            field.Set(Quaternion.One);
            field.Set(Quaternion.Zero);
            field.Set(Quaternion.One);

            var serializedBytes = field.Serialize();
            var receivingQueue = new NetByteQueue();
            receivingQueue.WriteRaw(serializedBytes);

            var receivingField = new NetFieldQuaternion(field.Description, Callback);
            receivingField.Deserialize(receivingQueue);
            receivingField.Process();
            receivingField.Process();
            receivingField.Process();

            Assert.That(count, Is.EqualTo(3));
            Assert.That(lastValue, Is.EqualTo(Quaternion.One));
        }
    }
}

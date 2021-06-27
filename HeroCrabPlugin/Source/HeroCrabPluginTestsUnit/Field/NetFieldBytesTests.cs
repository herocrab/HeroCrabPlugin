using System;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Field;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Field
{
    [TestFixture]
    public class NetFieldBytesTests
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
            var lastValue = new byte[0];
            void Callback(byte[] value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldBytes(0, "Test", false, Callback);
            field.Set(BitConverter.GetBytes(int.MaxValue));
            field.Process();

            Assert.That(count, Is.EqualTo(1));
            Assert.That(lastValue, Is.EqualTo(BitConverter.GetBytes(int.MaxValue)));
        }

        [Test]
        public void Serialize_SerializeAndDeserialize_CompareResultsAreEqual()
        {
            var count = 0;
            var lastValue = new byte[0];
            void Callback(byte[] value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldBytes(0, "Test", false, null);
            field.Set(BitConverter.GetBytes(int.MaxValue));

            var serializedBytes = field.Serialize();
            var receivingQueue = new NetByteQueue();
            receivingQueue.WriteRaw(serializedBytes);

            var receivingField = new NetFieldBytes(field.Description, Callback);
            receivingField.Deserialize(receivingQueue);
            receivingField.Process();

            Assert.That(count, Is.EqualTo(1));
            Assert.That(BitConverter.ToInt32(lastValue.Take(4).ToArray(), 0), Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void Serialize_SetFieldThreeTimesSerializeAndDeserialize_CompareTheCountAndLastResult()
        {
            var count = 0;
            var lastValue = new byte[0];
            void Callback(byte[] value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldBytes(0, "Test", false, null);
            field.Set(new byte[]{0, 1, byte.MaxValue});
            field.Set(new byte[]{1, 2, 3});
            field.Set(new byte[]{2, 3, 4});

            var serializedBytes = field.Serialize();
            var receivingQueue = new NetByteQueue();
            receivingQueue.WriteRaw(serializedBytes);

            var receivingField = new NetFieldBytes(field.Description, Callback);
            receivingField.Deserialize(receivingQueue);
            receivingField.Process();
            receivingField.Process();
            receivingField.Process();

            Assert.That(count, Is.EqualTo(3));
            Assert.That(lastValue.Take(3), Is.EqualTo(new byte[]{2, 3, 4}));
        }
    }
}

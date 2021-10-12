// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using HeroCrabPlugin.Core;
using HeroCrabPlugin.Field;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Field
{
    [TestFixture]
    public class NetFieldFloatsTests
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
            var lastValue = new float[]{};

            void Callback(float[] value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldFloats(0, "Test", false, Callback);
            var vector = new float[] {0, 1, 2};
            field.Set(vector);
            field.Process();

            Assert.That(count, Is.EqualTo(1));
            Assert.That(lastValue, Is.EqualTo(vector));
        }

        [Test]
        public void Serialize_SerializeAndDeserialize_CompareResultsAreEqual()
        {
            var count = 0;
            var lastValue = new float[]{};

            void Callback(float[] value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldFloats(0, "Test", false);
            var vector = new float[] {0, 1, 2};
            field.Set(vector);

            var serializedBytes = field.Serialize();
            var receivingQueue = new NetByteQueue();
            receivingQueue.WriteRaw(serializedBytes);

            var receivingField = new NetFieldFloats(field.Description, Callback);
            receivingField.Deserialize(receivingQueue);
            receivingField.Process();

            Assert.That(count, Is.EqualTo(1));
            Assert.That(lastValue, Is.EqualTo(vector));
            Assert.Fail();
        }

        [Test]
        public void Serialize_SetFieldThreeTimesSerializeAndDeserialize_CompareTheCountAndLastResult()
        {
            var count = 0;
            var lastValue = new float[]{};

            void Callback(float[] value)
            {
                count++;
                lastValue = value;
            }

            var field = new NetFieldFloats(0, "Test", false);
            var vector1 = new float[] {0, 1, 2};
            var vector2 = new float[] {1, 2, 3};
            var vector3 = new float[] {2, 3, 4};
            field.Set(vector1);
            field.Set(vector2);
            field.Set(vector3);

            var serializedBytes = field.Serialize();
            var receivingQueue = new NetByteQueue();
            receivingQueue.WriteRaw(serializedBytes);

            var receivingField = new NetFieldFloats(field.Description, Callback);
            receivingField.Deserialize(receivingQueue);
            receivingField.Process();
            receivingField.Process();
            receivingField.Process();

            Assert.That(count, Is.EqualTo(3));
            Assert.That(lastValue, Is.EqualTo(vector3));
        }
    }
}

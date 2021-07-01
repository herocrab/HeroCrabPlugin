// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Field;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Field
{
    [TestFixture]
    public class NetFieldBufferTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Peek_LoadBuffer_VerifyPeekLast()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var buffer = new NetFieldBuffer<int>(10);
            buffer.Add(0);
            buffer.Add(1);
            buffer.Add(2);

            Assert.That(buffer.Count, Is.EqualTo(3));
            Assert.That(buffer.Peek(), Is.EqualTo(0));
            Assert.That(buffer.Last(), Is.EqualTo(2));
            Assert.That(buffer.Any(), Is.EqualTo(true));
        }

        [Test]
        public void Clear_LoadBuffer_VerifyClear()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var buffer = new NetFieldBuffer<int>(10);
            buffer.Add(0);
            buffer.Add(1);
            buffer.Add(2);
            buffer.Clear();

            Assert.That(buffer.Count, Is.EqualTo(0));
            Assert.That(buffer.Peek(), Is.EqualTo(0));
            Assert.That(buffer.Last(), Is.EqualTo(0));
            Assert.That(buffer.Any(), Is.EqualTo(false));
        }

        [Test]
        public void Dequeue_LoadBuffer_DequeueThreeValuesAndCompare()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var buffer = new NetFieldBuffer<int>(10);
            buffer.Add(0);
            buffer.Add(1);
            buffer.Add(2);

            Assert.That(buffer.Dequeue(), Is.EqualTo(new[] {0, 1, 2}));
        }

        [Test]
        public void Consume_LoadBuffer_ConsumeBufferWithNewBufferVerifyContents()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var buffer = new NetFieldBuffer<int>(10);
            buffer.Add(0);
            buffer.Add(1);
            buffer.Add(2);

            var consumeBuffer = new NetFieldBuffer<int>(10);
            consumeBuffer.Consume(buffer);

            Assert.That(consumeBuffer.Dequeue(), Is.EqualTo(new[] {0, 1, 2}));
        }

        [Test]
        public void Constructor_ProvideOtherBufferAtConstruction_VerifyContents()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var buffer = new NetFieldBuffer<int>(10);
            buffer.Add(0);
            buffer.Add(1);
            buffer.Add(2);

            var otherBuffer = new NetFieldBuffer<int>(buffer);

            Assert.That(otherBuffer.Dequeue(), Is.EqualTo(new[] {0, 1, 2}));
        }

        [Test]
        public void Add_AddAnEntryToAFullQueue_VerifyCountAfterwardsIsLimitedToSize()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var buffer = new NetFieldBuffer<int>(10);
            for (var i = 0; i < 100; i++) {
                buffer.Add(i);
            }

            Assert.That(buffer.Count, Is.EqualTo(10));
        }
    }
}

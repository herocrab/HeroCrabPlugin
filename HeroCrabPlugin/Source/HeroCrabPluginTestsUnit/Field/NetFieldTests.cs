/* Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
Distributed under the MIT license. See the LICENSE.md file in the project root for more information. */
using System;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Field;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Field
{
    [TestFixture]
    public class NetFieldTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Clear_SetValuesAndClear_VerifyLengthAndDepthAre0()
        {
            var field = new NetFieldByte(0, "Test", false);
            field.Set(byte.MaxValue);
            field.Set(0);
            field.Set(1);

            field.Clear();
            var serializedBytes = field.Serialize();

            Assert.That(serializedBytes.Length, Is.EqualTo(1));
        }

        [Test]
        public void Serialize_AttemptToSerializeFieldWithGreaterThan255Entries_ThrowNotSupportedException()
        {
            var field = new NetFieldByte(0, "Test", false);

            for (var i = 0; i < 256; i++) {
                field.Set((byte)i);
            }

            Assert.Throws<NotSupportedException>(() => field.Serialize());
        }
    }
}

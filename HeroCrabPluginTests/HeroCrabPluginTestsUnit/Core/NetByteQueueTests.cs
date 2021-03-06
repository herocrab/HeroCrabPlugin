// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System;
using System.Linq;
using HeroCrabPlugin.Core;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Core
{
    [TestFixture]
    public class NetByteQueueTests
    {
        [Test]
        [TestCase((uint)0, "TestString1")]
        [TestCase((uint)1, "TestString2")]
        [TestCase((uint)3, "TestString3")]
        public void WriteString_WriteTestStringAfterUIntAndTransfer_ReturnsUIntAndTestString(uint a, string b)
        {
            var sendingByteQueue = new NetByteQueue();
            sendingByteQueue.WriteUInt(a);
            sendingByteQueue.WriteString(b);
            var bytes = sendingByteQueue.ToBytes();

            var receivingByteQueue = new NetByteQueue();
            receivingByteQueue.WriteRaw(bytes);

            var receivedUint = receivingByteQueue.ReadUInt();
            var receivedString = receivingByteQueue.ReadString();

            Assert.That(receivedUint, Is.EqualTo(a));
            Assert.That(receivedUint, Is.TypeOf(typeof(uint)));

            Assert.That(receivedString, Is.EqualTo(b));
            Assert.That(receivedString, Is.TypeOf(typeof(string)));
        }

        [Test]
        public void WriteString_WriteTestStringGreaterThanUShortMaxChars_ReturnsTestStringTruncatedAtUshortMaxChars()
        {
            var sendingByteQueue = new NetByteQueue();
            var sentString = new string(new char(), ushort.MaxValue + 1);

            sendingByteQueue.WriteString(sentString);

            var bytes = sendingByteQueue.ToBytes();

            var receivingByteQueue = new NetByteQueue();
            receivingByteQueue.WriteRaw(bytes);

            var receivedString = receivingByteQueue.ReadString();

            Assert.That(receivedString.Length, Is.EqualTo(ushort.MaxValue));
            Assert.That(receivedString, Is.TypeOf(typeof(string)));
        }

        [Test]
        public void WriteUShort_WriteTestUShortTransfer_ReturnsTestUShort()
        {
            var sendingBytes = new NetByteQueue();
            const ushort sentUShort = (ushort)15;

            sendingBytes.WriteUShort(sentUShort);

            var bytes = sendingBytes.ToBytes();

            var receivingByteQueue = new NetByteQueue();
            receivingByteQueue.WriteRaw(bytes);

            var receivedUShort = receivingByteQueue.ReadUShort();

            Assert.That(receivedUShort, Is.EqualTo(sentUShort));
            Assert.That(receivedUShort, Is.TypeOf(typeof(ushort)));
        }

        [Test]
        public void WriteUInt_WriteTestUintTAndTransfer_ReturnsTestUShort()
        {
            var sendingBytes = new NetByteQueue();
            const uint sentUInt = (uint)533;

            sendingBytes.WriteUInt(sentUInt);

            var bytes = sendingBytes.ToBytes();

            var receivingByteQueue = new NetByteQueue();
            receivingByteQueue.WriteRaw(bytes);

            var readUInt = receivingByteQueue.ReadUInt();

            Assert.That(readUInt, Is.EqualTo(sentUInt));
            Assert.That(readUInt, Is.TypeOf(typeof(uint)));
        }

        [Test]
        public void WriteInt_WriteTestIntTAndTransfer_ReturnsTestUShort()
        {
            var sendingBytes = new NetByteQueue();
            const int sentInt = -6715;

            sendingBytes.WriteInt(sentInt);

            var bytes = sendingBytes.ToBytes();

            var receivingByteQueue = new NetByteQueue();
            receivingByteQueue.WriteRaw(bytes);

            var readInt = receivingByteQueue.ReadInt();

            Assert.That(readInt, Is.EqualTo(sentInt));
            Assert.That(readInt, Is.TypeOf(typeof(int)));
        }

        [Test]
        public void WriteFloat_WriteTestFloatAndTransfer_ReturnsTestFloat()
        {
            var sendingBytes = new NetByteQueue();
            const float sentFloat = -0.1f;

            sendingBytes.WriteFloat(sentFloat);

            var bytes = sendingBytes.ToBytes();

            var receivingByteQueue = new NetByteQueue();
            receivingByteQueue.WriteRaw(bytes);

            var receivedFloat = receivingByteQueue.ReadFloat();

            Assert.That(receivedFloat, Is.EqualTo(sentFloat));
            Assert.That(receivedFloat, Is.TypeOf(typeof(float)));
        }

        [Test]
        public void PeekByte_WriteOneByteAndPeek_ReturnsPeekValueCorrectlyAndQueueCountReturnsOne()
        {
            var sendingBytes = new NetByteQueue();
            sendingBytes.WriteByte(35);

            var bytes = sendingBytes.ToBytes();

            var receivingByteQueue = new NetByteQueue();
            receivingByteQueue.WriteRaw(bytes);

            var receivedByte = receivingByteQueue.PeekByte();

            Assert.That(receivedByte, Is.EqualTo(35));
            Assert.That(receivingByteQueue.Length, Is.EqualTo(1));
        }

        [Test]
        public void Clear_AddBytesThenClear_VerifyLengthIsZero()
        {
            var byteQueue = new NetByteQueue();
            byteQueue.WriteString("Testing...");

            byteQueue.Clear();
            Assert.That(byteQueue.Length, Is.EqualTo(0));
        }

        [Test]
        public void WriteBytes_WriteAnArrayOfBytes_RetrieveArrayOfBytes()
        {
            var byteQueue = new NetByteQueue();
            var sentBytes = new byte[] {1, 2, 3, 4};

            byteQueue.WriteBytes(sentBytes);
            var receivedBytes = byteQueue.ReadBytes();

            Assert.That(sentBytes, Is.EqualTo(receivedBytes.Take(4).ToArray()));
        }

        [Test]
        public void WriteLong_WriteALong_RetrieveSameLong()
        {
            var byteQueue = new NetByteQueue();
            const long sentLong = 1;

            byteQueue.WriteLong(sentLong);
            var receivedLong = byteQueue.ReadLong();

            Assert.That(sentLong, Is.EqualTo(receivedLong));
        }

        [Test]
        public void WriteByte_WriteByte_RetrieveSameByte()
        {
            var byteQueue = new NetByteQueue();
            const byte sentByte = 1;

            byteQueue.WriteByte(sentByte);
            var receivedByte = byteQueue.ReadByte();

            Assert.That(sentByte, Is.EqualTo(receivedByte));
        }

        [Test]
        public void WriteByte_WriteByteVerifyAny_AnyReturnsTrue()
        {
            var byteQueue = new NetByteQueue();
            byteQueue.WriteByte(0);

            Assert.That(byteQueue.Any, Is.EqualTo(true));
        }

        [Test]
        public void WriteBytes_WriteBytesSequence_VerifySameBytesSequence()
        {
            var byteQueue = new NetByteQueue();
            byteQueue.WriteBytes(BitConverter.GetBytes(int.MaxValue));

            var bytes = byteQueue.ReadBytes();

            Assert.That(bytes.Take(4).ToArray(), Is.EqualTo(BitConverter.GetBytes(int.MaxValue)));
        }

        [Test]
        public void ReadBytes_WriteTooManYBytes_VerifyReadBytesAreClamped()
        {
            var byteQueue = new NetByteQueue();
            byteQueue.WriteInt(65536);

            for (var i = 0; i < 65536; i++) {
                byteQueue.WriteByte(0);
            }

            var bytes = byteQueue.ReadBytes();
            Assert.That(bytes.Length, Is.EqualTo(0));
        }

        [Test]
        public void WriteVector2_ReadVector2ToArray_VerifyContentsAreSame()
        {
            var writeVector = new float[]{0, 1, 2};
            var byteQueue = new NetByteQueue();
            byteQueue.WriteFloats(writeVector);

            var readVector = byteQueue.ReadFloats();
            Assert.That(readVector, Is.EqualTo(writeVector));
        }

        [Test]
        public void WriteBool_ReadBool_VerifyContentsAreSame()
        {
            const bool isTrue = true;
            const bool isFalse = false;
            var byteQueue = new NetByteQueue();
            byteQueue.WriteBool(isTrue);
            byteQueue.WriteBool(isFalse);

            var resultIsTrue = byteQueue.ReadBool();
            var resultIsFalse = byteQueue.ReadBool();

            Assert.That(resultIsTrue, Is.True);
            Assert.That(resultIsFalse, Is.False);
        }
    }
}

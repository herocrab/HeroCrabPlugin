// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.


using HeroCrabPlugin.Core;
using HeroCrabPlugin.Sublayer.Replay;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Sublayer.Replay
{
    [TestFixture]
    public class NetRecorderTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Start_StartRecordingThenDisconnect_VerifyIsRecordingFalse()
        {
            var recorder = new NetRecorder();
            recorder.Start(0);
            Assert.That(recorder.IsRecording, Is.True);

            recorder.Disconnect();
            Assert.That(recorder.IsRecording, Is.False);
        }

        [Test]
        public void Start_SendDataWithoutCallingStart_VerifyBytesAfterStopZero()
        {
            var recorder = new NetRecorder();
            recorder.SendId(uint.MaxValue);
            recorder.Send(0, new byte[] {0, 1, 2, 3}, false);
            recorder.Send(1, new byte[] {4, 5, 6, 7}, false);
            recorder.Stop();

            var bytes = recorder.Bytes;
            Assert.That(bytes.Length, Is.EqualTo(4)); // Length field is an int
        }

        [Test]
        public void Start_StartTwice_VerifyLogMessage()
        {
            var logCount = 0;
            var logMessage = string.Empty;

            void LogWrite(object sender, string message)
            {
                logCount++;
                logMessage = message;
            }

            var logger = NetServices.Registry.Get<NetLogger>();
            logger.LogWrite += LogWrite;

            var recorder = new NetRecorder();
            recorder.Start(0);
            recorder.Start(1);

            Assert.That(logCount, Is.EqualTo(1));
            Assert.That(logMessage, Contains.Substring("ERROR"));
        }

        [Test]
        public void Start_StartThenStopRecording_VerifyContentsOfBytes()
        {
            // ReSharper disable once NotAccessedVariable
            var logCount = 0;

            void LogWrite(object sender, string message)
            {
                logCount++;
            }

            var logger = NetServices.Registry.Get<NetLogger>();
            logger.LogWrite += LogWrite;

            var receivedId = uint.MinValue;

            void ReceiveId(uint id)
            {
                receivedId = id;
            }

            var recorder = new NetRecorder {ReceiveIdCallback = ReceiveId};

            recorder.Start(0);
            recorder.SendId(uint.MaxValue);
            recorder.Send(0, new byte[] {0, 1, 2, 3}, false);
            recorder.Send(1, new byte[] {4, 5, 6, 7}, false);
            recorder.Stop();

            var bytes = recorder.Bytes;
            var byteQueue = new NetByteQueue();
            byteQueue.WriteRaw(bytes);

            var entryCount = byteQueue.ReadInt();
            var firstTime = byteQueue.ReadFloat();
            var firstData = byteQueue.ReadBytes();
            var secondTime = byteQueue.ReadFloat();
            var secondData = byteQueue.ReadBytes();

            Assert.That(receivedId, Is.EqualTo(uint.MaxValue));
            Assert.That(entryCount, Is.EqualTo(2));
            Assert.That(firstTime, Is.EqualTo(0));
            Assert.That(firstData, Is.EqualTo(new byte[] {0, 1, 2, 3}));
            Assert.That(secondTime, Is.EqualTo(1));
            Assert.That(secondData, Is.EqualTo(new byte[] {4, 5, 6, 7}));
            Assert.That(recorder.IsRecording, Is.False);
        }
    }
}

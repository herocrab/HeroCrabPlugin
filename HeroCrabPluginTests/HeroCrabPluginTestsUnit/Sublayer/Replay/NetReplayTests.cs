// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Sublayer.Replay;
using NUnit.Framework;
// ReSharper disable CollectionNeverQueried.Local

namespace HeroCrabPluginTestsUnit.Sublayer.Replay
{
    [TestFixture]
    public class NetReplayTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Play_ProvideBytesProcess_EnsureDataCallBackIsCorrect()
        {
            var replayData = new SortedDictionary<float, byte[]>
            {
                {0f, new byte[] {0, 1, 2, 3}},
                {1f, new byte[] {3, 2, 1, 0}}
            };

            var playback = new List<byte[]>();
            void DataPlayback(byte[] data)
            {
                playback.Add(data);
            }

            var replayQueue = new NetByteQueue();
            replayQueue.WriteInt(2);
            replayQueue.WriteFloat(replayData.ElementAt(0).Key);
            replayQueue.WriteBytes(replayData.ElementAt(0).Value);
            replayQueue.WriteFloat(replayData.ElementAt(1).Key);
            replayQueue.WriteBytes(replayData.ElementAt(1).Value);

            var replay = NetReplay.Create() as NetReplay;
            if (replay == null) {
                Assert.Fail();
            }

            replay.DisableStream = true;
            replay.ReceiveDataCallback = DataPlayback;
            replay.Play(0f, replayQueue.ToBytes());
            Assert.That(replay.IsPlaying, Is.True);
            for (int i = 0; i < 10; i++) {
                replay.Process(i);
            }

            replay.Stop();
            Assert.That(replay.IsPlaying, Is.False);
            Assert.That(playback[0], Is.EqualTo(replayData[0]));
            Assert.That(playback[1], Is.EqualTo(replayData[1]));
            Assert.That(replay.Stream.SessionCount, Is.EqualTo(0));
        }

        [Test]
        public void Play_Stop_EnsureIsPlayingIsFalse()
        {
            var replay = NetReplay.Create() as NetReplay;
            if (replay == null) {
                Assert.Fail();
            }

            replay.DisableStream = true;
            replay.Play(0f, new byte[]{0});
            Assert.That(replay.IsPlaying, Is.True);
            replay.Stop();
            Assert.That(replay.IsPlaying, Is.False);
        }

        [Test]
        public void Play_PlayTwice_EnsureIsPlayingIsTrue()
        {
            var replay = NetReplay.Create() as NetReplay;
            if (replay == null) {
                Assert.Fail();
            }

            replay.DisableStream = true;
            replay.Play(0f, new byte[]{0});
            replay.Play(0f, new byte[]{1});
            Assert.That(replay.IsPlaying, Is.True);
            replay.Stop();
            Assert.That(replay.IsPlaying, Is.False);
        }

        [Test]
        public void Play_Disconnect_EnsureIsPlayingIsFalse()
        {
            var replay = NetReplay.Create() as NetReplay;
            if (replay == null) {
                Assert.Fail();
            }

            replay.DisableStream = true;
            replay.Play(0f, new byte[]{0});
            Assert.That(replay.IsPlaying, Is.True);
            replay.Disconnect();
            Assert.That(replay.IsPlaying, Is.False);
        }
    }
}

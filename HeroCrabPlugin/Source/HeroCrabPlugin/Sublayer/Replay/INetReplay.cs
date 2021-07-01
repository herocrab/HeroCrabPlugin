// // Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)

using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Sublayer.Replay
{
    /// <summary>
    /// Network replay player.
    /// </summary>
    public interface INetReplay
    {
        /// <summary>
        /// Play a replay.
        /// </summary>
        /// <param name="time">Start time of this play action</param>
        /// <param name="bytes">Replay contents in bytes</param>
        void Play(float time, byte[] bytes);

        /// <summary>
        /// Stop a replay.
        /// </summary>
        void Stop();

        /// <summary>
        /// Process this replay, required for playing.
        /// </summary>
        /// <param name="time">Game time</param>
        void Process(float time);

        /// <summary>
        /// Replay stream.
        /// </summary>
        INetStreamClient Stream { get; }

        /// <summary>
        /// True if this replay is currently playing.
        /// </summary>
        bool IsPlaying { get; }
    }
}

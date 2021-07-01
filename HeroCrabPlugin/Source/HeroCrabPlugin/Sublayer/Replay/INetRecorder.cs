// // Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)

namespace HeroCrabPlugin.Sublayer.Replay
{
    /// <summary>
    /// Network recorder for replay system.
    /// </summary>
    public interface INetRecorder
    {
        /// <summary>
        /// Start the network recorder.
        /// </summary>
        void Start(float time);

        /// <summary>
        /// Stop the network recorder.
        /// </summary>
        /// <returns>Recorded bytes</returns>
        void Stop();
    }
}

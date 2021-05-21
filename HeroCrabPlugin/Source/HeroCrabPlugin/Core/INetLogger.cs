namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Logger interface.
    /// </summary>
    public interface INetLogger
    {
        /// <summary>
        /// Write a log message to the logger.
        /// </summary>
        /// <param name="sender">Sending class, this will be added to the log for debugging</param>
        /// <param name="message">Log message text</param>
        void Write(object sender, string message);
    }
}

// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
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

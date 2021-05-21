namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network series of bytes field.
    /// </summary>
    public interface INetFieldBytes
    {
        /// <summary>
        /// Set the network series of bytes field.
        /// </summary>
        /// <param name="value">Bytes</param>
        void Set(byte[] value);
    }
}

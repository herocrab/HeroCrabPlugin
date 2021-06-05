namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Network object base class for network configuration and logger.
    /// </summary>
    public class NetObject
    {
        /// <summary>
        /// Network configuration.
        /// </summary>
        protected readonly NetSettings NetSettings;

        /// <summary>
        /// Network logger.
        /// </summary>
        protected readonly NetLogger NetLogger;

        /// <summary>
        /// Network object base class for injection of network configuration and logger.
        /// </summary>
        protected NetObject()
        {
            NetSettings = NetServices.Registry.Get<NetSettings>();
            NetLogger = NetServices.Registry.Get<NetLogger>();
        }
    }
}

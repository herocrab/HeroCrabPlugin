// ReSharper disable once CheckNamespace
public class NetObject
{
    protected readonly NetConfig NetConfig;
    protected readonly NetLogger NetLogger;

    protected NetObject()
    {
        NetConfig = NetServices.Registry.Get<NetConfig>();
        NetLogger = NetServices.Registry.Get<NetLogger>();
    }
}

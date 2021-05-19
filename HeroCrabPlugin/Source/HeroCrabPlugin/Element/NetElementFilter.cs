// ReSharper disable once CheckNamespace
public class NetElementFilter
{
    public NetStreamGroup StreamGroup { get; set; }
    public uint Recipient { get; set; } // 0 = Everyone
    public uint Exclude { get; set; }

    public NetElementFilter()
    {
        StreamGroup = NetStreamGroup.Default;
        Recipient = 0;
    }

    public bool Contains(NetStreamGroup streamGroup)
    {
        return (StreamGroup & streamGroup) == streamGroup;
    }
}

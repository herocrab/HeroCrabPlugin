using System;
// ReSharper disable UnusedMember.Global

namespace HeroCrabPlugin.Stream
{
    /// <summary>
    /// Stream group are a set of flags used in filtering which elements are sent over a session.
    /// </summary>
    [Flags]
    public enum NetStreamGroup
    {
        #pragma warning disable 1591
        Default = 1,
        Lobby = 2,
        Load = 4,
        Select = 8,
        Game = 16,
        Conclude = 32,
        Team1 = 64,
        Team2 = 128,
        Team3 = 256,
        Team4 = 512,
        Custom1 = 1024,
        Custom2 = 2048,
        Custom3 = 4096,
        Custom4 = 8192,
        #pragma warning restore 1591
    }
}

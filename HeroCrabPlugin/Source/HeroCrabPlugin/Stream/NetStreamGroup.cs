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
        Game = 8,
        Conclude = 16,
        Team1 = 32,
        Team2 = 64,
        Team3 = 128,
        Team4 = 256,
        Custom1 = 512,
        Custom2 = 1024,
        Custom3 = 2048,
        Custom4 = 4096,
        #pragma warning restore 1591
    }
}

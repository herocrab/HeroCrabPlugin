using System;
// ReSharper disable once CheckNamespace

[Flags]
public enum NetStreamGroup
{
    Default = 1,
    Lobby = 2,
    Load = 4,
    Game = 8,
    Conclude = 16,
    Team1 = 32,
    Team2 = 64,
    Team3 = 128,
    Team4 = 256,
}

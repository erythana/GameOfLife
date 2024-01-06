using System;

namespace GameOfLife;

public static class Defaults
{
    public static TimeSpan TickRate { get; } = TimeSpan.FromMilliseconds(200);
}
using System.Collections.Generic;
using System.Drawing;

namespace GameOfLife;

public static class SamplePatterns
{
    public static class StillLifes
    {
        public static IEnumerable<Point> Block = new[]
        {
            new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, 1)
        };

        public static IEnumerable<Point> BeeHive = new[]
        {
            new Point(0, 1), new Point(1, 1), new Point(-1, 0), new Point(2, 0), new Point(0, -1), new Point(1, -1)
        };

        public static IEnumerable<Point> Loaf = new[]
        {
            new Point(0, 1), new Point(1, 1), new Point(-1, 0), new Point(2, 0), new Point(0, -1), new Point(2, -1), new Point(1, -2)
        };

        public static IEnumerable<Point> Boat = new[]
        {
            new Point(-1, 1), new Point(0, 1), new Point(-1, 0), new Point(1, 0), new Point(0, -1)
        };

        public static IEnumerable<Point> Tub = new[]
        {
            new Point(0, 1), new Point(-1, 0), new Point(1, 0), new Point(0, -1)
        };

        public static IEnumerable<IEnumerable<Point>> AllStillLifes = new[]
        {
            Block, BeeHive, Loaf, Boat, Tub
        };
    }

    public static class Oscillators
    {
        public static IEnumerable<Point> Blinker = new[]
        {
            new Point(-1, 0), new Point(0, 0), new Point(1, 0)
        };

        public static IEnumerable<Point> Toad = new[]
        {
            new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(-1, -1), new Point(0, -1), new Point(1, -1)
        };

        public static IEnumerable<Point> Beacon = new[]
        {
            new Point(-1, 1), new Point(0, 1), new Point(-1, 0), new Point(0, 0), new Point(1, -1), new Point(2, -1), new Point(1, -2), new Point(2, -2)
        };

        public static IEnumerable<Point> Pulsar = new[]
        {
            new Point(-4, 6), new Point(-3, 6), new Point(-2, 6), new Point(2, 6), new Point(3, 6), new Point(4, 6), new Point(-6, 4), new Point(-1, 4), new Point(1, 4), new Point(6, 4), new Point(-6, 3),
            new Point(-1, 3), new Point(1, 3), new Point(6, 3), new Point(-6, 2), new Point(-1, 2), new Point(1, 2), new Point(6, 2), new Point(-4, 1), new Point(-3, 1), new Point(-2, 1), new Point(2, 1),
            new Point(3, 1), new Point(4, 1), new Point(-4, -6), new Point(-3, -6), new Point(-2, -6), new Point(2, -6), new Point(3, -6), new Point(4, -6), new Point(-6, -4), new Point(-1, -4), new Point(1, -4),
            new Point(6, -4), new Point(-6, -3), new Point(-1, -3), new Point(1, -3), new Point(6, -3), new Point(-6, -2), new Point(-1, -2), new Point(1, -2), new Point(6, -2), new Point(-4, -1),
            new Point(-3, -1), new Point(-2, -1), new Point(2, -1), new Point(3, -1), new Point(4, -1)
        };

        public static IEnumerable<Point> Pentadecathlon = new[]
        {
            new Point(-1, 3),new Point(0, 3),new Point(1, 3),
            new Point(-1, 2),new Point(1, 2),
            new Point(-1, 1),new Point(0, 1),new Point(1, 1),
            new Point(-1, 0),new Point(0, 0),new Point(1, 0),
            new Point(-1, -1),new Point(0, -1),new Point(1, -1),
            new Point(-1, -2),new Point(0, -2),new Point(1, -2),
            new Point(-1, -3),new Point(1, -3),
            new Point(-1, -4),new Point(0, -4),new Point(1, -4),
        };
        
        public static IEnumerable<IEnumerable<Point>> AllOscillators = new[]
        {
            Blinker, Toad, Beacon, Pulsar, Pentadecathlon
        };
    }

    public static class Spaceships
    {
        public static IEnumerable<Point> Glider = new[]
        {
            new Point(0, 1),
            new Point(1, 0), new Point(2,0),
            new Point(0, -1), new Point(1, -1)
        };
        public static IEnumerable<Point> LightweightSpaceship = new[]
        {
            new Point(-1, 2),new Point(0, 2),new Point(1, 2),new Point(2, 2),
            new Point(-2, 1), new Point(2, 1),
            new Point(2, 0),
            new Point(-2, -1), new Point(1, -1)
        };
        public static IEnumerable<Point> MiddleweightSpaceship = new[]
        {
            new Point(-1, 2), new Point(0, 2), new Point(1, 2),
            new Point(-2, 1), new Point(-1, 1), new Point(0, 1), new Point(1, 1), new Point(2, 1),
            new Point(-2, 0), new Point(-1, 0), new Point(0, 0), new Point(2, 0), new Point(3, 0),
            new Point(1, -1), new Point(2, -1)
        };
        public static IEnumerable<Point> HeavyweightSpaceship = new[]
        {
            new Point(-2, 2), new Point(-1, 2), new Point(0, 2), new Point(1, 2),
            new Point(-3, 1), new Point(-2, 1),  new Point(-1, 1), new Point(0, 1), new Point(1, 1), new Point(2, 1),
            new Point(-3, 0), new Point(-2, 0), new Point(-1, 0), new Point(0, 0), new Point(2, 0), new Point(3, 0),
            new Point(1, -1), new Point(2, -1)
        };
        
        public static IEnumerable<IEnumerable<Point>> AllSpaceships = new[]
        {
            Glider, LightweightSpaceship, MiddleweightSpaceship, HeavyweightSpaceship
        };
    }

    public static class Methuselah
    {
        public static IEnumerable<Point> RPentomino = new[]
        {
            new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(-1, 0), new Point(0, -1)
        };

    }
}
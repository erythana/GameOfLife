using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameOfLife.Extensions;

public static class PointEnumerableExtensions
{
    public static IEnumerable<Point> Offset(this IEnumerable<Point> lifeForm, Point offset)
    {
        return lifeForm.Select(p =>
        {
            p.Y += offset.Y;
            p.X += offset.X;
            return p;
        });
    }
}
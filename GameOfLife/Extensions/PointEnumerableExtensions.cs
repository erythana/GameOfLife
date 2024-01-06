using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameOfLife.Extensions;

public static class PointEnumerableExtensions
{
    public static IEnumerable<Point> Offset(this IEnumerable<Point> lifeForm, int xOffset, int yOffset)
    {
        return lifeForm.Select(p =>
        {
            p.Y += yOffset;
            p.X += xOffset;
            return p;
        });
    }
}
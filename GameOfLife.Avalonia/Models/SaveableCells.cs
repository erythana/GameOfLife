using System.Collections.Generic;
using System.Drawing;

namespace GameOfLife.Avalonia.Models;

public class SaveableCells
{
    public SaveableCells(string patternName, IEnumerable<Point> pattern)
    {
        PatternName = patternName;
        Pattern = pattern;
    }
    public string PatternName { get; set; }
    public IEnumerable<Point> Pattern { get; set; }
}
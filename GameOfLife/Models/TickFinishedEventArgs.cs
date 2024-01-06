using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameOfLife.Models;

public class TickFinishedEventArgs(long generation, IEnumerable<Point> activeCells) : EventArgs
{
    public long Generation { get; } = generation;
    public IEnumerable<Point> ActiveCells { get; } = activeCells;
}
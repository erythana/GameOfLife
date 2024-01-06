using System.Drawing;

namespace GameOfLife.Models;

public record struct Cell
{
    public required Point Position { get; set; }
}
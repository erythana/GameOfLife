using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace GameOfLife.Avalonia.Models;

public class PatternNode
{
    #region constructor

    public PatternNode(string title, ObservableCollection<PatternNode> subNodes)
    {
        Title = title;
        SubNodes = subNodes;
    }
    
    public PatternNode(string title, IEnumerable<Point>? pattern)
    {
        Title = title;
        Pattern = pattern;
    }

    #endregion

    #region properties

    public string Title { get; }
    public bool IsLeaf { get => Pattern?.Count() >= 0; }
    public IEnumerable<Point>? Pattern { get; set; }
    public ObservableCollection<PatternNode> SubNodes { get; } = [];

    #endregion
}
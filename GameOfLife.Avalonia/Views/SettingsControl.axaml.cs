using Avalonia.Controls;
using GameOfLife.Avalonia.Models;

namespace GameOfLife.Avalonia.Views;

public partial class SettingsControl : UserControl
{
    public SettingsControl()
    {
        InitializeComponent();
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView treeView || e.AddedItems.Count != 1 || e.AddedItems[0] is PatternNode { IsLeaf: true })
            return;
        
        treeView.UnselectAll();
        treeView.SelectedItem = null;
    }
}
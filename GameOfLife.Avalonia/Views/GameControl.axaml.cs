using System.Drawing;
using Avalonia.Controls;
using Avalonia.Input;
using GameOfLife.Avalonia.ViewModels;

namespace GameOfLife.Avalonia.Views;

public partial class GameControl : UserControl
{
    public GameControl()
    {
        InitializeComponent();
    }

    private void InputElement_OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not ItemsControl { ItemsPanelRoot: Canvas { DataContext: MainWindowViewModel viewModel } canvas } itemsControl)
            return;
        
        var pointerPosition = e.GetPosition(canvas);
        viewModel.ToggleCellOnCanvas(new Point((int)pointerPosition.X, (int)pointerPosition.Y));
    }
}
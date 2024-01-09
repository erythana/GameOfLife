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
        if (sender is not ItemsControl { ItemsPanelRoot: Canvas { DataContext: MainWindowViewModel viewModel } canvas })
            return;
        
        var pointerPosition = e.GetPosition(canvas);
        viewModel.OnCanvasClick(new Point((int)pointerPosition.X, (int)pointerPosition.Y));
    }

    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not ItemsControl { ItemsPanelRoot: Canvas { DataContext: MainWindowViewModel viewModel } canvas })
            return;

        var pointerPosition = e.GetPosition(canvas);
        viewModel.PlaceOverlayCells(new Point((int)pointerPosition.X, (int)pointerPosition.Y));
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not ItemsControl { ItemsPanelRoot: Canvas { DataContext: MainWindowViewModel viewModel } })
            return;

        if (e.InitialPressMouseButton != MouseButton.Left)
            viewModel.DisableCellOverlay();
    }
}
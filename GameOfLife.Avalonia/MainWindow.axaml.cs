using Avalonia.Controls;
using Avalonia.Input;
using GameOfLife.Avalonia.Models;
using GameOfLife.Avalonia.ViewModels;

namespace GameOfLife.Avalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        Title = AppInfo.ApplicationName;
    }

    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not MainWindow { DataContext: MainWindowViewModel viewModel })
            return;

        if (e.Key == Key.Escape)
            viewModel.DisableCellOverlay();
    }
}
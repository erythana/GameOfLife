using Avalonia.Controls;
using GameOfLife.Avalonia.ViewModels;

namespace GameOfLife.Avalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace GameOfLife.Avalonia.ViewModels;

public class SaveDialogViewModel : ViewModelBase
{
    private readonly Window _thisWindow;
    private string _fileName;
    #region constructor
    
    public SaveDialogViewModel(Window thisWindow)
    {
        _thisWindow = thisWindow;
        SaveCommand = ReactiveCommand.Create(Save, canExecute: this.WhenAnyValue(x => x.FileName, filename => !string.IsNullOrWhiteSpace(filename)));
    }

    private void Save()
    {
        _thisWindow.Close(FileName);
    }

    #endregion

    #region properties

    public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }

    public string FileName {
        get => _fileName;
        private set => SetField(ref _fileName, value);
    }

    #endregion
}
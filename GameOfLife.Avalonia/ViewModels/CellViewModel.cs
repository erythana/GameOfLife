namespace GameOfLife.Avalonia.ViewModels;

public class CellViewModel : ViewModelBase
{
    #region member fields

    private double _left;
    private double _top;
    private double _cellSize;

    #endregion

    #region properties

    public double Left {
        get => _left;
        set => SetField(ref _left, value);
    }

    public double Top {
        get => _top;
        set => SetField(ref _top, value);
    }

    public double CellSize {
        get => _cellSize;
        set => SetField(ref _cellSize, value);
    }

    #endregion
}
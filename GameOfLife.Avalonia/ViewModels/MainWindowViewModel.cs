using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using GameOfLife.Avalonia.Models;
using GameOfLife.Avalonia.Views;
using GameOfLife.Models;
using GameOfLife.Ruleset;
using ReactiveUI;
using Point = System.Drawing.Point;

namespace GameOfLife.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region member fields

    private readonly GameEngine _gameEngine;
    private long _currentGeneration;
    private int _cellSize;
    private int _updateRate;
    private Rect _canvasSize;
    private Rect _backgroundRectangleSize;
    private RelativeRect _backgroundGridSize;
    private IEnumerable<Point> _currentCells;
    private PatternNode? _selectedPattern;
    private bool _isPlacementMode;
    private PatternNode _customPatternNode;
    private Point? _lastKnownCellPosition;

    #endregion

    #region constructor

    public MainWindowViewModel()
    {
        _gameEngine = new GameEngine();
        _gameEngine.TickFinished += GameEngineOnTickFinished;
        CurrentCells = Enumerable.Empty<Point>();
        Cells = new ObservableCollection<CellViewModel>();
        CellSize = 20;

        InitializeCommands();
        InitializePatterns();
    }

    private void InitializePatterns()
    {
        InitializePatternPresets();
        _customPatternNode = new PatternNode(AppInfo.PresetNodeName,  new ObservableCollection<PatternNode>());
        PatternPresets.Add(_customPatternNode);
        
        var persistence = new PatternPersistence();
        var storedPatterns = persistence.LoadCellsFromFile();
        
        foreach (var storedPattern in storedPatterns)
            _customPatternNode.SubNodes.Add(new PatternNode(storedPattern.PatternName, storedPattern.Pattern));
    }

    #endregion

    #region properties

    public ReactiveCommand<Unit, Unit> StartGameCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> PauseGameCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> StopGameCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> PlacePatternCommand { get; private set; }
    public ReactiveCommand<Unit,Unit> ExportCellsCommand { get; private set; }
    public ReactiveCommand<PatternNode, Unit> DeletePresetCommand { get; private set; }

    public int CellSize {
        get => _cellSize;
        private set
        {
            SetField(ref _cellSize, value);
            BackgroundGridSize = new RelativeRect(0, 0, CellSize, CellSize, RelativeUnit.Absolute);
            BackgroundRectangleSize = new Rect(0, 0, CellSize, CellSize);
            PlaceCells(CurrentCells);
        }
    }

    public RelativeRect BackgroundGridSize {
        get => _backgroundGridSize;
        private set => SetField(ref _backgroundGridSize, value);
    }

    public Rect BackgroundRectangleSize {
        get => _backgroundRectangleSize;
        private set => SetField(ref _backgroundRectangleSize, value);
    }


    public ObservableCollection<CellViewModel> Cells { get; private set; }

    public long CurrentGeneration {
        get => _currentGeneration;
        private set => SetField(ref _currentGeneration, value);
    }

    public int UpdateRate {
        get => _updateRate;
        private set
        {
            SetField(ref _updateRate, value);
            _gameEngine.TickRate = new TimeSpan(0, 0, 0, 0, value);
        }
    }

    public List<PatternNode> PatternPresets { get; private set; }

    public PatternNode? SelectedPattern {
        get => _selectedPattern;
        set => SetField(ref _selectedPattern, value);
    }

    public Rect CanvasSize {
        get => _canvasSize;
        set
        {
            _canvasSize = value;
            PlaceCells(CurrentCells);
        }
    }

    private IEnumerable<Point> CurrentCells {
        get => _currentCells;
        set => SetField(ref _currentCells, value);
    }

    #endregion

    #region methods

    private void StartGame()
    {
        _isPlacementMode = false;
        _gameEngine.StartGame();
    }

    private void PauseGame() => _gameEngine.PauseGame();

    private void StopGame()
    {
        _gameEngine.StopGame();
        Cells.Clear();
        CurrentGeneration = 0;
    }
    
    private void PlacePattern() => _isPlacementMode = true;
    
    private async void ExportCells()
    {
        if (Application.Current!.ApplicationLifetime is not ClassicDesktopStyleApplicationLifetime { MainWindow: { } ownerWindow })
            return;
        
        var saveDialog = new SaveDialogWindow();
        var viewModel = new SaveDialogViewModel(saveDialog);
        saveDialog.DataContext = viewModel;
        var fileName = await saveDialog.ShowDialog<string>(ownerWindow);

        if (string.IsNullOrWhiteSpace(fileName))
            return;
        
        var persistance = new PatternPersistence();
        persistance.ExportCellsToFile(new SaveableCells(fileName, CurrentCells));

        var presetNode = PatternPresets.Find(x => x.Title == AppInfo.PresetNodeName);
        presetNode?.SubNodes.Add(new PatternNode(fileName, CurrentCells));
    }
    
    private void DeletePreset(PatternNode patternNode)
    {
        var persistence = new PatternPersistence();
        persistence.DeleteCellsFromFile(new SaveableCells(patternNode.Title, patternNode.Pattern!));
        _customPatternNode.SubNodes.Remove(patternNode);
    }

    public void OnCanvasClick(Point pointerPosition)
    {
        if (_gameEngine.IsGameRunning && !_gameEngine.IsGamePaused)
            return;

        if (_isPlacementMode)
        {
            var pattern = SelectedPattern!.Pattern!;
            var cellToPlace = GetCellPosition(pointerPosition);
            var modifiedPatternPosition = pattern.Select(p =>
            {
                p.X += cellToPlace.X;
                p.Y += cellToPlace.Y;
                return p;
            });
            
            PlaceCells(CurrentCells.Concat(modifiedPatternPosition));
        }
        else
            ToggleCellOnCanvas(pointerPosition);
    }

    private void ToggleCellOnCanvas(Point pointerPosition)
    {
        var cellToPlace = GetCellPosition(pointerPosition);

        PlaceCells(CurrentCells.Contains(cellToPlace)
            ? CurrentCells.Where(x => !x.Equals(cellToPlace))
            : CurrentCells.Append(cellToPlace));
    }

    private void GameEngineOnTickFinished(object? sender, TickFinishedEventArgs e)
    {
        PlaceCells(e.ActiveCells);
        CurrentGeneration = e.Generation;
    }
    
    public void PlaceOverlayCells(Point pointerPosition)
    {
        var mouseCellPosition = GetCellPosition(pointerPosition);
        if (!_isPlacementMode || SelectedPattern?.Pattern is null || _lastKnownCellPosition == mouseCellPosition)
            return;

        _lastKnownCellPosition = mouseCellPosition;
        
        var cellsToPlace = SelectedPattern.Pattern.ToList();
        
        var furthestTopLeft = GetMostTopLeftFrom(cellsToPlace);
        var cellOffset = new Point(mouseCellPosition.X - furthestTopLeft.X, mouseCellPosition.Y - furthestTopLeft.Y);
        
        var horizontalOffsetPixel = CanvasSize.Width / 2 % CellSize;
        var verticalOffsetPixel = CanvasSize.Height / 2 % CellSize - CellSize;

        var placedCells = CurrentCells.Select(p => new CellViewModel
        {
            CellSize = CellSize,
            Left = p.X * CellSize - horizontalOffsetPixel,
            Top = p.Y * CellSize + verticalOffsetPixel
        });
        
        Cells = new ObservableCollection<CellViewModel>(placedCells.Concat(cellsToPlace.Select(p => new CellViewModel
        {
            CellSize = CellSize,
            Left = (p.X + cellOffset.X) * CellSize - horizontalOffsetPixel,
            Top = (p.Y + cellOffset.Y) * CellSize + verticalOffsetPixel 
        })));
        OnPropertyChanged(nameof(Cells));
    }

    public void DisableCellOverlay()
    {
        _isPlacementMode = false;
        
        var horizontalOffset = (CanvasSize.Width / 2) % CellSize;
        var verticalOffset = (CanvasSize.Height / 2) % CellSize - CellSize;
        Cells = new ObservableCollection<CellViewModel>(CurrentCells.Select(p => new CellViewModel
        {
            CellSize = CellSize,
            Left = p.X * CellSize - horizontalOffset,
            Top = p.Y * CellSize + verticalOffset 
        }));
        OnPropertyChanged(nameof(Cells));
    }

    #endregion

    #region helper methods

    private void InitializeCommands()
    {
        
        var notRunning = this
            .WhenAnyValue(x => x._gameEngine.IsGameRunning)
            .ObserveOn(RxApp.MainThreadScheduler)
            .CombineLatest(this
                    .WhenAnyValue(x => x._gameEngine.IsGamePaused)
                    .ObserveOn(RxApp.MainThreadScheduler),
                (isRunning, isPaused) => !isRunning || isPaused);//directly negating is not supported

        StartGameCommand = ReactiveCommand.Create(StartGame,
            canExecute: notRunning, RxApp.MainThreadScheduler);
        
        ExportCellsCommand = ReactiveCommand.Create(ExportCells, 
            canExecute: notRunning
            .CombineLatest(this
                .WhenAnyValue(x => x.CurrentCells)
                .ObserveOn(RxApp.MainThreadScheduler), (previousCombined, currentCells) => previousCombined && currentCells.Any()), outputScheduler: RxApp.MainThreadScheduler);
        
        PauseGameCommand = ReactiveCommand.Create(PauseGame,
            canExecute: this
                .WhenAnyValue(x => x._gameEngine.IsGameRunning)
                .ObserveOn(RxApp.MainThreadScheduler)
                .CombineLatest(this
                        .WhenAnyValue(x => x._gameEngine.IsGamePaused)
                        .ObserveOn(RxApp.MainThreadScheduler),
                    (isRunning, isPaused) => isRunning && !isPaused), outputScheduler: RxApp.MainThreadScheduler);

        
        StopGameCommand = ReactiveCommand.Create(StopGame,
            canExecute: this
                .WhenAnyValue(x => x._gameEngine.IsGameRunning)
                .ObserveOn(RxApp.MainThreadScheduler), outputScheduler: RxApp.MainThreadScheduler);
        
        PlacePatternCommand = ReactiveCommand.Create(PlacePattern,
            canExecute: this
                .WhenAnyValue(x => x._gameEngine.IsGameRunning)
                .ObserveOn(RxApp.MainThreadScheduler)
                .CombineLatest(this
                        .WhenAnyValue(x => x._gameEngine.IsGamePaused)
                        .ObserveOn(RxApp.MainThreadScheduler),
                    (isRunning, isPaused) => !isRunning || isPaused)
                .CombineLatest(this
                    .WhenAnyValue(x => x.SelectedPattern)
                    .ObserveOn(RxApp.MainThreadScheduler), 
                    (previousCombined, patternNode) => previousCombined && patternNode is not null), outputScheduler: RxApp.MainThreadScheduler); //directly negating is not supported
        
        DeletePresetCommand = ReactiveCommand.Create<PatternNode>(DeletePreset);
    }

    private void InitializePatternPresets()
    {
        PatternPresets =
        [
            new PatternNode("Sample patterns",
                new ObservableCollection<PatternNode>
                {
                    new("Still Life",
                        new ObservableCollection<PatternNode>
                        {
                            new(nameof(SamplePatterns.StillLifes.Block), SamplePatterns.StillLifes.Block),
                            new(nameof(SamplePatterns.StillLifes.BeeHive), SamplePatterns.StillLifes.BeeHive),
                            new(nameof(SamplePatterns.StillLifes.Loaf), SamplePatterns.StillLifes.Loaf),
                            new(nameof(SamplePatterns.StillLifes.Boat), SamplePatterns.StillLifes.Boat),
                            new(nameof(SamplePatterns.StillLifes.Tub), SamplePatterns.StillLifes.Tub)
                        }),
                    new("Oscillators",
                        new ObservableCollection<PatternNode>
                        {
                            new(nameof(SamplePatterns.Oscillators.Blinker), SamplePatterns.Oscillators.Blinker),
                            new(nameof(SamplePatterns.Oscillators.Beacon), SamplePatterns.Oscillators.Beacon),
                            new(nameof(SamplePatterns.Oscillators.Pulsar), SamplePatterns.Oscillators.Pulsar),
                            new(nameof(SamplePatterns.Oscillators.Toad), SamplePatterns.Oscillators.Toad),
                            new(nameof(SamplePatterns.Oscillators.Pentadecathlon), SamplePatterns.Oscillators.Pentadecathlon)
                        }),
                    new("Spaceships",
                        new ObservableCollection<PatternNode>
                        {
                            new(nameof(SamplePatterns.Spaceships.Glider), SamplePatterns.Spaceships.Glider),
                            new(nameof(SamplePatterns.Spaceships.LightweightSpaceship), SamplePatterns.Spaceships.LightweightSpaceship),
                            new(nameof(SamplePatterns.Spaceships.MiddleweightSpaceship), SamplePatterns.Spaceships.MiddleweightSpaceship),
                            new(nameof(SamplePatterns.Spaceships.HeavyweightSpaceship), SamplePatterns.Spaceships.HeavyweightSpaceship)
                        }),
                    new("Methuselah",
                        new ObservableCollection<PatternNode>
                        {
                            new(nameof(SamplePatterns.Methuselah.RPentomino), SamplePatterns.Methuselah.RPentomino)
                        }),
                })
        ];
    }
    
    private Point GetCellPosition(Point pointerPosition)
    {

        var horizontalCell = pointerPosition.X / CellSize + (pointerPosition.X >= 0
            ? 1
            : 0);
        var verticalCell = pointerPosition.Y / CellSize + (pointerPosition.Y >= 0
            ? 0
            : -1);
        var cellToPlace = new Point(horizontalCell, verticalCell);
        return cellToPlace;
    }
    
    private static Point GetMostTopLeftFrom(IEnumerable<Point> cellsToPlace)
    {
        var furthestPoint = new Point(int.MaxValue, int.MaxValue);
        foreach (var point in cellsToPlace)
        {
            if (point.X < furthestPoint.X)
                furthestPoint = point;
            else if (point.X == furthestPoint.X && point.Y > furthestPoint.Y)
                furthestPoint = point;
        }

        return furthestPoint;
    }
    
    private void PlaceCells(IEnumerable<Point> cells)
    {
        var cellsToPlace = cells.ToList();
        CurrentCells = cellsToPlace;
        
        var horizontalOffset = (CanvasSize.Width / 2) % CellSize;
        var verticalOffset = (CanvasSize.Height / 2) % CellSize - CellSize;

        Cells = new ObservableCollection<CellViewModel>(cellsToPlace.Select(p => new CellViewModel
        {
            CellSize = CellSize,
            Left = p.X * CellSize - horizontalOffset,
            Top = p.Y * CellSize + verticalOffset 
        }));
        OnPropertyChanged(nameof(Cells));
        
        _gameEngine.PlaceCells(cellsToPlace);
    }

    #endregion
}
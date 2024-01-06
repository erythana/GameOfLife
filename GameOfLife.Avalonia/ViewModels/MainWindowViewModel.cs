using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using GameOfLife.Avalonia.Models;
using GameOfLife.Extensions;
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
    private double _cellSize;
    private RelativeRect _backgroundGridSize;
    private Rect _backgroundRectangleSize;
    private int _updateRate;
    private PatternNode _selectedPattern;

    #endregion

    #region constructor

    public MainWindowViewModel()
    {
        Cells = new ObservableCollection<CellViewModel>();
        CellSize = 20;
        _gameEngine = new GameEngine();
        _gameEngine.TickFinished += GameEngineOnTickFinished;
        
        //temporary
        _gameEngine.PlaceCells(SamplePatterns.Oscillators.Pentadecathlon.Offset(0, 0));
        
        InitializeCommands();
        InitializePatternPresets();
    }
    
    #endregion

    #region properties

    public ReactiveCommand<Unit, Unit> StartGameCommand { get; private set; }
    public ReactiveCommand<Unit, Unit>  PauseGameCommand { get; private set; }
    public ReactiveCommand<Unit, Unit>  StopGameCommand { get; private set; }

    public double CellSize {
        get => _cellSize;
        private set
        {
            SetField(ref _cellSize, value);
            BackgroundGridSize = new RelativeRect(0, 0, CellSize, CellSize, RelativeUnit.Absolute);
            BackgroundRectangleSize = new Rect(0, 0, CellSize, CellSize);
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

    public IEnumerable<PatternNode> PatternPresets { get; private set; }

    public PatternNode SelectedPattern {
        get => _selectedPattern;
        set => SetField(ref _selectedPattern, value);
    }

    #endregion

    #region methods

    private void StartGame() => _gameEngine.StartGame();
    private void PauseGame() => _gameEngine.PauseGame();
    private void StopGame() => _gameEngine.StopGame();
    
    private void GameEngineOnTickFinished(object? sender, TickFinishedEventArgs e)
    {
        var cells = e.ActiveCells.Select(c => new CellViewModel
        {
            Top = c.Y * CellSize,
            Left = c.X * CellSize,
            CellSize = CellSize
        });
        Cells = new ObservableCollection<CellViewModel>(cells);
        OnPropertyChanged(nameof(Cells));
        
        CurrentGeneration = e.Generation;
    }

    #endregion

    #region helper methods

    private void InitializeCommands()
    {
        StartGameCommand = ReactiveCommand.Create(StartGame,
            canExecute: this
                .WhenAnyValue(x => x._gameEngine.IsGameRunning)
                .ObserveOn(RxApp.MainThreadScheduler)
                .CombineLatest(this
                        .WhenAnyValue(x => x._gameEngine.IsGamePaused)
                        .ObserveOn(RxApp.MainThreadScheduler),
                    (isRunning, isPaused) => !isRunning || isPaused), outputScheduler: RxApp.MainThreadScheduler); //directly negating is not supported
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
    }

    private void InitializePatternPresets()
    {
        PatternPresets = new[]
        {
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
                })
        };
    }

    #endregion
}
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using GameOfLife.Models;
using GameOfLife.Ruleset;
using ReactiveUI;

namespace GameOfLife.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region member fields

    private readonly GameEngine _gameEngine;
    private long _currentGeneration;

    #endregion

    #region constructor

    public MainWindowViewModel()
    {
        _gameEngine = new GameEngine();
        
        //temporary
        CellSize = 20;
        _gameEngine.PlaceCells(new []
        {
            new Point(-1,0),
            new Point(0,0),
            new Point(1,0)
        });
        
        Cells = new ObservableCollection<CellViewModel>();
        InitializeCommands();

        _gameEngine.TickFinished += GameEngineOnTickFinished;
    }

    private void GameEngineOnTickFinished(object? sender, TickFinishedEventArgs e)
    {
        var cells = e.ActiveCells.Select(c => new CellViewModel
        {
            Top = c.Y * CellSize,
            Left = c.X * CellSize,
            CellSize = CellSize
        });
        Cells.Clear();
        Cells.AddRange(cells);
        CurrentGeneration = e.Generation;
    }

    #endregion

    #region properties

    public IReactiveCommand StartGameCommand { get; private set; }
    public IReactiveCommand PauseGameCommand { get; private set; }
    public IReactiveCommand StopGameCommand { get; private set; }

    public double CellSize { get; private set; }

    public ObservableCollection<CellViewModel> Cells { get; private set; }

    public long CurrentGeneration {
        get => _currentGeneration;
        private set => SetField(ref _currentGeneration, value);
    }

    #endregion

    #region methods

    private void StartGame() => _gameEngine.StartGame();
    private void PauseGame() => _gameEngine.PauseGame();
    private void StopGame() => _gameEngine.StopGame();

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

    #endregion
}
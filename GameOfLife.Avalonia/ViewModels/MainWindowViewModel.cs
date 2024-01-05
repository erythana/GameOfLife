using System.Reactive.Linq;
using GameOfLife.Ruleset;
using ReactiveUI;

namespace GameOfLife.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region member fields

    private readonly GameEngine _gameEngine;

    #endregion

    #region constructor

    public MainWindowViewModel()
    {
        _gameEngine = new GameEngine();
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        StartGameCommand = ReactiveCommand.Create(StartGame,
            canExecute: this
                .WhenAnyValue(x => x._gameEngine.IsGameRunning)
                .CombineLatest(this
                        .WhenAnyValue(x => x._gameEngine.IsGamePaused),
                    (isRunning, isPaused) => !isRunning || isPaused)); //directly negating is not supported
        PauseGameCommand = ReactiveCommand.Create(PauseGame,
            canExecute: this
                .WhenAnyValue(x => x._gameEngine.IsGameRunning)
                .CombineLatest(this
                        .WhenAnyValue(x => x._gameEngine.IsGamePaused),
                    (isRunning, isPaused) => isRunning && !isPaused));
        StopGameCommand = ReactiveCommand.Create(StopGame,
            canExecute: this.WhenAnyValue(x => x._gameEngine.IsGameRunning));
    }

    #endregion

    #region properties

    public IReactiveCommand StartGameCommand { get; private set; }
    public IReactiveCommand PauseGameCommand { get; private set; }
    public IReactiveCommand StopGameCommand { get; private set; }

    #endregion

    #region methods

    private void StartGame() => _gameEngine.StartGame();
    private void PauseGame() => _gameEngine.PauseGame();
    private void StopGame() => _gameEngine.StopGame();

    #endregion
}
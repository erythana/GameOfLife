using System;
using System.Timers;

namespace GameOfLife.Ruleset;

public class GameEngine : ModelBase
{
    #region member fields

    private readonly Timer _tickTimer;
    private TimeSpan _tickRate;
    private bool _isGameRunning;
    private bool _isGamePaused;

    #endregion

    #region constructor

    public GameEngine()
    {
        _tickTimer = new Timer();
        _tickTimer.Elapsed += TickTimerOnElapsed;
        _tickTimer.AutoReset = true;

        IsGameRunning = false;
        
        TickRate = Defaults.TickRate;
    }

    private void TickTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        // Any live cell with fewer than two live neighbours dies, as if by underpopulation.
        // Any live cell with two or three live neighbours lives on to the next generation.
        // Any live cell with more than three live neighbours dies, as if by overpopulation.
        // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
    }

    #endregion


    #region methods
    
    public void StartGame()
    {
        IsGameRunning = true;
        IsGamePaused = false;
        _tickTimer.Enabled = true;
    }
    
    public void PauseGame()
    {
        _tickTimer.Enabled = false;
        IsGamePaused = true;
    }

    public void StopGame()
    {
        //Clear all Cells N Stuff
        _tickTimer.Enabled = false;
        IsGameRunning = false;
    }

    #endregion

    #region properties

    /// <summary>
    /// The timespans in which the cells get updated.
    /// </summary>
    public TimeSpan TickRate {
        get => _tickRate;
        set
        {
            _tickRate = value;
            _tickTimer.Interval = TickRate.TotalMilliseconds;
        }
    }

    public bool IsGameRunning {
        get => _isGameRunning;
        private set => SetField(ref _isGameRunning, value);
    }
    
    public bool IsGamePaused {
        get => _isGamePaused;
        private set => SetField(ref _isGamePaused, value);
    }

    #endregion
    
}
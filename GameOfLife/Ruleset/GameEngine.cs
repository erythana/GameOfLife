using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using GameOfLife.Models;

namespace GameOfLife.Ruleset;

public class GameEngine : ModelBase
{
    #region member fields

    private readonly Timer _tickTimer;
    private TimeSpan _tickRate;
    private bool _isGameRunning;
    private bool _isGamePaused;
    private long _generation;

    //alternate between these two for each generation (current/next)
    private readonly HashSet<Point> _activeCellsBuffer1;
    private readonly HashSet<Point> _activeCellsBuffer2;

    #endregion

    #region constructor

    public GameEngine()
    {
        _tickTimer = new Timer();
        _tickTimer.Elapsed += TickTimerOnElapsed;

        _activeCellsBuffer1 = new HashSet<Point>();
        _activeCellsBuffer2 = new HashSet<Point>();
        IsGameRunning = false;

        TickRate = Defaults.TickRate;
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
        _activeCellsBuffer1.Clear();
        _activeCellsBuffer2.Clear();
        _tickTimer.Enabled = false;
        IsGameRunning = false;
    }

    public void PlaceCells(IEnumerable<Point> cellsToPlace)
    {
        var buffer = GetCellBuffer(Generation);
        foreach (var cell in cellsToPlace)
            buffer.Add(cell);
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

    public long Generation {
        get => _generation;
        private set => SetField(ref _generation, value);
    }

    #endregion

    #region events

    public event EventHandler<TickFinishedEventArgs>? TickFinished;

    private void TickTimerOnElapsed(object? sender, ElapsedEventArgs e) => PopulateGeneration();

    #endregion


    #region helper methods

    private void PopulateGeneration()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var currentGeneration = GetCellBuffer(Generation);
        var nextGeneration = GetCellBuffer(Generation + 1);
        if (currentGeneration.Count == 0)
            StopGame();

        nextGeneration.Clear();

        foreach (var activeCell in currentGeneration)
        {
            var adjacentCellPositions = GetAdjacentPoints(activeCell);

            var deadNeighbors = new HashSet<Point>();
            var liveNeighbors = 0;
            foreach (var point in adjacentCellPositions)
            {
                if (currentGeneration.Contains(point))
                    liveNeighbors++;
                else
                    deadNeighbors.Add(point);
            }

            // Any live cell with fewer than two live neighbours dies, as if by underpopulation.
            // Any live cell with two or three live neighbours lives on to the next generation.
            // Any live cell with more than three live neighbours dies, as if by overpopulation.
            // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            if (liveNeighbors is >= 2 and <= 3)
                nextGeneration.Add(activeCell);

            foreach (var point in deadNeighbors)
            {
                liveNeighbors = 0;
                var deadAdjacents = GetAdjacentPoints(point);
                foreach (var adjacentPoint in deadAdjacents)
                {
                    if (currentGeneration.Contains(adjacentPoint))
                        liveNeighbors++;
                }

                if (liveNeighbors == 3)
                    nextGeneration.Add(point);
            }
        }

        TickFinished?.Invoke(this, new TickFinishedEventArgs(Generation, currentGeneration));
        ++Generation;
        stopwatch.Stop();
        Debug.WriteLine(stopwatch.Elapsed);
    }

    private IEnumerable<Point> GetAdjacentPoints(Point currentPosition)
    {
        var x = currentPosition.X;
        var y = currentPosition.Y;

        for (var i = y - 1; i <= y + 1; i++)
        {
            for (var j = x - 1; j <= x + 1; j++)
            {
                var point = new Point(j, i);
                if (point == currentPosition)
                    continue;
                yield return new Point(j, i);
            }
        }
    }

    private HashSet<Point> GetCellBuffer(long generation) => generation % 2 == 0
        ? _activeCellsBuffer1
        : _activeCellsBuffer2;

    #endregion

}
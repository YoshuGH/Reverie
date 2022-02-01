using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState state { get; private set; }

    public delegate void GameStateChangeHandler(GameState _newGameState);
    public event GameStateChangeHandler OnGameStateChanged;

    public void SetGameState( GameState _newGameState)
    {
        if (_newGameState == state)
            return;

        state = _newGameState;

        switch (state)
        {
            case GameState.Gameplay:
                break;
            case GameState.Pause:
                break;
            case GameState.Victory:
                break;
            case GameState.Lose:
                break;
            default:
                break;
        }

        OnGameStateChanged?.Invoke(_newGameState);
    }
}

public enum GameState
{
    Gameplay,
    Pause,
    Victory,
    Lose
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePnl, optionsPnl, startPnl;
    [SerializeField] private Selectable optionsSelectBtn;
    [SerializeField] private Button pauseSelectBTN;

    private bool isNotInPrincipal = false;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        OnGameStateChanged(GameManager.Instance.state);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState _newGameState)
    {
        if(_newGameState == GameState.Pause && !isNotInPrincipal)
        {
            pausePnl.SetActive(_newGameState == GameState.Pause);
            pauseSelectBTN.Select();
        }
        else if (_newGameState != GameState.Pause)
        {
            pausePnl.SetActive(false);
        }
    }

    public void Resume()
    {
        GameManager.Instance.SetGameState(GameState.Gameplay);
    }

    public void Options( bool exit)
    {
        if(exit)
        {
            startPnl.SetActive(true);
            optionsPnl.SetActive(false);
            pauseSelectBTN.Select();
            isNotInPrincipal = false;
        }
        else
        {
            startPnl.SetActive(false);
            optionsPnl.SetActive(true);
            optionsSelectBtn.Select();
            isNotInPrincipal = true;
        }
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}

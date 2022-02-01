using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject menuPnl, lvlSelecPnl, settingsPnl, creditsPnl, settingsSelectedObject, startSelectedObject, creditsSelectedObject, lvlSelecObj;
    [SerializeField] private Button start, options, exit;
    public float leanTime;
    [SerializeField] private EventSystem eventSys;

    private bool runAnimOnce = false, activeRotation = true;
    private GameObject lastGameObject;

    private void Awake()
    {
        //SoundManager.Initialize();
        lastGameObject = eventSys.firstSelectedGameObject;
    }

    private void Update()
    {
        if (lastGameObject != eventSys.currentSelectedGameObject)
        {
            runAnimOnce = false;
        }

        if(activeRotation)
        {
            if (eventSys.currentSelectedGameObject == options.gameObject && !runAnimOnce)
            {
                RotateBtns(menuPnl, -50f);
                lastGameObject = eventSys.currentSelectedGameObject;
                runAnimOnce = true;
            }

            if (eventSys.currentSelectedGameObject == exit.gameObject && !runAnimOnce)
            {
                RotateBtns(menuPnl, 50f);
                lastGameObject = eventSys.currentSelectedGameObject;
                runAnimOnce = true;
            }

            if (eventSys.currentSelectedGameObject == start.gameObject && !runAnimOnce)
            {
                RotateBtns(menuPnl, 0f);
                lastGameObject = eventSys.currentSelectedGameObject;
                runAnimOnce = true;
            }
        }

    }

    public void Play(bool exit)
    {
        if (exit)
        {
            eventSys.SetSelectedGameObject(null);
            lastGameObject = null;

            LeanTween.cancel(lvlSelecPnl);
            LeanTween.rotateZ(lvlSelecPnl, -520f + 180f, leanTime).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
                lvlSelecPnl.SetActive(false);
                menuPnl.SetActive(true);
                LeanTween.rotateZ(menuPnl, 0f + 180f, leanTime).setEase(LeanTweenType.easeOutBack).setOnComplete(() => {
                    start.Select();
                    activeRotation = true;
                });
            });
        }
        else
        {
            eventSys.SetSelectedGameObject(null);
            lastGameObject = null;
            activeRotation = false;

            LeanTween.cancel(menuPnl);
            LeanTween.rotateZ(menuPnl, 520f + 180f, leanTime).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
                menuPnl.SetActive(false);
                lvlSelecPnl.SetActive(true);
                LeanTween.rotateZ(lvlSelecPnl, 0f + 180f, leanTime).setEase(LeanTweenType.easeOutBack).setOnComplete(() => {
                    eventSys.SetSelectedGameObject(lvlSelecObj);
                });
            });
        }
    }

    public void Settings(bool exit)
    {
        if (exit)
        {
            eventSys.SetSelectedGameObject(null);
            lastGameObject = null;
            
            LeanTween.cancel(settingsPnl);
            LeanTween.rotateZ(settingsPnl, -520f + 180f, leanTime).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
                settingsPnl.SetActive(false);
                menuPnl.SetActive(true);
                LeanTween.rotateZ(menuPnl, 0f + 180f, leanTime).setEase(LeanTweenType.easeOutBack).setOnComplete(() => {
                    start.Select();
                    activeRotation = true;
                });
            });
        }
        else
        {
            eventSys.SetSelectedGameObject(null);
            lastGameObject = null;
            activeRotation = false;
            
            LeanTween.cancel(menuPnl);
            LeanTween.rotateZ(menuPnl, 520f + 180f, leanTime).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
                menuPnl.SetActive(false);
                settingsPnl.SetActive(true);
                LeanTween.rotateZ(settingsPnl, 0f + 180f, leanTime).setEase(LeanTweenType.easeOutBack).setOnComplete(() => {
                    eventSys.SetSelectedGameObject(settingsSelectedObject);
                });
            });
        }
    }

    public void Credits(bool exit)
    {
        if(exit)
        {
            eventSys.SetSelectedGameObject(null);
            lastGameObject = null;

            LeanTween.cancel(creditsPnl);
            LeanTween.rotateZ(creditsPnl, -520f + 180f, leanTime).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
                creditsPnl.SetActive(false);
                settingsPnl.SetActive(true);
                LeanTween.rotateZ(settingsPnl, 0f + 180f, leanTime).setEase(LeanTweenType.easeOutBack).setOnComplete(() => {
                    eventSys.SetSelectedGameObject(settingsSelectedObject);
                });
            });
        }
        else
        {
            eventSys.SetSelectedGameObject(null);
            lastGameObject = null;
            activeRotation = false;

            LeanTween.cancel(settingsPnl);
            LeanTween.rotateZ(settingsPnl, 520f + 180f, leanTime).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
                settingsPnl.SetActive(false);
                creditsPnl.SetActive(true);
                LeanTween.rotateZ(creditsPnl, 0f + 180f, leanTime).setEase(LeanTweenType.easeOutBack).setOnComplete(() => {
                    eventSys.SetSelectedGameObject(creditsSelectedObject);
                });
            });
        }
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Se salio del juego");
    }

    void RotateBtns(GameObject _pivot, float _rotation)
    {
        LeanTween.cancelAll();
        LeanTween.rotateZ(_pivot, _rotation + 180f, leanTime).setEase(LeanTweenType.easeInOutBack);
    }
}

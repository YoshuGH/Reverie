using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class VictoryManager : MonoBehaviour
{
    [SerializeField] private GameObject victoryPnl;
    [SerializeField] private String levelNameTitle, nextLevel;
    [SerializeField] TextMeshProUGUI levelTextUI;
    [SerializeField] private Text completedTxt;
    [SerializeField] private Button continueBtn;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState _newGameState)
    {
        if(_newGameState == GameState.Victory)
        {
            victoryPnl.SetActive(true);
            LeanTween.alphaText(victoryPnl.gameObject.GetComponent<RectTransform>(), 1, 1f);
            levelTextUI.text = levelNameTitle;
            LeanTween.move(completedTxt.rectTransform, new Vector3(0, 50, 0), 1f).setEase(LeanTweenType.easeOutCirc);
            LeanTween.textAlpha(completedTxt.rectTransform, 0.55f, 2f).setEase(LeanTweenType.easeOutCirc).setOnComplete(() => {
                continueBtn.Select();
            });
        }
    }

    public void Continue()
    {
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetGameState(GameState.Victory);
        }
    }
}

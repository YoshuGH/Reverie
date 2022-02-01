using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public GameObject levelBtnPrefab;
    public RectTransform levelHolder;

    [SerializeField] private string[] levels;
    [SerializeField] private int unlockLevels;

    private void Awake()
    {
        unlockLevels = PlayerPrefs.GetInt("unlockLevels", 1);
    }

    void Start()
    {
        int i = 0;

        foreach (string level in levels)
        {
            GameObject buttonPrefab = Instantiate(levelBtnPrefab, levelHolder);
            TextMeshProUGUI lvlName = buttonPrefab.transform.Find("LevelName").GetComponent<TextMeshProUGUI>();
            lvlName.text = level;
            Button button = buttonPrefab.transform.Find("LVLButton").GetComponent<Button>();
            button.FindSelectableOnDown();
            button.FindSelectableOnRight();
            button.FindSelectableOnLeft();

            
            button.onClick.AddListener(delegate { BTNOnClick(level); });

            if (i + 1 > unlockLevels)
                button.interactable = false;

            i++;
        }
    }

    void BTNOnClick(string _levelName)
    {
        SceneManager.LoadScene(_levelName, LoadSceneMode.Single);
    }
}

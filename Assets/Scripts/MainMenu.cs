using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Serializable]
    public class LevelButtonData
    {
        public int levelIndex = 1;
        public Button button;
        public TextMeshProUGUI label;
        public GameObject lockOverlay;
    }

    [Header("Play")]
    [SerializeField] private bool playHighestUnlockedLevel = true;
    [SerializeField] private int fallbackPlayLevel = 1;
    [SerializeField] private int maxLevelCount = 3;

    [Header("Level Select")]
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private LevelButtonData[] levelButtons;

    [Header("Title Animation")]
    [SerializeField] private RectTransform titleTransform;
    [SerializeField] private float pulseSpeed = 1.5f;
    [SerializeField] private float pulseAmount = 0.08f;

    private Vector3 titleBaseScale = Vector3.one;

    private void Start()
    {
        AutoWireReferencesIfNeeded();

        if (titleTransform != null)
        {
            titleBaseScale = titleTransform.localScale;
            StartCoroutine(AnimateTitlePulseRoutine());
        }

        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }

        RefreshLevelButtons();
    }

    private void AutoWireReferencesIfNeeded()
    {
        if (titleTransform == null)
        {
            GameObject titleGo = GameObject.Find("Title");
            if (titleGo != null)
            {
                titleTransform = titleGo.GetComponent<RectTransform>();
            }
        }

        if (levelSelectPanel == null)
        {
            levelSelectPanel = GameObject.Find("LevelSelectPanel");
        }

        if (levelButtons == null || levelButtons.Length < 3)
        {
            levelButtons = new LevelButtonData[3];
        }

        AssignLevelButtonData(0, 1, "Level1Button");
        AssignLevelButtonData(1, 2, "Level2Button");
        AssignLevelButtonData(2, 3, "Level3Button");

        BindButtonIfPresent("PlayButton", OnPlayPressed);
        BindButtonIfPresent("LevelSelectButton", OnOpenLevelSelectPressed);
        BindButtonIfPresent("QuitButton", OnQuitPressed);
        BindButtonIfPresent("CloseLevelSelectButton", OnCloseLevelSelectPressed);
        BindLevelButtonIfPresent("Level1Button", 1);
        BindLevelButtonIfPresent("Level2Button", 2);
        BindLevelButtonIfPresent("Level3Button", 3);
    }

    public void OnPlayPressed()
    {
        AudioManager.Instance?.PlayUIClickSfx();
        int targetLevel = playHighestUnlockedLevel
            ? LevelManager.GetHighestUnlockedLevel(maxLevelCount)
            : fallbackPlayLevel;
        SceneLoader.LoadScene($"Level{Mathf.Clamp(targetLevel, 1, maxLevelCount)}");
    }

    public void OnOpenLevelSelectPressed()
    {
        AudioManager.Instance?.PlayUIClickSfx();
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
        }
        RefreshLevelButtons();
    }

    public void OnCloseLevelSelectPressed()
    {
        AudioManager.Instance?.PlayUIClickSfx();
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }
    }

    public void OnLevelSelected(int levelIndex)
    {
        AudioManager.Instance?.PlayUIClickSfx();
        int highestUnlocked = LevelManager.GetHighestUnlockedLevel(maxLevelCount);
        if (levelIndex > highestUnlocked)
        {
            return;
        }

        SceneLoader.LoadScene($"Level{Mathf.Clamp(levelIndex, 1, maxLevelCount)}");
    }

    public void OnQuitPressed()
    {
        AudioManager.Instance?.PlayUIClickSfx();
        Application.Quit();
    }

    private void RefreshLevelButtons()
    {
        if (levelButtons == null)
        {
            return;
        }

        int highestUnlocked = LevelManager.GetHighestUnlockedLevel(maxLevelCount);
        foreach (LevelButtonData levelButton in levelButtons)
        {
            if (levelButton == null || levelButton.button == null)
            {
                continue;
            }

            bool unlocked = levelButton.levelIndex <= highestUnlocked;
            levelButton.button.interactable = unlocked;

            if (levelButton.lockOverlay != null)
            {
                levelButton.lockOverlay.SetActive(!unlocked);
            }

            if (levelButton.label != null)
            {
                levelButton.label.text = $"Level {levelButton.levelIndex}";
            }
        }
    }

    private IEnumerator AnimateTitlePulseRoutine()
    {
        while (true)
        {
            float wave = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            titleTransform.localScale = titleBaseScale * (1f + wave);
            yield return null;
        }
    }

    private void AssignLevelButtonData(int index, int level, string buttonName)
    {
        if (index < 0 || index >= levelButtons.Length)
        {
            return;
        }

        if (levelButtons[index] == null)
        {
            levelButtons[index] = new LevelButtonData();
        }

        GameObject buttonGo = GameObject.Find(buttonName);
        if (buttonGo == null)
        {
            return;
        }

        levelButtons[index].levelIndex = level;
        levelButtons[index].button = buttonGo.GetComponent<Button>();
        levelButtons[index].label = buttonGo.GetComponentInChildren<TextMeshProUGUI>();

        Transform lockOverlay = buttonGo.transform.Find("LockOverlay");
        if (lockOverlay != null)
        {
            levelButtons[index].lockOverlay = lockOverlay.gameObject;
        }
    }

    private void BindButtonIfPresent(string buttonName, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonGo = GameObject.Find(buttonName);
        if (buttonGo == null)
        {
            return;
        }

        Button button = buttonGo.GetComponent<Button>();
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    private void BindLevelButtonIfPresent(string buttonName, int levelIndex)
    {
        GameObject buttonGo = GameObject.Find(buttonName);
        if (buttonGo == null)
        {
            return;
        }

        Button button = buttonGo.GetComponent<Button>();
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { OnLevelSelected(levelIndex); });
    }
}

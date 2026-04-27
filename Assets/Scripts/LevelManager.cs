using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public const string HighestUnlockedLevelKey = "HighestUnlockedLevel";

    [Header("Level Flow")]
    [SerializeField] private int maxLevelCount = 3;
    [SerializeField] private float transitionDelay = 2f;
    [SerializeField] private bool loopAfterLastLevel = true;
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private string levelScenePrefix = "Level";

    public int CurrentLevel { get; private set; } = 1;

    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CurrentLevel = Mathf.Clamp(GetCurrentLevelFromSceneName(), 1, maxLevelCount);
        EnsureProgressIsInitialized();
    }

    private void Start()
    {
        StartCoroutine(BindToLanderRoutine());
    }

    private void OnDestroy()
    {
        if (Lander.Instance != null)
        {
            Lander.Instance.onLanded -= Lander_OnLanded;
        }
    }

    private IEnumerator BindToLanderRoutine()
    {
        while (Lander.Instance == null)
        {
            yield return null;
        }
        Lander.Instance.onLanded += Lander_OnLanded;
    }

    public void LoadLevel(int levelIndex)
    {
        int clampedLevel = Mathf.Clamp(levelIndex, 1, maxLevelCount);
        CurrentLevel = clampedLevel;
        string sceneName = $"{levelScenePrefix}{clampedLevel}";
        SceneLoader.LoadScene(sceneName);
    }

    public void LoadNextLevel()
    {
        int nextLevel = CurrentLevel + 1;
        if (nextLevel > maxLevelCount)
        {
            if (!string.IsNullOrWhiteSpace(winSceneName) && !loopAfterLastLevel)
            {
                SceneLoader.LoadScene(winSceneName);
                return;
            }

            nextLevel = 1;
        }

        LoadLevel(nextLevel);
    }

    public void RestartCurrentLevel()
    {
        LoadLevel(CurrentLevel);
    }

    public static int GetHighestUnlockedLevel(int maxLevel = 3)
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(HighestUnlockedLevelKey, 1), 1, maxLevel);
    }

    public static void UnlockLevel(int level, int maxLevel = 3)
    {
        int highest = GetHighestUnlockedLevel(maxLevel);
        int newHighest = Mathf.Clamp(Mathf.Max(highest, level), 1, maxLevel);
        PlayerPrefs.SetInt(HighestUnlockedLevelKey, newHighest);
        PlayerPrefs.Save();
    }

    private void Lander_OnLanded(object sender, Lander.LandedEventArgs e)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(HandleLandingResultRoutine(e));
    }

    private IEnumerator HandleLandingResultRoutine(Lander.LandedEventArgs e)
    {
        yield return new WaitForSeconds(transitionDelay);

        if (e.landingType == Lander.LandingType.Sucess)
        {
            UnlockLevel(CurrentLevel + 1, maxLevelCount);
            LoadNextLevel();
        }
        else
        {
            RestartCurrentLevel();
        }
    }

    private int GetCurrentLevelFromSceneName()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith(levelScenePrefix))
        {
            string numberPart = sceneName.Substring(levelScenePrefix.Length);
            if (int.TryParse(numberPart, out int level))
            {
                return level;
            }
        }
        return 1;
    }

    private void EnsureProgressIsInitialized()
    {
        if (!PlayerPrefs.HasKey(HighestUnlockedLevelKey))
        {
            PlayerPrefs.SetInt(HighestUnlockedLevelKey, 1);
            PlayerPrefs.Save();
        }
    }
}

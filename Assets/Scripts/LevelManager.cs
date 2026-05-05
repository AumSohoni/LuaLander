using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private GameLevel[] levels;
    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        AutoWireLevels();
    }

    private void Start()
    {
        AutoWireLevels();
        ActivateLevel(currentLevelIndex);
    }

    public void OnSuccessfulLanding()
    {
        if (currentLevelIndex < levels.Length - 1)
        {
            currentLevelIndex++;
            ActivateLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }

    public void RestartLevel()
    {
        ActivateLevel(currentLevelIndex);
    }

    public void ResetCurrentLevel()
    {
        ActivateLevel(currentLevelIndex);
    }

    private void ActivateLevel(int levelIndex)
    {
        if (levels == null || levels.Length == 0)
        {
            return;
        }

        currentLevelIndex = Mathf.Clamp(levelIndex, 0, levels.Length - 1);

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == null)
            {
                continue;
            }

            if (i == currentLevelIndex)
            {
                levels[i].ActivateLevel();
            }
            else
            {
                levels[i].DeactivateLevel();
            }
        }
    }

    private void AutoWireLevels()
    {
        if (levels != null)
        {
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i] != null)
                {
                    return;
                }
            }
        }

        GameLevel[] foundLevels = FindObjectsByType<GameLevel>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (foundLevels == null || foundLevels.Length == 0)
        {
            Debug.LogError("LevelManager could not find any GameLevel objects in the scene.");
            return;
        }

        System.Array.Sort(foundLevels, (a, b) => a.LevelIndex.CompareTo(b.LevelIndex));
        levels = foundLevels;
    }
}

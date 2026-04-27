using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private const string DefaultLoadingSceneName = "LoadingScene";
    private static string pendingSceneName;
    private static string loadingSceneName = DefaultLoadingSceneName;

    public static string PendingSceneName => pendingSceneName;

    public static void SetLoadingScene(string sceneName)
    {
        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            loadingSceneName = sceneName;
        }
    }

    public static void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("SceneLoader.LoadScene called with empty scene name.");
            return;
        }

        pendingSceneName = sceneName;
        if (Application.CanStreamedLevelBeLoaded(loadingSceneName))
        {
            SceneManager.LoadScene(loadingSceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public static void LoadSceneByBuildIndex(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Invalid build index {buildIndex}.");
            return;
        }

        string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
        LoadScene(sceneName);
    }

    public static string ConsumePendingSceneName()
    {
        string value = pendingSceneName;
        pendingSceneName = null;
        return value;
    }
}

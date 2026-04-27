using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private float minimumDisplayTime = 0.25f;

    private void Start()
    {
        StartCoroutine(LoadTargetSceneRoutine());
    }

    private IEnumerator LoadTargetSceneRoutine()
    {
        string targetScene = SceneLoader.ConsumePendingSceneName();
        if (string.IsNullOrWhiteSpace(targetScene))
        {
            Debug.LogError("LoadingScreen opened without a pending target scene.");
            yield break;
        }

        float displayTimer = 0f;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            displayTimer += Time.deltaTime;
            float normalizedProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            if (progressBar != null)
            {
                progressBar.value = normalizedProgress;
            }

            if (progressText != null)
            {
                progressText.text = $"{Mathf.RoundToInt(normalizedProgress * 100f)}%";
            }

            if (asyncLoad.progress >= 0.9f && displayTimer >= minimumDisplayTime)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}

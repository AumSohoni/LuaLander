using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Build index of the game scene
    [SerializeField] private int gameSceneBuildIndex;

    // Method to load the game scene
    public void PlayGame()
    {
        if (gameSceneBuildIndex >= 0 && gameSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(gameSceneBuildIndex);
        }
        else
        {
            Debug.LogError("Invalid scene build index. Please check the assigned build index.");
        }
    }

    // Optional: Method to quit the game
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
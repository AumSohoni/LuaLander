using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private LandedUI landedUI;

    private int score;
    private float time;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(BindToLanderRoutine());
    }

    private void Update()
    {
        time += Time.deltaTime;
    }

    private void lander_Landed(object sender, Lander.LandedEventArgs e)
    {
        e.Score = AddScore(e.Score);
        OnLanded(e.landingType == Lander.LandingType.Sucess);
    }

    private void lander_CoinPickup(object sender, EventArgs e)
    {
        AddScore(500);
    }

    private int AddScore(int addScoreAmount)
    {
        score += addScoreAmount;
        return score;
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetRunState()
    {
        score = 0;
        time = 0f;
    }

    public float GetTime()
    {
        return time;
    }

    private void OnDestroy()
    {
        if (Lander.Instance != null)
        {
            Lander.Instance.onCoinPickup -= lander_CoinPickup;
            Lander.Instance.onLanded -= lander_Landed;
        }
    }

    private IEnumerator BindToLanderRoutine()
    {
        while (Lander.Instance == null)
        {
            yield return null;
        }

        Lander.Instance.onCoinPickup += lander_CoinPickup;
        Lander.Instance.onLanded += lander_Landed;
    }

    public void StartGame()
    {
        score = 0;
        time = 0f;

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
    }

    public void OnLanded(bool isSuccessful)
    {
        if (landedUI == null)
        {
            landedUI = FindFirstObjectByType<LandedUI>();
        }

        if (landedUI == null)
        {
            Debug.LogError("GameManager could not find LandedUI in the scene.");
            return;
        }

        landedUI.ShowLandingOutcome(isSuccessful);

        if (isSuccessful)
        {
            return;
        }

        Debug.Log("Crash detected! Showing Restart button.");
    }

    public void OnContinuePressed()
    {
        (levelManager != null ? levelManager : LevelManager.Instance)?.OnSuccessfulLanding();
        landedUI?.HideUI();
    }

    public void RestartLevel()
    {
        ResetRunState();
        landedUI?.HideUI();
        (levelManager != null ? levelManager : LevelManager.Instance)?.ResetCurrentLevel();
    }
}

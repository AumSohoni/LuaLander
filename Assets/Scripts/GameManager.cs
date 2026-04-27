using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

   public static GameManager Instance { get; private set; }

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
}

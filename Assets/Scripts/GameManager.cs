using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

   public static GameManager Instance { get; private set; }

    private int score;
    private float time;
    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        Lander.Instance.onCoinPickup += lander_CoinPickup;  
        Lander.Instance.onLanded += lander_Landed;
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
        Debug.Log($"Score: {score}");
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






}

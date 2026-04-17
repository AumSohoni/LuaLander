using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

   

    private int score;

    private void Start()
    {
        Lander.Instance.onCoinPickup += lander_CoinPickup;  
        Lander.Instance.onLanded += lander_Landed;
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








}

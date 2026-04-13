using UnityEngine;

public class LandingPad : MonoBehaviour
{
    [SerializeField] private float scoreMultiplier = 1f;

    public float GetScoreMultiplier()
    {
        return scoreMultiplier;
    }
}

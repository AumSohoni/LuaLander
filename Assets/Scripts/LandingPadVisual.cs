using TMPro;
using UnityEngine;

public class LandingPadVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreText;

    private void Awake()
    {
        LandingPad landingPad = GetComponentInParent<LandingPad>();
        if (landingPad != null)
        {
            float scoreMultiplier = landingPad.GetScoreMultiplier();
            scoreText.text = $"x{scoreMultiplier}";
        }
    }

}

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsTextMesh;
    [SerializeField] private GameObject speedUpArrow;
    [SerializeField] private GameObject speedDownArrow;
    [SerializeField] private GameObject speedLeftArrow;
    [SerializeField] private GameObject speedRightArrow;
    [SerializeField] private Image fuelBar;
    [SerializeField] private float fuelBarLerpSpeed = 8f;

    private float currentFuelFill = 1f;

    private void Update()
    {
        if (Lander.Instance == null || GameManager.Instance == null)
        {
            return;
        }
        UpdateStatsTextMesh();
    }

    private void UpdateStatsTextMesh()
    {
        speedUpArrow.SetActive(Lander.Instance.GetSpeedY() >= 0);
        speedDownArrow.SetActive(Lander.Instance.GetSpeedY() < 0);
        speedLeftArrow.SetActive(Lander.Instance.GetSpeedX() < 0);
        speedRightArrow.SetActive(Lander.Instance.GetSpeedX() > 0);



        statsTextMesh.text = GameManager.Instance.GetScore() + "\n" +
                             Mathf.Round(GameManager.Instance.GetTime()) + "\n" +
                             Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedX() * 10f)) + "\n" +
                             Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedY() * 10f)) + "\n";

        if (fuelBar != null)
        {
            float targetFuelFill = Lander.Instance.GetFuelAmountNormalized();
            currentFuelFill = Mathf.Lerp(currentFuelFill, targetFuelFill, Time.deltaTime * fuelBarLerpSpeed);
            fuelBar.fillAmount = currentFuelFill;
        }
    }
}


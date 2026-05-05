using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    [SerializeField] private GameObject landerStartPoint;

    public int LevelIndex => levelIndex;

    public void ActivateLevel()
    {
        gameObject.SetActive(true);
        ResetLevelState();
    }

    public void DeactivateLevel()
    {
        gameObject.SetActive(false);
    }

    public void ResetLevelState()
    {
        foreach (CoinPickup coinPickup in GetComponentsInChildren<CoinPickup>(true))
        {
            coinPickup.ResetPickup();
        }

        foreach (FuelPickup fuelPickup in GetComponentsInChildren<FuelPickup>(true))
        {
            fuelPickup.ResetPickup();
        }

        if (Lander.Instance != null)
        {
            Lander.Instance.ResetForLevel(landerStartPoint != null ? landerStartPoint.transform : null);
        }
    }
}

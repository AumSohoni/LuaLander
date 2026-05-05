using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public void DestroySelf()
    {
        gameObject.SetActive(false);
    }

    public void ResetPickup()
    {
        gameObject.SetActive(true);
    }
}

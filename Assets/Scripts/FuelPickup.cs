using UnityEngine;

public class FuelPickup : MonoBehaviour
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

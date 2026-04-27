using UnityEngine;
using Unity.Cinemachine;

public class ScreenShake : MonoBehaviour
{
    private static ScreenShake instance;

    [SerializeField] private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        instance = this;
        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }
    }

    public static void ShakeCamera(float intensity)
    {
        if (instance == null || instance.impulseSource == null)
        {
            return;
        }

        instance.impulseSource.GenerateImpulseWithForce(intensity);
    }
}

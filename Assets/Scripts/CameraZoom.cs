using UnityEngine;
using Unity.Cinemachine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D targetRigidbody;

    [Header("Zoom Settings")]
    [SerializeField] private float minOrthoSize = 8f;
    [SerializeField] private float maxOrthoSize = 25f;
    [SerializeField] private float zoomLerpSpeed = 2f;
    [SerializeField] private float speedForMaxZoom = 14f;
    [SerializeField] private float heightForMaxZoom = 20f;
    [SerializeField] private float groundY = 0f;

    private void Awake()
    {
        if (cinemachineCamera == null)
        {
            cinemachineCamera = GetComponent<CinemachineCamera>();
        }

        if (target == null && Lander.Instance != null)
        {
            target = Lander.Instance.transform;
        }

        if (targetRigidbody == null && target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody2D>();
        }
    }

    private void LateUpdate()
    {
        if (cinemachineCamera == null || target == null)
        {
            return;
        }

        if (Lander.Instance != null && !Lander.Instance.HasStarted)
        {
            SetZoom(maxOrthoSize);
            return;
        }

        float speed = targetRigidbody != null ? targetRigidbody.linearVelocity.magnitude : 0f;
        float altitude = Mathf.Max(0f, target.position.y - groundY);

        float speedFactor = Mathf.InverseLerp(0f, speedForMaxZoom, speed);
        float altitudeFactor = Mathf.InverseLerp(0f, heightForMaxZoom, altitude);
        float zoomFactor = Mathf.Max(speedFactor, altitudeFactor);
        float targetSize = Mathf.Lerp(minOrthoSize, maxOrthoSize, zoomFactor);

        SetZoom(targetSize);
    }

    private void SetZoom(float targetSize)
    {
        LensSettings lens = cinemachineCamera.Lens;
        lens.OrthographicSize = Mathf.Lerp(lens.OrthographicSize, targetSize, Time.deltaTime * zoomLerpSpeed);
        cinemachineCamera.Lens = lens;
    }
}

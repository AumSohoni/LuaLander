using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float lifetime = 1.2f;
    [SerializeField] private float upwardSpeed = 1.4f;

    private float timer;

    public void Initialize(int score)
    {
        if (textMesh != null)
        {
            textMesh.text = $"+{score}";
        }
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * upwardSpeed * Time.deltaTime;

        if (textMesh != null)
        {
            Color color = textMesh.color;
            color.a = Mathf.Clamp01(1f - (timer / lifetime));
            textMesh.color = color;
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}

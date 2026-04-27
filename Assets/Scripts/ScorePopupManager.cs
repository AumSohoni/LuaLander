using UnityEngine;

public class ScorePopupManager : MonoBehaviour
{
    public static ScorePopupManager Instance { get; private set; }

    [SerializeField] private GameObject scorePopupPrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 1.5f, 0f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowScorePopup(int score, Vector3 worldPosition)
    {
        if (scorePopupPrefab == null || score <= 0)
        {
            return;
        }

        GameObject popupGo = Instantiate(scorePopupPrefab, worldPosition + spawnOffset, Quaternion.identity);
        ScorePopup popup = popupGo.GetComponent<ScorePopup>();
        if (popup != null)
        {
            popup.Initialize(score);
        }
    }
}

using UnityEngine;
using TMPro;
using System.Collections;

public class LandedUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TitleText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private RectTransform panelRoot;
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private float hiddenYOffset = 700f;

    private Vector2 shownAnchoredPosition;
    private Vector2 hiddenAnchoredPosition;
    private Coroutine panelAnimationCoroutine;

    private void Start()
    {
        StartCoroutine(BindToLanderRoutine());
        if (panelRoot == null)
        {
            panelRoot = transform as RectTransform;
        }

        shownAnchoredPosition = panelRoot.anchoredPosition;
        hiddenAnchoredPosition = shownAnchoredPosition + Vector2.down * hiddenYOffset;
        panelRoot.anchoredPosition = hiddenAnchoredPosition;
        Hide();
    }

    private void Lander_OnLanded(object sender, Lander.LandedEventArgs e)
    {
        Show();
        if (statsText != null && TitleText != null)
        {
           if(e.landingType == Lander.LandingType.Sucess)
            {
                TitleText.text = "Successful Landing!";
            }
            else
            {
                TitleText.text = "<color=red>CRASH</color>";
            }

            statsText.text = $"{e.LandingSpeed:F1}\n" +
                             $" {e.LandingAngle:F2}\n" +
                             $"{e.Multiplier:F1}\n" +
                             $" {e.Score}";
        }

        if (panelAnimationCoroutine != null)
        {
            StopCoroutine(panelAnimationCoroutine);
        }
        panelAnimationCoroutine = StartCoroutine(AnimatePanelSlideIn());
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Lander.Instance != null)
        {
            Lander.Instance.onLanded -= Lander_OnLanded;
        }
    }

    private IEnumerator BindToLanderRoutine()
    {
        while (Lander.Instance == null)
        {
            yield return null;
        }
        Lander.Instance.onLanded += Lander_OnLanded;
    }

    private IEnumerator AnimatePanelSlideIn()
    {
        panelRoot.anchoredPosition = hiddenAnchoredPosition;
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            panelRoot.anchoredPosition = Vector2.LerpUnclamped(hiddenAnchoredPosition, shownAnchoredPosition, eased);
            yield return null;
        }
        panelRoot.anchoredPosition = shownAnchoredPosition;
    }
}

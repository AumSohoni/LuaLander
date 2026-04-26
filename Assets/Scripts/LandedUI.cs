using UnityEngine;
using TMPro;

public class LandedUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TitleText;
    [SerializeField] private TextMeshProUGUI statsText;

    private void Start()
    {
        Lander.Instance.onLanded += Lander_OnLanded;
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
}

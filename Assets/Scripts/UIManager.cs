using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpaceRocket
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider fuelSlider;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI speedText;

        [Header("References")]
        [SerializeField] private RocketController rocketController;
        [SerializeField] private FuelSystem fuelSystem;

        private void Start()
        {
            // Reset UI
            statusText.text = "";
            
            if (fuelSystem != null)
            {
                fuelSystem.OnFuelChanged += UpdateFuelUI;
            }
        }

        private void Update()
        {
            if (rocketController != null && speedText != null)
            {
                float speed = rocketController.GetCurrentSpeed();
                speedText.text = $"Speed: {speed:F2} m/s";
            }
        }

        public void UpdateFuelUI(float percentage)
        {
            if (fuelSlider != null)
            {
                fuelSlider.value = percentage;
            }
        }

        public void UpdateGameStateUI(GameState state)
        {
            switch (state)
            {
                case GameState.Playing:
                    statusText.text = "";
                    break;
                case GameState.Won:
                    statusText.text = "<color=green>SUCCESSFUL LANDING!</color>";
                    break;
                case GameState.Crashed:
                    statusText.text = "<color=red>ROCKET CRASHED!</color>";
                    break;
                case GameState.OutOfFuel:
                    statusText.text = "<color=yellow>OUT OF FUEL!</color>";
                    break;
            }
        }
    }
}

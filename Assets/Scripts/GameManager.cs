using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace SpaceRocket
{
    public enum GameState { Starting, Playing, Won, Crashed, OutOfFuel }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private FuelSystem fuelSystem;
        [SerializeField] private LandingManager landingManager;
        [SerializeField] private UIManager uiManager;

        private GameState currentState;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            SetGameState(GameState.Playing);

            // Subscribe to events
            fuelSystem.OnOutOfFuel += HandleOutOfFuel;
            landingManager.OnCrashed += HandleCrashed;
            landingManager.OnLandedSuccessfully += HandleWin;
        }

        public void SetGameState(GameState newState)
        {
            currentState = newState;
            uiManager.UpdateGameStateUI(newState);
            
            if (newState == GameState.Won || newState == GameState.Crashed || newState == GameState.OutOfFuel)
            {
                // Optionally stop the rocket's movement here or wait a few seconds then restart
                StartCoroutine(RestartAfterDelay(3f));
            }
        }

        private void HandleOutOfFuel()
        {
            if (currentState == GameState.Playing)
            {
                SetGameState(GameState.OutOfFuel);
            }
        }

        private void HandleCrashed()
        {
            if (currentState == GameState.Playing)
            {
                SetGameState(GameState.Crashed);
            }
        }

        private void HandleWin()
        {
            if (currentState == GameState.Playing)
            {
                SetGameState(GameState.Won);
            }
        }

        private IEnumerator RestartAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            RestartLevel();
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

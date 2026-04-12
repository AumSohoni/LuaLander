using System;
using UnityEngine;

namespace SpaceRocket
{
    public class FuelSystem : MonoBehaviour
    {
        [Header("Fuel Settings")]
        [SerializeField] private float maxFuel = 100f;
        [SerializeField] private float fuelConsumptionRate = 10f; // Amount of fuel consumed per second of thrust

        private float currentFuel;

        public event Action OnOutOfFuel;
        public event Action<float> OnFuelChanged;

        private void Awake()
        {
            currentFuel = maxFuel;
        }

        private void Start()
        {
            // Initial UI update
            OnFuelChanged?.Invoke(GetFuelPercentage());
        }

        public void ConsumeFuel(float deltaTime)
        {
            if (currentFuel > 0)
            {
                currentFuel -= fuelConsumptionRate * deltaTime;
                currentFuel = Mathf.Max(currentFuel, 0);

                OnFuelChanged?.Invoke(GetFuelPercentage());

                if (currentFuel <= 0)
                {
                    OnOutOfFuel?.Invoke();
                }
            }
        }

        public bool HasFuel()
        {
            return currentFuel > 0;
        }

        public float GetFuelPercentage()
        {
            return currentFuel / maxFuel;
        }

        public float GetCurrentFuel()
        {
            return currentFuel;
        }

        public void ResetFuel()
        {
            currentFuel = maxFuel;
            OnFuelChanged?.Invoke(GetFuelPercentage());
        }
    }
}

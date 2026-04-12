using UnityEngine;
using System;

namespace SpaceRocket
{
    public class LandingManager : MonoBehaviour
    {
        [Header("Landing Limits")]
        [SerializeField] private float maxSafeLandingSpeed = 3.0f;
        [SerializeField] private float maxSafeLandingAngle = 15.0f;

        [Header("References")]
        [SerializeField] private RocketController rocketController;

        public event Action OnCrashed;
        public event Action OnLandedSuccessfully;

        private bool hasFinished = false;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (hasFinished) return;

            // Determine if the landing was safe or a crash
            if (collision.gameObject.CompareTag("LandingPad"))
            {
                CheckLanding(collision);
            }
            else
            {
                // Hitting anything else is a crash (terrain, obstacles, etc.)
                TriggerCrash("Hit terrain!");
            }
        }

        private void CheckLanding(Collision2D collision)
        {
            float impactSpeed = collision.relativeVelocity.magnitude;
            float currentAngle = Mathf.Abs(rocketController.GetCurrentRotation());

            Debug.Log($"Impact Speed: {impactSpeed:F2}, Angle: {currentAngle:F2}");

            if (impactSpeed <= maxSafeLandingSpeed && currentAngle <= maxSafeLandingAngle)
            {
                TriggerSafeLanding();
            }
            else
            {
                if (impactSpeed > maxSafeLandingSpeed)
                {
                    TriggerCrash("Landed too fast!");
                }
                else
                {
                    TriggerCrash("Landed at a sharp angle!");
                }
            }
        }

        private void TriggerSafeLanding()
        {
            hasFinished = true;
            Debug.Log("Safe Landing!");
            OnLandedSuccessfully?.Invoke();
        }

        private void TriggerCrash(string reason)
        {
            hasFinished = true;
            Debug.Log($"Crashed: {reason}");
            OnCrashed?.Invoke();
        }

        public void ResetLandingState()
        {
            hasFinished = false;
        }
    }
}

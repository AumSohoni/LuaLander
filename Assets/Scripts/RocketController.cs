using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceRocket
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RocketController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float thrustForce = 15f;
        [SerializeField] private float rotationTorque = 5f;

        [Header("References")]
        [SerializeField] private ParticleSystem mainThrusterParticles;
        [SerializeField] private FuelSystem fuelSystem;

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private bool isThrusting;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            // REALISTIC SPACE PHYSICS:
            // Remove all damping (air resistance/friction)
            rb.linearDamping = 0f; 
            rb.angularDamping = 0f;
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        private void Update()
        {
            // Determine if we are thrusting based on input and fuel availability
            isThrusting = moveInput.y > 0.1f && fuelSystem.HasFuel();

            UpdateParticles();
        }

        private void FixedUpdate()
        {
            HandleThrust();
            HandleRotation();
        }

        private void HandleThrust()
        {
            if (isThrusting)
            {
                // Apply thrust in the upward direction (relative to rocket's orientation)
                rb.AddRelativeForce(Vector2.up * thrustForce);
                
                // Consume fuel
                fuelSystem.ConsumeFuel(Time.fixedDeltaTime);
            }
        }

        private void HandleRotation()
        {
            // REALISTIC SPACE ROTATION:
            // Use AddTorque to simulate small RCS (Reaction Control System) thrusters.
            // This means the rocket will keep spinning until you add torque in the opposite direction.
            if (moveInput.x != 0)
            {
                rb.AddTorque(-moveInput.x * rotationTorque);
            }
        }

        private void UpdateParticles()
        {
            if (mainThrusterParticles != null)
            {
                var emission = mainThrusterParticles.emission;
                emission.enabled = isThrusting;
            }
        }

        public float GetCurrentSpeed()
        {
            return rb.linearVelocity.magnitude;
        }

        public float GetCurrentRotation()
        {
            // Normalize rotation to 0-360 range
            float angle = transform.eulerAngles.z;
            return angle > 180 ? angle - 360 : angle;
        }
    }
}

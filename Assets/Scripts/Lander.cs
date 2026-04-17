using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
public class Lander : MonoBehaviour
{
    public event EventHandler onLeftForce;
    public event EventHandler onRightForce;
    public event EventHandler onMiddleForce;
    public event EventHandler onBeforeForce;

    private Rigidbody2D lanbderRb;
    [SerializeField] private float Force = 700f;
    [SerializeField] private float turnSpeed = 100f;
    private const float MaxLandingVelocity = 4f;
    private const float MinLandingAlignment = 0.9f; 
    private const float MaxScorePerCategory = 100f;

    private float fuelAmount = 10f;
    private void Awake()
    {
        
        lanbderRb = GetComponent<Rigidbody2D>();   
    }
    
    private void FixedUpdate()
    {
        onBeforeForce?.Invoke(this, EventArgs.Empty);
        Debug.Log($"Fuel: {fuelAmount:F2}");
        if (fuelAmount <= 0)
        {
            return;
        }

        if(Keyboard.current.wKey.isPressed || 
           Keyboard.current.aKey.isPressed ||
           Keyboard.current.dKey.isPressed)
        {
            ConsumeFuel();
        }
        
        if (Keyboard.current.wKey.isPressed)
        {

                lanbderRb.AddForce(transform.up * Force * Time.deltaTime);
                onMiddleForce?.Invoke(this, EventArgs.Empty);
        }
         if(Keyboard.current.aKey.isPressed)
        {
            onLeftForce?.Invoke(this, EventArgs.Empty);
            lanbderRb.AddTorque(turnSpeed * Time.deltaTime);
        }
         if(Keyboard.current.dKey.isPressed)
        {
            onRightForce?.Invoke(this, EventArgs.Empty);    
            lanbderRb.AddTorque(-turnSpeed * Time.deltaTime);
        }

    }

 

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Calculate Physics Data
        float velocityMag = collision.relativeVelocity.magnitude;
        float alignment = Vector2.Dot(Vector2.up, transform.up);
        if(collision.collider.TryGetComponent<LandingPad>(out var landingPad))
        {
            float scoreMultiplier = landingPad.GetScoreMultiplier();
            int score = Mathf.RoundToInt((CalculateAngleScore(alignment) + CalculateSpeedScore(velocityMag)) * scoreMultiplier);
            Debug.Log($"Landed on Pad! Score: {score} (Multiplier: {scoreMultiplier})");
        }
        
    
        if (velocityMag > MaxLandingVelocity)
        {
            Debug.Log("Crashed: Too fast!");
            return;
        }

        if (alignment < MinLandingAlignment)
        {
            Debug.Log("Crashed: Too tilted!");
            return;
        }

        Debug.Log("Successful landing!");

        float angleScore = CalculateAngleScore(alignment);
        float speedScore = CalculateSpeedScore(velocityMag);

        
        Debug.Log($"Scores -> Angle: {angleScore} | Speed: {speedScore}");
        
    }


    private float CalculateAngleScore(float alignment)
    {
        // Remaps alignment (0.9 to 1.0) to a score (0 to 100)
        // 1.0 alignment = 100 points, 0.9 alignment = 0 points
        float score = (alignment - MinLandingAlignment) / (1.0f - MinLandingAlignment);
        return Mathf.Clamp(score * MaxScorePerCategory, 0, MaxScorePerCategory);
    }

   
    private float CalculateSpeedScore(float velocity)
    {
        // Lower speed = Higher score
        // 0 speed = 100 points, Max speed = 0 points
        float score = 1.0f - (velocity / MaxLandingVelocity);
        return Mathf.Clamp(score * MaxScorePerCategory, 0, MaxScorePerCategory);
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.TryGetComponent<FuelPickup>(out var fuelPickup))
        {
            Debug.Log("Fuel pickup collected!");
            float addFuelAmount = 10f;
            fuelAmount += addFuelAmount;
            fuelPickup.DestroySelf();
        }
       
    }


    private void ConsumeFuel()
    {
        float fuelConsumed = 1f; // Adjust as needed
        fuelAmount -= fuelConsumed * Time.deltaTime;
    }
}



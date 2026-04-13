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
    private const float MinLandingAlignment = 0.9f; // Roughly 25 degrees
    private const float MaxScorePerCategory = 100f;
    private void Awake()
    {
        
        lanbderRb = GetComponent<Rigidbody2D>();   
    }
    
    private void FixedUpdate()
    {
        onBeforeForce?.Invoke(this, EventArgs.Empty);
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
        
        // 2. Check for Failure Conditions
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

        // 3. Successful Landing - Calculate Scores
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
}



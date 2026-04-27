using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Lander : MonoBehaviour
{
    public static Lander Instance { get; private set; }

    public event EventHandler onLeftForce;
    public event EventHandler onRightForce;
    public event EventHandler onMiddleForce;
    public event EventHandler onBeforeForce;
    public event EventHandler onCoinPickup;
    public event EventHandler<LandedEventArgs> onLanded;

    public class LandedEventArgs : EventArgs
    {
        public LandingType landingType;
        public int Score { get; set; }
        public float LandingSpeed { get; set; }
        public float LandingAngle { get; set; }
        public float Multiplier { get; set; }
        public Vector3 LandingPosition { get; set; }
        public bool LandedOnPad { get; set; }
    }

    public enum LandingType
    {
        Sucess,
        WrongLandingAngle,
        TooSteepAngle,
        TooFastLanding
    }

    [Header("Movement")]
    [SerializeField, FormerlySerializedAs("Force")] private float force = 700f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("Fuel")]
    [SerializeField] private float startingFuel = 10f;
    [SerializeField] private float fuelConsumePerSecond = 1f;

    [Header("Particles")]
    [SerializeField] private GameObject explosionParticleSystemPrefab;
    [SerializeField] private GameObject landingParticleSystemPrefab;

    [Header("Screen Shake")]
    [SerializeField] private float crashShakeIntensity = 1f;

    private const float MaxLandingVelocity = 4f;
    private const float MinLandingAlignment = 0.9f;
    private const float MaxScorePerCategory = 100f;

    private Rigidbody2D landerRb;
    private float defaultGravityScale;
    private float fuelAmount;
    private float maxFuel;
    private float coinAmount;
    private bool hasLanded;
    private bool hasStarted;

    public float FuelAmount => fuelAmount;
    public float CoinAmount => coinAmount;
    public float CurrentThrustInput { get; private set; }
    public bool HasLanded => hasLanded;
    public bool HasStarted => hasStarted;

    private void Awake()
    {
        Instance = this;
        landerRb = GetComponent<Rigidbody2D>();
        defaultGravityScale = landerRb.gravityScale;
        landerRb.gravityScale = 0f;
        fuelAmount = startingFuel;
        maxFuel = startingFuel;
        if (inputHandler == null)
        {
            inputHandler = GetComponent<PlayerInputHandler>();
        }
    }

    private void FixedUpdate()
    {
        onBeforeForce?.Invoke(this, EventArgs.Empty);

        if (hasLanded || fuelAmount <= 0f)
        {
            CurrentThrustInput = 0f;
            AudioManager.Instance?.SetThrusterActive(false);
            return;
        }

        float thrustInput = inputHandler != null ? inputHandler.ThrustInput : 0f;
        float rotateInput = inputHandler != null ? inputHandler.RotateInput : 0f;

        CurrentThrustInput = Mathf.Clamp01(thrustInput);

        if (CurrentThrustInput > 0.01f || Mathf.Abs(rotateInput) > 0.01f)
        {
            ConsumeFuel();
        }

        if (fuelAmount <= 0f)
        {
            CurrentThrustInput = 0f;
            AudioManager.Instance?.SetThrusterActive(false);
            return;
        }

        if (CurrentThrustInput > 0.01f)
        {
            landerRb.AddForce(transform.up * force * CurrentThrustInput * Time.fixedDeltaTime);
            onMiddleForce?.Invoke(this, EventArgs.Empty);
            AudioManager.Instance?.SetThrusterActive(true, CurrentThrustInput);
        }
        else
        {
            AudioManager.Instance?.SetThrusterActive(false);
        }

        if (rotateInput < -0.01f)
        {
            onLeftForce?.Invoke(this, EventArgs.Empty);
            landerRb.AddTorque(turnSpeed * -rotateInput * Time.fixedDeltaTime);
        }
        else if (rotateInput > 0.01f)
        {
            onRightForce?.Invoke(this, EventArgs.Empty);
            landerRb.AddTorque(-turnSpeed * rotateInput * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasLanded)
        {
            return;
        }

        float velocityMag = collision.relativeVelocity.magnitude;
        float alignment = Vector2.Dot(Vector2.up, transform.up);
        bool collidedWithLandingPad = collision.collider.TryGetComponent<LandingPad>(out var landingPad);
        float multiplier = collidedWithLandingPad ? landingPad.GetScoreMultiplier() : 1f;
        Vector3 landingPosition = collision.contactCount > 0
            ? (Vector3)collision.GetContact(0).point
            : collision.transform.position;

        if (velocityMag > MaxLandingVelocity)
        {
            HandleCrash(collision, collidedWithLandingPad, LandingType.TooFastLanding, velocityMag, alignment, multiplier, landingPosition);
            return;
        }

        if (alignment < MinLandingAlignment)
        {
            LandingType crashType = alignment < 0 ? LandingType.TooSteepAngle : LandingType.WrongLandingAngle;
            HandleCrash(collision, collidedWithLandingPad, crashType, velocityMag, alignment, multiplier, landingPosition);
            return;
        }

        float angleScore = CalculateAngleScore(alignment);
        float speedScore = CalculateSpeedScore(velocityMag);
        int finalScore = Mathf.RoundToInt((angleScore + speedScore) * multiplier);

        hasLanded = true;
        AudioManager.Instance?.SetThrusterActive(false);
        AudioManager.Instance?.PlayLandingSuccessSfx();

        SpawnLandingParticles(collidedWithLandingPad ? landingPad.transform.position : landingPosition);
        if (collidedWithLandingPad)
        {
            ScorePopupManager.Instance?.ShowScorePopup(finalScore, landingPad.transform.position);
        }

        onLanded?.Invoke(this, new LandedEventArgs
        {
            landingType = LandingType.Sucess,
            Score = finalScore,
            LandingSpeed = velocityMag,
            LandingAngle = alignment,
            Multiplier = multiplier,
            LandingPosition = landingPosition,
            LandedOnPad = collidedWithLandingPad
        });
    }

    private void HandleCrash(
        Collision2D collision,
        bool collidedWithLandingPad,
        LandingType crashType,
        float velocityMag,
        float alignment,
        float multiplier,
        Vector3 landingPosition)
    {
        hasLanded = true;
        AudioManager.Instance?.SetThrusterActive(false);
        AudioManager.Instance?.PlayCrashSfx();

        TriggerTerrainCrashExplosion(collision, collidedWithLandingPad);
        ScreenShake.ShakeCamera(crashShakeIntensity);

        onLanded?.Invoke(this, new LandedEventArgs
        {
            landingType = crashType,
            Score = 0,
            LandingSpeed = velocityMag,
            LandingAngle = alignment,
            Multiplier = multiplier,
            LandingPosition = landingPosition,
            LandedOnPad = collidedWithLandingPad
        });
    }

    private float CalculateAngleScore(float alignment)
    {
        float score = (alignment - MinLandingAlignment) / (1.0f - MinLandingAlignment);
        return Mathf.Clamp(score * MaxScorePerCategory, 0, MaxScorePerCategory);
    }

    private float CalculateSpeedScore(float velocity)
    {
        float score = 1.0f - (velocity / MaxLandingVelocity);
        return Mathf.Clamp(score * MaxScorePerCategory, 0, MaxScorePerCategory);
    }

    private void SpawnLandingParticles(Vector3 spawnPosition)
    {
        if (landingParticleSystemPrefab == null)
        {
            return;
        }

        Instantiate(landingParticleSystemPrefab, spawnPosition, Quaternion.identity);
    }

    private void TriggerTerrainCrashExplosion(Collision2D collision, bool collidedWithLandingPad)
    {
        if (collidedWithLandingPad || explosionParticleSystemPrefab == null)
        {
            return;
        }

        Vector2 impactPoint = collision.contactCount > 0 ? collision.GetContact(0).point : collision.transform.position;
        Vector3 spawnPosition = new Vector3(impactPoint.x, impactPoint.y, transform.position.z);
        Instantiate(explosionParticleSystemPrefab, spawnPosition, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.TryGetComponent<FuelPickup>(out var fuelPickup))
        {
            float addFuelAmount = 10f;
            fuelAmount += addFuelAmount;
            if (fuelAmount > maxFuel)
            {
                maxFuel = fuelAmount;
            }
            fuelPickup.DestroySelf();
        }

        if (collider2D.TryGetComponent<CoinPickup>(out var coinPickup))
        {
            coinAmount++;
            onCoinPickup?.Invoke(this, EventArgs.Empty);
            AudioManager.Instance?.PlayCoinSfx();
            coinPickup.DestroySelf();
        }
    }

    private void ConsumeFuel()
    {
        fuelAmount = Mathf.Max(0f, fuelAmount - fuelConsumePerSecond * Time.fixedDeltaTime);
    }

    public void BeginGameplay()
    {
        if (hasStarted)
        {
            return;
        }

        hasStarted = true;
        if (landerRb != null)
        {
            landerRb.gravityScale = defaultGravityScale;
        }
    }

    public float GetFuelAmount()
    {
        return fuelAmount;
    }

    public float GetFuelAmountNormalized()
    {
        return maxFuel <= 0f ? 0f : Mathf.Clamp01(fuelAmount / maxFuel);
    }

    public float GetSpeedX()
    {
        return landerRb.linearVelocity.x;
    }

    public float GetSpeedY()
    {
        return landerRb.linearVelocity.y;
    }
}

using System;
using UnityEngine;

public class LanderVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem leftThruster;
    [SerializeField] private ParticleSystem rightThruster;
    [SerializeField] private ParticleSystem middleThruster;
    [SerializeField] private float minEmissionMultiplier = 10f;
    [SerializeField] private float maxEmissionMultiplier = 40f;

    private Lander lander;

    private void Awake()
    {
        lander = GetComponentInParent<Lander>();
        lander.onBeforeForce += Lander_onBeforeForce;
        lander.onMiddleForce += Lander_onMiddleForce;
        lander.onLeftForce += Lander_onLeftForce;
        lander.onRightForce += Lander_onRightForce;
        SetEnabledThrusterParticleSystem(middleThruster, false);
        SetEnabledThrusterParticleSystem(leftThruster, false);
        SetEnabledThrusterParticleSystem(rightThruster, false);
    }

    private void Update()
    {
        float thrustIntensity = lander != null ? lander.CurrentThrustInput : 0f;
        ApplyThrusterEmission(middleThruster, thrustIntensity);
        ApplyThrusterEmission(leftThruster, thrustIntensity);
        ApplyThrusterEmission(rightThruster, thrustIntensity);
    }

    private void Lander_onBeforeForce(object sender, EventArgs e)
    {
        SetEnabledThrusterParticleSystem(middleThruster, false);
        SetEnabledThrusterParticleSystem(leftThruster, false);
        SetEnabledThrusterParticleSystem(rightThruster, false);
    }

    private void Lander_onLeftForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(rightThruster, true);
    }
    private void Lander_onRightForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(leftThruster, true);
    }
    private void Lander_onMiddleForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(middleThruster, true);
        SetEnabledThrusterParticleSystem(leftThruster, true);  
        SetEnabledThrusterParticleSystem(rightThruster, true);
    }

    private void SetEnabledThrusterParticleSystem(ParticleSystem particleSystem, bool enabled)
    {
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.enabled = enabled;
    }

    private void ApplyThrusterEmission(ParticleSystem particleSystem, float thrustIntensity)
    {
        if (particleSystem == null)
        {
            return;
        }

        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTimeMultiplier = Mathf.Lerp(minEmissionMultiplier, maxEmissionMultiplier, thrustIntensity);
    }

    private void OnDestroy()
    {
        if (lander == null)
        {
            return;
        }

        lander.onBeforeForce -= Lander_onBeforeForce;
        lander.onMiddleForce -= Lander_onMiddleForce;
        lander.onLeftForce -= Lander_onLeftForce;
        lander.onRightForce -= Lander_onRightForce;
    }
}

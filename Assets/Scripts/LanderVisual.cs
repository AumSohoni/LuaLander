using System;
using UnityEngine;

public class LanderVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem leftThruster;
    [SerializeField] private ParticleSystem rightThruster;
    [SerializeField] private ParticleSystem middleThruster;

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
    
}
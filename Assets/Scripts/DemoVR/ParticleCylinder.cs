using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCylinder : MonoBehaviour {
    [SerializeField] private ParticleSystem particle;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            PlayStopParticle();
        }
    }

    public void ParticleIntensity(float value) {
        ParticleSystem.EmissionModule tmpEmission = this.particle.emission;
        tmpEmission.rateOverTime = value;
    }

    public void PlayStopParticle() {
        if (this.particle.isEmitting) {
            this.particle.Stop();
        } else {
            this.particle.Play();
        }
    }
}

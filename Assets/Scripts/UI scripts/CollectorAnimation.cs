﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorAnimation : MonoBehaviour
{
    public Transform target;
    public float force = 10f;
    // Start is called before the first frame update

    ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ps != null)
        {

            ParticleSystem.Particle[] particles =
                new ParticleSystem.Particle[ps.particleCount];

            ps.GetParticles(particles);

            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem.Particle p = particles[i];

                Vector3 particleWorldPosition;

                if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                {
                    particleWorldPosition = transform.TransformPoint(p.position);
                }
                else if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Custom)
                {
                    particleWorldPosition = ps.main.customSimulationSpace.TransformPoint(p.position);
                }
                else
                {
                    particleWorldPosition = p.position;
                }

                Vector3 directionToTarget = (target.position - particleWorldPosition).normalized;
                Vector3 seekForce = (directionToTarget * force) * Time.deltaTime;

                p.velocity += seekForce;

                particles[i] = p;
            }

            ps.SetParticles(particles, particles.Length);
        }
        if (ps == null )
        {
            Destroy(this);
        }

    }
}

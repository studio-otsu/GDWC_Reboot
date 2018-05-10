using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffProtection : BuffBase {

    float reduction = 1; // [0,1]
    private GameObject particle;

    public BuffProtection(float reduction) : base() {
        this.reduction = reduction;
        name = "Protected";
        description = "Cette unité subit des dégâts réduits ce tour.";
        description += "\nDégâts réduits de " + (int)Mathf.Round(reduction * 100) + " %";
    }

    public override void OnBuffStart(Unit target, Unit origin, Map map) {
        particle = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Particles/ShieldLoop"),
            target.transform.position, Quaternion.identity, target.transform);
    }

    public override void OnDamaged(Unit target, Unit origin, Map map, Unit damager, ref int amount) {
        amount -= (int)(amount * reduction);
    }

    public override void OnBuffEnd(Unit target, Unit origin, Map map) {
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        ps.Stop();
        GameObject.Destroy(particle, ps.main.duration/ps.main.simulationSpeed);
    }

}

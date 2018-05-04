using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffProtection : BuffBase {

    float reduction = 1; // [0,1]

    public BuffProtection(float reduction) : base() {
        this.reduction = reduction;
        name = "Protected";
        description = "Cette unité subit des dégâts réduits ce tour.";
        description += "\nDégâts réduits de " + (int)Mathf.Round(reduction * 100) + " %";
    }


    public override void OnDamaged(Unit target, Unit origin, Map map, Unit damager, ref int amount) {
        amount -= (int)(amount * reduction);
    }

}

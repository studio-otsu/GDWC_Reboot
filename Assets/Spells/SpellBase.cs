using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellBase {

    public string name;
    public string description;
    public int cooldownLight;
    public int cooldownHeavy;
    public AreaProfile rangeLight;
    public AreaProfile rangeHeavy;
    public bool lineOfSightLight;
    public bool lineOfSightHeavy;

    public abstract IEnumerator SolveSpellLight(Player caster, Cell target, Map map);
    public abstract IEnumerator SolveSpellHeavy(Player caster, Cell target, Map map);
}

public enum AreaType {
    Circle, Cross, Diagonal
}

public struct AreaProfile {
    public AreaProfile(AreaType type, int min, int max) {
        this.type = type;
        this.min = min;
        this.max = max;
    }

    AreaType type;
    int min;
    int max;
}

public struct PlayerSpell {
    SpellBase spell;
    int cooldown;

    bool isRecharging {
        get { return cooldown > 0; }
    }

    public IEnumerator SolveSpellLight(Player caster, Cell target, Map map) {
        return spell.SolveSpellLight(caster, target, map);
    }
    public IEnumerator SolveSpellHeavy(Player caster, Cell target, Map map) {
        return spell.SolveSpellHeavy(caster, target, map);
    }

}

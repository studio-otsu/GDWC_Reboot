using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellBase {

    public static int runningSpells = 0; // updated each time a spell starts and ends SolveSpellXXXXX(...)

    public string name;
    public string description;
    public string iconPath;
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

    public AreaType type;
    public int min;
    public int max;
}

public struct PlayerSpell {
    public SpellBase spell;
    public int cooldown;

    public bool isRecharging {
        get { return cooldown > 0; }
    }

    public IEnumerator SolveSpellLight(Player caster, Cell target, Map map) {
        return spell.SolveSpellLight(caster, target, map);
    }
    public IEnumerator SolveSpellHeavy(Player caster, Cell target, Map map) {
        return spell.SolveSpellHeavy(caster, target, map);
    }
}

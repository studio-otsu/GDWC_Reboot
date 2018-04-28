using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCac : SpellBase {
    public SpellCac() {
        name = "";
        description = "";
        rangeLight = new AreaProfile(AreaType.Cross, 1, 1);
        rangeHeavy = new AreaProfile(AreaType.Cross, 1, 1);
        cooldownLight = 0;
        cooldownHeavy = 0;
    }
    public override IEnumerator SolveSpellLight(Player caster, Cell target, Map map) {
        List<Cell> affectedCells = new List<Cell>();
        if (caster.currentCell.x != target.x) { // attack left or right
            
        } else if (caster.currentCell.x != target.x) { // attack up or down

        }
        foreach (Cell c in affectedCells) {
            // damage units
            if (c.currentUnit != null)
                c.currentUnit.Damage(20);
            // do pretty explosions. pew pew!
            // ...
        }
        yield return null;
    }
    public override IEnumerator SolveSpellHeavy(Player caster, Cell target, Map map) {
        List<Cell> affectedCells = null; // getfrom MAP
        foreach (Cell c in affectedCells) {
            if (c.currentUnit != null)
                c.currentUnit.Damage(30);
        }
        yield return null;
    }
}

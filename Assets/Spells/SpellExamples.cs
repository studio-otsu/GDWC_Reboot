using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAttackMelee : SpellBase {
    public SpellAttackMelee() {
        name = "Corps à corps";
        description = "Inflige 20 dégâts sur une ligne perpendiculaire de 1 case / Inflige 30 dégâts sur une ligne perpendiculaire de 1 case.";
        rangeLight = new AreaProfile(AreaType.Cross, 1, 1);
        rangeHeavy = new AreaProfile(AreaType.Cross, 1, 1);
        cooldownLight = 0;
        cooldownHeavy = 0;
    }
    public override IEnumerator SolveSpellLight(Player caster, Cell target, Map map) {
        List<Cell> affectedCells = null;
        if (caster.currentCell.x != target.x) { // attack left or right
            affectedCells = map.GetCellsVerticalLine(target, 0, 1);
        } else if (caster.currentCell.x != target.x) { // attack up or down
            affectedCells = map.GetCellsHorizontalLine(target, 0, 1);
        } else throw new System.Exception("Can't aim at the caster cell!");
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
        List<Cell> affectedCells = null;
        if (caster.currentCell.x != target.x) { // attack left or right
            affectedCells = map.GetCellsVerticalLine(target, 0, 1);
        } else if (caster.currentCell.x != target.x) { // attack up or down
            affectedCells = map.GetCellsHorizontalLine(target, 0, 1);
        } else throw new System.Exception("Can't aim at the caster cell!");
        foreach (Cell c in affectedCells) {
            // damage units
            if (c.currentUnit != null)
                c.currentUnit.Damage(30);
            // do pretty explosions. pew pew!
            // ...
        }
        yield return null;
    }
}

public class SpellAttackShort : SpellBase {
    public SpellAttackShort() {
        name = "Attaque courte";
        description = "Inflige 16 dégâts sur une case / Inflige 20 dégâts sur une case.";
        rangeLight = new AreaProfile(AreaType.Circle, 1, 4);
        rangeHeavy = new AreaProfile(AreaType.Circle, 1, 4);
        cooldownLight = 0;
        cooldownHeavy = 0;
    }
    public override IEnumerator SolveSpellLight(Player caster, Cell target, Map map) {
        List<Cell> affectedCells = map.GetCellsCross(target, 0, 0);
        foreach (Cell c in affectedCells) {
            // damage units
            if (c.currentUnit != null)
                c.currentUnit.Damage(16);
            // do pretty explosions. pew pew!
            // ...
        }
        yield return null;
    }
    public override IEnumerator SolveSpellHeavy(Player caster, Cell target, Map map) {
        List<Cell> affectedCells = map.GetCellsCross(target, 0, 0);
        foreach (Cell c in affectedCells) {
            // damage units
            if (c.currentUnit != null)
                c.currentUnit.Damage(20);
            // do pretty explosions. pew pew!
            // ...
        }
        yield return null;
    }
}
public class SpellAttackLarge : SpellBase {
    public SpellAttackLarge() {
        name = "Attaque large";
        description = "Inflige 12 dégâts dans une croix de 1 case / Inflige 16 dégâts dans une croix de 1 case.";
        rangeLight = new AreaProfile(AreaType.Circle, 2, 4);
        rangeHeavy = new AreaProfile(AreaType.Circle, 2, 4);
        cooldownLight = 1;
        cooldownHeavy = 1;
    }
    public override IEnumerator SolveSpellLight(Player caster, Cell target, Map map) {
        List<Cell> affectedCells = map.GetCellsCross(target, 0, 1);
        foreach (Cell c in affectedCells) {
            // damage units
            if (c.currentUnit != null)
                c.currentUnit.Damage(12);
            // do pretty explosions. pew pew!
            // ...
        }
        yield return null;
    }
    public override IEnumerator SolveSpellHeavy(Player caster, Cell target, Map map) {
        List<Cell> affectedCells = map.GetCellsCross(target, 0, 1);
        foreach (Cell c in affectedCells) {
            // damage units
            if (c.currentUnit != null)
                c.currentUnit.Damage(16);
            // do pretty explosions. pew pew!
            // ...
        }
        yield return null;
    }
}
public class SpellAttackLong : SpellBase {
    public SpellAttackLong() {
        name = "Attaque précise";
        description = "Inflige 20 dégâts sur une case / Inflige 30 dégâts sur uen case.";
        rangeLight = new AreaProfile(AreaType.Circle, 4, 7);
        rangeHeavy = new AreaProfile(AreaType.Circle, 4, 7);
        cooldownLight = 1;
        cooldownHeavy = 1;
    }
    public override IEnumerator SolveSpellLight(Player caster, Cell target, Map map) {
        List<Cell> affectedCells = map.GetCellsCross(target, 0, 0);
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
        List<Cell> affectedCells = map.GetCellsCross(target, 0, 0);
        foreach (Cell c in affectedCells) {
            // damage units
            if (c.currentUnit != null)
                c.currentUnit.Damage(30);
            // do pretty explosions. pew pew!
            // ...
        }
        yield return null;
    }
}

public class SpellDash : SpellBase {
    public SpellDash() {
        name = "Précipitation";
        description = "Avance de 2 cases / Avance de 3 cases.";
        rangeLight = new AreaProfile(AreaType.Cross, 2, 2);
        rangeHeavy = new AreaProfile(AreaType.Cross, 3, 3);
        cooldownLight = 3;
        cooldownHeavy = 3;
    }
    public override IEnumerator SolveSpellLight(Player caster, Cell target, Map map) {
        Map.MovePlayerToCell(caster,target);
        yield return null;
    }
    public override IEnumerator SolveSpellHeavy(Player caster, Cell target, Map map) {
        Map.MovePlayerToCell(caster, target);
        yield return null;
    }
}
public class SpellHeal : SpellBase {
    public SpellHeal() {
        name = "Soin";
        description = "Rend 8 points de vie / Rend 12 points de vie.";
        rangeLight = new AreaProfile(AreaType.Circle, 0, 0);
        rangeHeavy = new AreaProfile(AreaType.Circle, 0, 0);
        cooldownLight = 3;
        cooldownHeavy = 3;
    }
    public override IEnumerator SolveSpellLight(Player caster, Cell target, Map map) {
        caster.Heal(8);
        yield return null;
    }
    public override IEnumerator SolveSpellHeavy(Player caster, Cell target, Map map) {
        caster.Heal(12);
        yield return null;
    }
}

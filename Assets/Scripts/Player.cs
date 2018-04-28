using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    public Player() {
        currentAction.player = this;
        currentAction.move = new Cell[] { };
        spells = new PlayerSpell[]{
            new PlayerSpell() { spell = new SpellAttackMelee() },
            new PlayerSpell() { spell = new SpellAttackLarge() },
            new PlayerSpell() { spell = new SpellDash(),cooldown = 1 },
            new PlayerSpell() { spell = new SpellHeal(),cooldown = 2 }};
    }

    public TurnAction currentAction;

    public int mpCurrent = 3;
    public int mpMax = 5;

    public PlayerSpell[] spells;

    public void RegenMP(int amount) {
        mpCurrent += amount;
        if (mpCurrent > mpMax)
            mpCurrent = mpMax;
    }

    public void UseMP(int amount) {
        mpCurrent -= amount;
        if (mpCurrent < 0)
            mpCurrent = 0;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public Team team;

    public int shieldCurrent = 0;
    public int healthCurrent = 50;
    public int healthMax = 50;

    public Cell currentCell;

    public int x {
        get { return currentCell.x; }
    }
    public int y {
        get { return currentCell.y; }
    }


    protected new MeshRenderer renderer {
        get { return GetComponent<MeshRenderer>(); }
    }
    protected Material material {
        get { return renderer.material; }
    }
    protected Animator animator;


    public void ChangeMaxHealth(int amount) {
        healthMax = amount;
        if (healthCurrent > healthMax)
            healthCurrent = healthMax;
    }
    public void Damage(int amount) {
        turnDamage -= amount;
    }
    public void Heal(int amount) {
        turnHeal += amount;
    }
    public void Shield(int amount) {
        shieldCurrent += amount;
    }

    public int turnDamage = 0;
    public int turnHeal = 0;
    public void ApplyTurnDamageHeal() {
        int turnDelta = turnDamage + turnHeal;
        healthCurrent += turnDelta;
        if (healthCurrent > healthMax)
            healthCurrent = healthMax;
        if (healthCurrent < 0)
            healthCurrent = 0;
    }

    public Color teamColor {
        get { return material.color; }
        set { material.color = value; }
    }
}


public enum Team {
    TeamA,
    TeamB,
    Neutral
}

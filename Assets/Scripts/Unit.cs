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
        healthCurrent -= amount;
        if (healthCurrent < 0)
            healthCurrent = 0;
    }
    public void Heal(int amount) {
        healthCurrent += amount;
        if (healthCurrent > healthMax)
            healthCurrent = healthMax;
    }
    public void Shield(int amount) {
        shieldCurrent += amount;
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

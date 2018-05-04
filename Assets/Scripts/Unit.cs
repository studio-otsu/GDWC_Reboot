using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public Unit() {
        buffs = new List<UnitBuff>();
    }

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

    public bool isAlive {
        get { return healthCurrent > 0; }
    }


    protected new MeshRenderer renderer {
        get { return GetComponent<MeshRenderer>(); }
    }
    protected Material material {
        get { return renderer.material; }
    }
    protected Animator animator;

    #region BUFFS

    public List<UnitBuff> buffs;

    public void AddBuff(Unit origin, BuffBase buff, int duration, Map map) {
        buffs.Add(new UnitBuff() { origin = origin, buff = buff, remainingDuration = duration });
        buff.OnBuffStart(this, origin, map);
    }

    public void OnDamagedBuff(Map map, Unit damager, ref int amount) {
        foreach (UnitBuff buff in buffs) {
            buff.buff.OnDamaged(this, buff.origin, map, damager, ref amount);
        }
    }

    public void OnHealedBuff(Map map, Unit healer, ref int amount) {
        foreach (UnitBuff buff in buffs) {
            buff.buff.OnHealed(this, buff.origin, map, healer, ref amount);
        }
    }

    public void OnEndTurnBuff(Map map) {
        foreach (UnitBuff buff in buffs) {
            buff.buff.OnTurnEnd(this, buff.origin, map);
        }
    }

    public void DecreaseBuffDuration() {
        foreach (UnitBuff buff in buffs) {
            buff.DecreaseDuration();
        }
    }

    public void OnEndBuff(Map map) {
        foreach (UnitBuff buff in buffs) {
            if (buff.isDone)
                buff.buff.OnBuffEnd(this, buff.origin, map);
        }
        buffs.RemoveAll(x => x.isDone);
    }

    #endregion // BUFFS


    public void ChangeMaxHealth(int amount) {
        healthMax = amount;
        if (healthCurrent > healthMax)
            healthCurrent = healthMax;
    }
    public void Damage(int amount, Map map, Unit origin) {
        OnDamagedBuff(map, origin, ref amount);
        if (shieldCurrent > 0) { // in case of shield
            shieldCurrent -= amount; // consume shield
            amount = shieldCurrent < 0 ? -shieldCurrent : 0; // update effective damage
        }
        turnDamage -= amount;
    }
    public void Heal(int amount, Map map, Unit origin) {
        OnHealedBuff(map, origin, ref amount);
        turnHeal += amount;
    }
    public void Shield(int amount, Map map, Unit origin) {
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
        turnDamage = 0;
        turnHeal = 0;
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

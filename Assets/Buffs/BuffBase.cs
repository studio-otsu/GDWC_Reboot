using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase {

    public static int runningBuffs = 0; // updated each time a buff starts and ends OnXXXXX(...)

    public string name;
    public string description;
    public string iconPath;

    public bool durationIsCharge = false;
    public bool durationIsTurn = true;


    public virtual void OnBuffStart(Unit target, Unit origin, Map map) { }

    public virtual void OnDamaged(Unit target, Unit origin, Map map, Unit damager, ref int amount) { }
    public virtual void OnHealed(Unit target, Unit origin, Map map, Unit healer, ref int amount) { }

    public virtual void OnTurnEnd(Unit target, Unit origin, Map map) { }

    public virtual void OnBuffEnd(Unit target, Unit origin, Map map) { }

}

[System.Serializable]
public class UnitBuff {
    public BuffBase buff;
    public Unit origin;
    public int remainingDuration;

    public void OnBuffStart(Unit target, Unit origin, Map map) {
        buff.OnBuffStart(target, origin, map);
    }

    public void OnDamaged(Unit target, Unit origin, Map map, Unit damager, ref int amount) {
        if (remainingDuration > 0) {
            ConsumeCharge();
            buff.OnDamaged(target, origin, map, damager, ref amount);
        }
    }
    public void OnHealed(Unit target, Unit origin, Map map, Unit healer, ref int amount) {
        if (remainingDuration > 0) {
            ConsumeCharge();
            buff.OnHealed(target, origin, map, healer, ref amount);
        }
    }

    public void OnTurnEnd(Unit target, Unit origin, Map map) {
        if (remainingDuration != 0) {
            buff.OnTurnEnd(target, origin, map);
        }
    }

    public void OnBuffEnd(Unit target, Unit origin, Map map) {
        buff.OnBuffEnd(target, origin, map);
    }

    public void DecreaseDuration() {
        if (!buff.durationIsTurn) return;
        if (remainingDuration > 0) // don't reduce if duration < 0
            remainingDuration--;
    }

    public void ConsumeCharge() {
        if (!buff.durationIsCharge) return;
        remainingDuration--;
    }

    public bool isDone {
        get { return remainingDuration == 0; }
    }
}

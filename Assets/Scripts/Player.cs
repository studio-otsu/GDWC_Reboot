using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    public int mpCurrent = 3;
    public int mpMax = 5;

    //public Spell[] spells;

    public void UseMP(int amount) {
        mpCurrent -= amount;
        if (mpCurrent < 0)
            mpCurrent = 0;
    }


}

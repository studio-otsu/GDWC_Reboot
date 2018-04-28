using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TurnAction {
	public Cell[] move;
    public TurnSpell spell;
    public Player player;
}

public struct TurnSpell {
    public SpellBase spell;
    public Cell target;
}

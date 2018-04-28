using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TurnAction {
	public List<Cell> move;
    public TurnSpell spell;
    public Player player;
}

public struct TurnSpell {
    public PlayerSpell spell;
    public Cell target;
}

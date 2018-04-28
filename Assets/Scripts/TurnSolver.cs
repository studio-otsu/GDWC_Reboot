using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnSolver {

    public Match match;
    public Map map;

    public bool isSolvingMovements;

    public IEnumerator DoSolveMovements() {
        isSolvingMovements = true;
        for (int i = 0; i < 5; ++i) {
            int iteration = 0;
            while (!TryMovementStep(i)) {
                iteration++;
                if (iteration > 2)
                    throw new System.Exception("Possible infinite loop. sort that out you fool!");
            }
            CommitMovementStep(i);
            yield return new WaitForSeconds(0.25f);
        }
        isSolvingMovements = false;
    }

    private bool TryMovementStep(int step) {
        Dictionary<Cell, Player> nextPlayer = new Dictionary<Cell, Player>();
        List<Player> unmovablePlayers = new List<Player>();
        foreach (Player p in match.players) {
            if (p.currentAction.move.Count > step) {
                if (!nextPlayer.ContainsKey(p.currentAction.move[step]))
                    nextPlayer.Add(p.currentAction.move[step], p);
                else {
                    unmovablePlayers.Add(p);
                    if (!unmovablePlayers.Contains(nextPlayer[p.currentAction.move[step]]))
                        unmovablePlayers.Add(nextPlayer[p.currentAction.move[step]]);
                }
            } else {
                if (!nextPlayer.ContainsKey(p.currentCell))
                    nextPlayer.Add(p.currentCell, p);
                else {
                    unmovablePlayers.Add(p);
                    if (!unmovablePlayers.Contains(nextPlayer[p.currentCell]))
                        unmovablePlayers.Add(nextPlayer[p.currentCell]);
                }
            }
        }
        if (unmovablePlayers.Count > 0) {
            foreach (Player p in unmovablePlayers) {
                p.Damage(4);
                p.currentAction.move = new List<Cell>();
            }
            return false; // something would have gone wrong, should be corrected, but rerun just to be sure
        }
        return true; // nothing should go wrong, commit step
    }

    private void CommitMovementStep(int step) {
        foreach (Player p in match.players) {
            if (p.currentAction.move.Count > step) {
                Map.MovePlayerToAdjacentCell(p, p.currentAction.move[step]);
            }
        }
    }

    public bool isSolvingSpells;

    public IEnumerator DoSolveSpells() {
        isSolvingSpells = true;
        yield return null;
        isSolvingSpells = false;
    }



}

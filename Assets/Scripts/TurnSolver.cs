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
            if (!TryMovementStep(i))
                throw new System.Exception("something went wrong. sort that out you fool!");
            CommitMovementStep(i);
            yield return new WaitForSeconds(0.25f);
        }
        isSolvingMovements = false;
    }

    private struct Step {
        public Cell from;
        public Cell to;
        public Player p;
    }

    private bool Collide(Step a, Step b) {
        if (a.to == b.to) return true; // going to the same place
        if (a.to != a.from && b.to != b.from) { // both moving
            if (a.to == b.from && b.to == a.from) return true; // crossing
        }
        return false;
    }

    private bool TryMovementStep(int step) {
        List<Step> steps = new List<Step>();
        bool verified = false;
        int iterations = 0;
        while (!verified) {
            if (iterations > match.players.Count)
                return false; // justto be sure...
            iterations++;
            steps.Clear();
            verified = true; // if nothing goes wrong, we'll stop after this iteration
            foreach (Player p in match.players) {
                if (p.currentAction.move.Count > step) {
                    steps.Add(new Step() { from = p.currentCell, to = p.currentAction.move[step], p = p });
                } else {
                    steps.Add(new Step() { from = p.currentCell, to = p.currentCell, p = p });
                }
            }
            for (int i = 0; i < steps.Count; ++i) {
                Step a = steps[i];
                for (int j = i + 1; j < steps.Count; ++j) {
                    Step b = steps[j];
                    if (Collide(a, b)) { // DAMN SON
                        a.p.Damage(4);
                        if (a.to != a.from) { // if player was moving
                            a.p.currentAction.move.Clear();
                            //a.p.currentAction.spell.spell = a.p.currentAction.spell.target = null; // cancel spell?
                        }
                        b.p.Damage(4);
                        if (b.to != b.from) { // if player was moving
                            b.p.currentAction.move.Clear();
                            //b.p.currentAction.spell.spell = b.p.currentAction.spell.target = null; // cancel spell?
                        }
                        verified = false; // still need verifications
                        i = steps.Count; // cancel remaining verifications
                        break; // outa here
                    }
                }
            }
        }
        return true;
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
        foreach (Player p in match.players) {
            if (p.currentAction.spell.spell != null && p.currentAction.spell.target != null && !p.currentAction.spell.spell.isRecharging) {
                if (match.currentTurn % 2 == 0) {
                    match.StartCoroutine(p.currentAction.spell.spell.spell.SolveSpellHeavy(p, p.currentAction.spell.target, map));
                    p.currentAction.spell.spell.StartCooldownHeavy();
                } else {
                    match.StartCoroutine(p.currentAction.spell.spell.spell.SolveSpellLight(p, p.currentAction.spell.target, map));
                    p.currentAction.spell.spell.StartCooldownLight();
                }
                //Debug.Log("Using " + p.currentAction.spell.spell.spell.name + " spell!");
            }
        }
        yield return new WaitUntil(delegate { return SpellBase.runningSpells == 0; });
        isSolvingSpells = false;
    }

}

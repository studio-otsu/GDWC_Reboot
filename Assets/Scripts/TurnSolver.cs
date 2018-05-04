using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnSolver {

    public Match match;
    public Map map;

    public bool isSolvingMovements;

    public IEnumerator DoSolveMovements(bool dash = false) {
        isSolvingMovements = true;
        for (int i = 0; i < 5; ++i) {
            if (!TryMovementStep(i, dash))
                throw new System.Exception("something went wrong. sort that out you fool!");
            CommitMovementStep(i, dash);
            if (dash) {
                yield return new WaitForSeconds(0.10f);
            } else {
                yield return new WaitForSeconds(0.25f);
            }
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

    private bool TryMovementStep(int step, bool dash) {
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
                        if (!dash) // only damage mover during movement, not dashes
                            a.p.Damage(4, map, b.p);
                        if (a.to != a.from) { // if player was moving
                            int x = a.from.x - a.p.currentAction.move[a.p.currentAction.move.Count - 1].x;
                            int y = a.from.y - a.p.currentAction.move[a.p.currentAction.move.Count - 1].y;
                            a.p.currentAction.spell.target = map.GetCell(a.p.currentAction.spell.target.x + x,
                                a.p.currentAction.spell.target.y + y); // reaim spell
                            a.p.currentAction.move.Clear(); // clear movement
                        }
                        b.p.Damage(4, map, a.p);
                        if (b.to != b.from) { // if player was moving
                            int x = b.from.x - b.p.currentAction.move[b.p.currentAction.move.Count - 1].x;
                            int y = b.from.y - b.p.currentAction.move[b.p.currentAction.move.Count - 1].y;
                            b.p.currentAction.spell.target = map.GetCell(b.p.currentAction.spell.target.x + x,
                                b.p.currentAction.spell.target.y + y); // reaim spell
                            b.p.currentAction.move.Clear(); // clear movement
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

    private void CommitMovementStep(int step, bool dash) {
        foreach (Player p in match.players) {
            if (p.currentAction.move.Count > step) {
                Map.MovePlayerToAdjacentCell(p, p.currentAction.move[step]);
            }
        }
    }

    public bool isSolvingSpells;

    public IEnumerator DoSolveSpells() {
        isSolvingSpells = true;
        if (DoSolveSpellPriority0()) // protec
            yield return new WaitUntil(delegate { return SpellBase.runningSpells == 0; });
        if (DoSolveSpellPriority1()) // tp
            yield return new WaitUntil(delegate { return SpellBase.runningSpells == 0; });
        if (DoSolveSpellPriority2()) // dash
            yield return new WaitWhile(delegate { return isSolvingMovements; });
        if (DoSolveSpellPriority3()) // damage/heal/buff
            yield return new WaitUntil(delegate { return SpellBase.runningSpells == 0; });

        foreach (Player p in match.players) {
            p.OnEndTurnBuff(map);
            p.DecreaseBuffDuration();
            p.OnEndTurnBuff(map);
        }
        foreach (Player p in match.players) {
            p.ApplyTurnDamageHeal();
        }

        isSolvingSpells = false;
    }

    // protec
    public bool DoSolveSpellPriority0() {
        bool didSomething = false;
        foreach (Player p in match.players) {
            TurnSpell spell = p.currentAction.spell;
            if (spell.spell != null && spell.target != null && !spell.spell.isRecharging) {
                if (match.currentTurn % 2 == 0) {
                    if (spell.spell.spell.priorityHeavy == 0) {
                        match.StartCoroutine(spell.spell.spell.SolveSpellHeavy(p, spell.target, map));
                        spell.spell.StartCooldownHeavy();
                        didSomething = true;
                    }
                } else {
                    if (spell.spell.spell.priorityLight == 0) {
                        match.StartCoroutine(spell.spell.spell.SolveSpellLight(p, spell.target, map));
                        spell.spell.StartCooldownLight();
                        didSomething = true;
                    }
                }
            }
        }
        return didSomething;
    }

    // tp
    public bool DoSolveSpellPriority1() {
        bool didSomething = false;
        foreach (Player p in match.players) {
            TurnSpell spell = p.currentAction.spell;
            if (spell.spell != null && spell.target != null && !spell.spell.isRecharging) {
                if (match.currentTurn % 2 == 0) {
                    if (spell.spell.spell.priorityHeavy == 1) {
                        match.StartCoroutine(spell.spell.spell.SolveSpellHeavy(p, spell.target, map));
                        spell.spell.StartCooldownHeavy();
                        didSomething = true;
                    }
                } else {
                    if (spell.spell.spell.priorityLight == 1) {
                        match.StartCoroutine(spell.spell.spell.SolveSpellLight(p, spell.target, map));
                        spell.spell.StartCooldownLight();
                        didSomething = true;
                    }
                }
            }
        }
        return didSomething;
    }

    // dash
    public bool DoSolveSpellPriority2() {
        bool didSomething = false;
        foreach (Player p in match.players) {
            TurnSpell spell = p.currentAction.spell;
            if (spell.spell != null && spell.target != null && !spell.spell.isRecharging) {
                if (match.currentTurn % 2 == 0) {
                    if (spell.spell.spell.priorityHeavy == 2) {
                        match.StartCoroutine(spell.spell.spell.SolveSpellHeavy(p, spell.target, map));
                        spell.spell.StartCooldownHeavy();
                        didSomething = true;
                    }
                } else {
                    if (spell.spell.spell.priorityLight == 2) {
                        match.StartCoroutine(spell.spell.spell.SolveSpellLight(p, spell.target, map));
                        spell.spell.StartCooldownLight();
                        didSomething = true;
                    }
                }
            }
        }
        if (didSomething) match.StartCoroutine(DoSolveMovements(true));
        return didSomething;
    }

    // damage/heal/buff
    public bool DoSolveSpellPriority3() {
        bool didSomething = false;
        foreach (Player p in match.players) {
            TurnSpell spell = p.currentAction.spell;
            if (spell.spell != null && spell.target != null && !spell.spell.isRecharging) {
                if (match.currentTurn % 2 == 0) {
                    if (spell.spell.spell.priorityHeavy == 3) {
                        match.StartCoroutine(spell.spell.spell.SolveSpellHeavy(p, spell.target, map));
                        spell.spell.StartCooldownHeavy();
                        didSomething = true;
                    }
                } else {
                    if (spell.spell.spell.priorityLight == 3) {
                        match.StartCoroutine(spell.spell.spell.SolveSpellLight(p, spell.target, map));
                        spell.spell.StartCooldownLight();
                        didSomething = true;
                    }
                }
            }
        }
        return didSomething;
    }

}

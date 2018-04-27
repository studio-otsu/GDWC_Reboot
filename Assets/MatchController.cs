using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchController : MonoBehaviour {

    #region GUI_ELEMENTS

    public Text turnCounter;
    public Text turnPlayer;
    public Text turnTimer;
    public Button endTurn;
    public Button[] spells;

    #endregion // GUI_ELEMENTS

    public void OnClickCell(Cell c) {
    }
    public void OnHoverCell(Cell c) {
    }
    public void OnClickEndTurn() {
        match.EndCurrentPlayerTurn();
    }

    public Match match;
    public Map map;

    private List<Cell> showRangeCells;
    private List<Cell> showAreaCells;
    private List<Cell> hoveredCells;
    private List<Cell> selectedCells;


    public void OnTurnStart(int turnNumber, int turnDuration, int playerId) {
        turnCounter.text = "Turn " + turnNumber;
        turnPlayer.text = "Player " + playerId;
        StartTurnTimer(turnDuration);
    }

    public void OnTurnEnd() {
        StopTurnTimer();
    }


    public void StartTurnTimer(int turnDuration) {
        if (turnTimerCoroutine != null)
            StopCoroutine(turnTimerCoroutine);
        turnTimerCoroutine = StartCoroutine(DoTurnTimer(turnDuration));
    }

    private Coroutine turnTimerCoroutine = null;

    private IEnumerator DoTurnTimer(int turnDuration) {
        while (turnDuration >= 0) {
            turnTimer.text = "" + turnDuration;
            turnDuration--;
            yield return new WaitForSeconds(1);
        }
        turnTimer.text = "...";
        turnTimerCoroutine = null;
    }

    public void StopTurnTimer() {
        if (turnTimerCoroutine != null)
            StopCoroutine(turnTimerCoroutine);
        turnTimerCoroutine = null;
    }
}

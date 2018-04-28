using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchController : MonoBehaviour {

    public static MatchController instance;

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
        if (match.phase == Match.TurnPhase.Choice) {
            match.EndTurn();
        }
    }

    public Match match;
    public Map map;

    private List<Cell> showRangeCells;
    private List<Cell> showAreaCells;
    private List<Cell> hoveredCells;
    private List<Cell> selectedCells;
    private List<Cell> highlightedCells = new List<Cell>();
    private Cell hoveredCell;
    private SpellBase selectedSpell;


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
        turnTimer.text = "...";
    }

    public void OnMouseDownCell(Cell cell)
    {
        if (match.phase == Match.TurnPhase.Choice)
        {
            if (selectedSpell == null)
            {
                //Do the move
                AddMovementToPlayer(cell);
                //currentPlayer.AddMovementToCell(cell);
            }
            else
            {
                //Target the spell
            }
        }
    }

    private void AddMovementToPlayer(Cell destinationCell)
    {
        Player currentPlayer = match.players[match.playerTurn];
        //Check if the distance is not too big (doesn't check with obstacles)
        if (Map.Distance(currentPlayer.currentCell, destinationCell) > currentPlayer.mpCurrent) return;
        //Breadth first search
        List<Cell> path = map.ShortestPath(currentPlayer.currentCell, destinationCell, currentPlayer.mpCurrent);
        if (path != null)
        {
            currentPlayer.AddMoveToTurnAction(path);
            HighlightMoveChosen(path);
        }
    }

    private void HighlightMoveChosen(List<Cell> path)
    {
        foreach (Cell cell in path)
        {
            cell.PutChosenPathSkin();
        }
    }

    public void OnMouseEnterNewCell(Cell cell) {
        if (match.phase == Match.TurnPhase.Choice)
        {
            ClearHighlightedCells();
            hoveredCell = cell;
            if (hoveredCell.currentUnit != null)
            {
                Player hoveredPlayer = hoveredCell.currentUnit.GetComponent<Player>();
                if (hoveredPlayer != null)
                {
                    DisplayMPRange(hoveredCell, hoveredPlayer.mpCurrent);
                }
            }
        }
    }

    private void DisplayMPRange(Cell startingCell, int maxDistance) {
        Queue<Cell> cells = new Queue<Cell>();
        cells.Enqueue(startingCell);
        while (cells.Count > 0) {
            Cell cellToProcess = cells.Dequeue();
            if (Map.Distance(startingCell, cellToProcess) < maxDistance) {
                AddCellToHightlight(map.RightCell(cellToProcess), cells);
                AddCellToHightlight(map.TopCell(cellToProcess), cells);
                AddCellToHightlight(map.LeftCell(cellToProcess), cells);
                AddCellToHightlight(map.BotCell(cellToProcess), cells);                
            }
        }

        foreach (Cell cell in highlightedCells) {
            cell.marked = false;
            cell.PutDisplayMPSkin();
        }
    }

    private void AddCellToHightlight(Cell cellToAdd, Queue<Cell> cells) {
        if (cellToAdd != null 
            && !cellToAdd.marked 
            && cellToAdd.type == Cell.CellType.NORMAL) {
            cells.Enqueue(cellToAdd);
            cellToAdd.marked = true;
            highlightedCells.Add(cellToAdd);
        }
    }

    private void ClearHighlightedCells() {
        foreach (Cell cell in highlightedCells) {
            cell.PutDefaultSkin();
        }
        highlightedCells.Clear();
    }
}

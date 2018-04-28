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

    public RectTransform playerPanelsTeamA;
    public RectTransform playerPanelsTeamB;

    public List<PlayerPanel> playerPanels = new List<PlayerPanel>();

    #endregion // GUI_ELEMENTS

    public void InitializeInterface() {
        foreach (Player p in match.players) {
            PlayerPanel pp = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerPanel"), p.team == Team.TeamA ? playerPanelsTeamA : playerPanelsTeamB).GetComponent<PlayerPanel>();
            pp.SetPlayer(p);
            pp.SetMatchController(this);
            pp.UpdateInterface();
            pp.SetPanelInteractable(false);
            playerPanels.Add(pp);
        }
    }

    private ControllerState state = ControllerState.Solving;

    public void OnClickCell(Cell c) {
    }
    public void OnHoverCell(Cell c) {
    }
    public void OnClickEndTurn() {
        if (match.phase == Match.TurnPhase.Choice) {
            match.EndTurn();
        }
    }

    public void OnHoverSpellStart(int spell) {
        if (state > ControllerState.Solving) { // ignore hovering during solving phase
            AreaProfile range;
            if (match.currentTurn % 2 == 0)
                range = match.player.spells[spell].spell.rangeHeavy;
            else
                range = match.player.spells[spell].spell.rangeLight;
            switch (range.type) {
                case AreaType.Circle:
                    spellRangeCells.AddRange(map.GetCellsCircle(match.player.currentCell, range.max, range.min)); break;
                case AreaType.Cross:
                    spellRangeCells.AddRange(map.GetCellsCross(match.player.currentCell, range.max, range.min)); break;
                default: break;
            }
            //do coloring
        }
    }
    private List<Cell> spellRangeCells = new List<Cell>();
    public void OnHoverSpellEnd() {
        if (state > ControllerState.Solving) { // ignore hovering during solving phase
            //1.undo color
            spellRangeCells.Clear();
        }
    }

    public void OnClickSpell(int spell) {
        if (state > ControllerState.Solving) { // ignore click during solving phase
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
        foreach(PlayerPanel pp in playerPanels) {
            pp.UpdateInterface();
            pp.SetPanelInteractable(false); // dissable all panels
        }
        playerPanels[playerId].SetPanelInteractable(true); // enable current active player panel
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
        turnTimerCoroutine = null;
        OnClickEndTurn();
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

public enum ControllerState {
    Solving, // waiting for the match to solve the previous turn
    Moving, // accepting cell selection for player movement (show movement range)
    HoveringSpell, // show spell range
    Aiming, // accepting cell selection for player spell target (show spell range)
    HoverigRadius // show spell effect radius
}

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
    public Toggle toggleAI;

    public RectTransform playerPanelsTeamA;

    public RectTransform playerPanelsTeamB;

    public List<PlayerPanel> playerPanels = new List<PlayerPanel>();

    public LineRenderer pathTracer;

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

    #region SPELLS
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
    #endregion // SPELLS

    public Match match;
    public Map map;

    private List<Cell> showRangeCells;
    private List<Cell> showAreaCells;
    private List<Cell> hoveredCells;
    private List<Cell> selectedCells;
    private List<Cell> highlightedCells = new List<Cell>();
    private List<Cell> chosenPathCells = new List<Cell>();
    private List<Cell> selectedPathCells = new List<Cell>();
    private Cell hoveredCell;
    private SpellBase selectedSpell;

    private bool selectingPath = false;

    #region TURN
    public void OnTurnStart(int turnNumber, int turnDuration, int playerId) {
        foreach (PlayerPanel pp in playerPanels) {
            pp.UpdateInterface();
            pp.SetPanelInteractable(false); // dissable all panels
        }
        playerPanels[playerId].SetPanelInteractable(true); // enable current active player panel
        turnCounter.text = "Turn " + turnNumber;
        turnPlayer.text = "Player " + playerId;
        StartTurnTimer(turnDuration);
        endTurn.interactable = true;
    }
    public void OnTurnEnd() {
        foreach (PlayerPanel pp in playerPanels) {
            pp.SetPanelInteractable(false); // dissable all panels
        }
        ClearAllCells();
        ClearSelectedPathArrow();
        StopTurnTimer();
        endTurn.interactable = false;
    }
    #endregion // TURN

    #region TIMER
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
    #endregion // TIMER

    public void OnMouseDownCell(Cell cell) {
        if (!IsCellEventValid(cell)) { return; }
        ClearSelectedPathCells();
        selectingPath = true;
        selectedPathCells.Add(cell);
        ClearHighlightedCells();
        if (Map.IsAdjacent(match.CurrentPlayer().currentCell, cell)) {
            DrawArrowForSelectedPath(selectedPathCells);
        }
        //if (selectedSpell == null) {
        //    //Do the move
        //    ClearChosenPathCells();
        //    AddMovementToPlayer(cell);
        //    //currentPlayer.AddMovementToCell(cell);
        //}
        //else {
        //    //Target the spell
        //}

    }

    public void OnMouseUpCell(Cell cell) {
        if (!IsCellEventValid(cell)) { return; }
        if (!selectingPath) { throw new Exception("SelectingPath not set to true"); }
        selectingPath = false;
        match.CurrentPlayer().ClearMovementAction();
        ClearSelectedPathArrow();
        if (selectedPathCells.Count == 1) {
            ClearSelectedPathCells();
            AddMovementToPlayer(cell);
        }
        else {
            AddMovementToPlayer(selectedPathCells);
        }

    }

    public void OnMouseEnterNewCell(Cell cell) {
        if (!IsCellEventValid(cell)) { return; }
        if (!selectingPath) {
            ClearHighlightedCells();
            hoveredCell = cell;
            if (hoveredCell.currentUnit != null) {
                Player hoveredPlayer = hoveredCell.currentUnit.GetComponent<Player>();
                if (hoveredPlayer != null) {
                    DisplayMPRange(hoveredCell, hoveredPlayer.mpCurrent);
                }
            }
            else {
                DisplayMovePrediction(hoveredCell);
            }
        }
        else {
            if (selectedPathCells.Count + 1 <= match.CurrentPlayer().mpCurrent
                && Map.IsAdjacent(selectedPathCells[0], match.CurrentPlayer().currentCell)) {
                selectedPathCells.Add(cell);
                DrawArrowForSelectedPath(selectedPathCells);
            }
        }

    }

    private void AddMovementToPlayer(Cell destinationCell) {
        Player p = match.CurrentPlayer();
        //Breadth first search
        List<Cell> path = map.ShortestPath(p.currentCell, destinationCell, p.mpCurrent);
        if (path != null) {
            p.AddMoveToTurnAction(path);
            selectedPathCells = path;
            DrawArrowForSelectedPath(path);
        }
    }

    private void AddMovementToPlayer(List<Cell> path) {
        Player p = match.CurrentPlayer();
        if (!IsMovementValid(path, p)) {
            ClearSelectedPathCells();
        }
        else {
            p.AddMoveToTurnAction(path);
            DrawArrowForSelectedPath(path);
        }
    }

    private bool IsMovementValid(List<Cell> path, Player p) {
        if (!Map.IsAdjacent(path[0], p.currentCell) || path.Count > p.mpCurrent) {
            return false;
        }
        return true;
    }

    private bool IsCellEventValid(Cell c) {
        return match.phase == Match.TurnPhase.Choice && c.type == Cell.CellType.NORMAL;
    }

    private void DrawArrowForSelectedPath(List<Cell> path) {
        pathTracer.positionCount = path.Count + 1;
        pathTracer.SetPosition(0, map.CellPosition(match.CurrentPlayer().currentCell));
        for (int i = 0; i < path.Count; ++i) {
            Vector3 pos = map.CellPosition(path[i]);
            pathTracer.SetPosition(i + 1, pos);
        }
    }

    private void DisplayMPRange(Cell startingCell, int maxDistance) {
        highlightedCells = map.BreadthFirstSearch(startingCell, maxDistance);
        foreach (Cell c in highlightedCells) {
            c.PutMoveSkin();
        }
    }

    private void DisplayMovePrediction(Cell destinationCell) {
        Player currentPlayer = match.players[match.playerTurn];
        List<Cell> path = map.ShortestPath(currentPlayer.currentCell, destinationCell, currentPlayer.mpCurrent);
        if (path != null) {
            highlightedCells = path;
            foreach (Cell c in highlightedCells) {
                c.PutMoveSkin();
            }
        }
    }

    private void ClearAllCells() {
        foreach (Cell cell in map.cells) {
            cell.PutDefaultSkin();
        }
        highlightedCells.Clear();
        selectedPathCells.Clear();
    }

    private void ClearHighlightedCells() {
        foreach (Cell cell in highlightedCells) {
            cell.PutDefaultSkin();
        }
        highlightedCells.Clear();
    }

    private void ClearSelectedPathCells() {
        foreach (Cell cell in selectedPathCells) {
            if (!highlightedCells.Contains(cell)) { cell.PutDefaultSkin(); }
        }
        selectedPathCells.Clear();
    }

    private void ClearSelectedPathArrow() {
        pathTracer.positionCount = 0;
    }
}

public enum ControllerState {
    Solving, // waiting for the match to solve the previous turn
    Moving, // accepting cell selection for player movement (show movement range)
    HoveringSpell, // show spell range
    Aiming, // accepting cell selection for player spell target (show spell range)
    HoverigRadius // show spell effect radius
}

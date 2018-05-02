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
            currentSpell = spell;
            AreaProfile range;
            if (match.currentTurn % 2 == 0)
                range = match.player.spells[spell].spell.rangeHeavy;
            else
                range = match.player.spells[spell].spell.rangeLight;
            spellRangeCells.AddRange(map.GetCellsArea(match.player.nextCell, range));
            //do coloring
            foreach (Cell c in spellRangeCells) {
                c.PutSpellRangeSkin();
            }
        }
    }
    private int currentSpell = -1;
    private List<Cell> spellRangeCells = new List<Cell>();
    private List<Cell> spellAreaCells = new List<Cell>();
    public void OnHoverSpellEnd() {
        currentSpell = -1;
        if (state > ControllerState.Solving) { // ignore hovering during solving phase
                                               //1.undo color
            foreach (Cell c in spellRangeCells) {
                c.PutDefaultSkin();
            }
            spellRangeCells.Clear();
        }
    }
    public void OnClickSpell(int spell) {
        if (state > ControllerState.Solving) { // ignore click during solving phase
            if (state == ControllerState.Spelling) { // was spelling
                ClearRangeColor(); // clear currently highlighted cells
                if (currentSpell == spell) { // if same cell, cancel and revert back to movement
                    currentSpell = -1;
                    state = ControllerState.Moving;
                    return;
                }
            }
            else if (state == ControllerState.Spelled) {
                state = ControllerState.Spelling;
                ClearAreaColor();
                match.player.currentAction.spell.spell = null;
                match.player.currentAction.spell.target = null;
            }
            else {
                state = ControllerState.Spelling;
            }
            currentSpell = spell;
            AreaProfile range;
            bool los;
            if (match.currentTurn % 2 == 0) {
                range = match.player.spells[spell].spell.rangeHeavy;
                los = match.player.spells[spell].spell.lineOfSightHeavy;
            }
            else {
                range = match.player.spells[spell].spell.rangeLight;
                los = match.player.spells[spell].spell.lineOfSightLight;
            }

            spellRangeCells.AddRange(map.GetCellsArea(match.player.nextCell, range, los));

            //do coloring
            foreach (Cell c in spellRangeCells) {
                if (c.type == Cell.CellType.NORMAL)
                    c.PutSpellRangeSkin();
            }
        }
    }

    public void OnClickCellSpell(Cell cell) {
        if (state > ControllerState.Moving) { // ignore click during solving and moving phase
            if (state == ControllerState.Spelled && match.player.currentAction.spell.target == cell) { // spelled, canceling
                ClearAreaColor();
                match.player.currentAction.spell.spell = null;
                match.player.currentAction.spell.target = null;
                state = ControllerState.Moving; // back to movement
            }
            else if (currentSpell != -1 && cell.type == Cell.CellType.NORMAL && spellRangeCells.Contains(cell)) { // spelling, targetable, in range
                match.player.currentAction.spell.spell = match.player.spells[currentSpell];
                match.player.currentAction.spell.target = cell;
                ClearRangeColor();
                if (match.currentTurn % 2 == 0)
                    spellAreaCells.AddRange(match.player.spells[currentSpell].spell.GetEffectAreaPreviewHeavy(match.player, cell, map));
                else
                    spellAreaCells.AddRange(match.player.spells[currentSpell].spell.GetEffectAreaPreviewLight(match.player, cell, map));
                foreach (Cell c in spellAreaCells)
                    c.PutSpellAreaSkin();
                currentSpell = -1;
                state = ControllerState.Spelled; // ready to end turn, respell, or cancel
            }
        }
    }

    private void ClearRangeColor() {
        foreach (Cell c in spellRangeCells) {
            c.PutDefaultSkin();
        }
        spellRangeCells.Clear();
    }

    private void ClearAreaColor() {
        foreach (Cell c in spellAreaCells) {
            c.PutDefaultSkin();
        }
        spellAreaCells.Clear();
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
        state = ControllerState.Moving;
    }
    public void OnTurnEnd() {
        foreach (PlayerPanel pp in playerPanels) {
            pp.SetPanelInteractable(false); // dissable all panels
        }
        ClearAllCells();
        ClearSelectedPathArrow();
        ClearRangeColor();
        ClearAreaColor();
        currentSpell = -1;
        StopTurnTimer();
        endTurn.interactable = false;
        state = ControllerState.Solving;
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

    #region MOUSE_CELL_EVENTS
    public void OnMouseDownCell(Cell cell) {
        if (!IsChoicePhaseActive()) { return; }
        switch (state) {
            case ControllerState.Moving:
                OnMouseDownCellMoving(cell);
                break;
        }
    }

    public void OnMouseUpCell(Cell cell) {
        if (!IsChoicePhaseActive()) { return; }
        switch (state) {
            case ControllerState.Moving:
                EndDrawingMovePath(cell);
                break;
            case ControllerState.Spelled:
            case ControllerState.Spelling:
                OnClickCellSpell(cell);
                break;
        }
    }

    public void OnMouseEnterNewCell(Cell cell) {
        if (!IsChoicePhaseActive()) { return; }
        switch (state) {
            case ControllerState.Moving:
                OnMouseEnterCellMoving(cell);
                break;
        }
    }
    #endregion //MOUSE_CELL_EVENTS

    #region MOVING_EVENTS
    private void OnMouseDownCellMoving(Cell cell) {
        if (cell.type != Cell.CellType.NORMAL) { return; }
        StartDrawingMovePath(cell);
    }

    private void OnMouseEnterCellMoving(Cell cell) {
        if (cell.type != Cell.CellType.NORMAL) { return; }
        if (selectingPath) {
            ContinueDrawingMovePath(cell);
        }
        else {
            ShowCellMoveInformation(cell);
        }
    }

    private void OnMouseUpCellMoving(Cell cell) {
        if (cell.type != Cell.CellType.NORMAL) { return; }
        if (selectingPath) {
            EndDrawingMovePath(cell);
        }
    }
    #endregion //MOVING_EVENTS

    #region MOVEMENT
    private void StartDrawingMovePath(Cell cell) {
        ClearSelectedPathCells();
        selectedPathCells.Add(cell);
        ClearHighlightedCells();
        if (Map.IsAdjacent(match.CurrentPlayer().currentCell, cell)) {
            selectingPath = true;
            DrawArrowForSelectedPath(selectedPathCells);
        }
    }

    private void ContinueDrawingMovePath(Cell cell) {
        if (selectedPathCells.Count == 0) return; //sometimes count == 0 which raise an exception, TODO : check why count can be 0
        Cell last = selectedPathCells[selectedPathCells.Count - 1];
        if (Map.IsAdjacent(cell, last)
            && selectedPathCells.Count + 1 <= match.CurrentPlayer().mpCurrent
            && Map.IsAdjacent(selectedPathCells[0], match.CurrentPlayer().currentCell)) {
            selectedPathCells.Add(cell);
            DrawArrowForSelectedPath(selectedPathCells);
        }
    }

    private void EndDrawingMovePath(Cell cell) {
        selectingPath = false;
        match.CurrentPlayer().ClearMovementAction();
        ClearSelectedPathArrow();
        if (selectedPathCells.Count <= 1) {
            ClearSelectedPathCells();
            AddMovementToPlayer(cell);
        }
        else {
            AddMovementToPlayer(selectedPathCells);
        }
    }

    private void ShowCellMoveInformation(Cell cell) {
        ClearHighlightedCells();
        hoveredCell = cell;
        if (hoveredCell.currentUnit != null) { //Player mp range information
            Player hoveredPlayer = hoveredCell.currentUnit.GetComponent<Player>();
            if (hoveredPlayer != null) {
                DisplayMPRange(hoveredCell, hoveredPlayer.mpCurrent);
            }
        }
        else {
            DisplayMovePrediction(hoveredCell); //Move prediction information
        }
    }

    private void DrawArrowForSelectedPath(List<Cell> path) {
        pathTracer.positionCount = path.Count + 1;
        pathTracer.SetPosition(0, map.CellPosition(match.CurrentPlayer().currentCell));
        for (int i = 0; i < path.Count; ++i) {
            Vector3 pos = map.CellPosition(path[i]);
            pathTracer.SetPosition(i + 1, pos);
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

    private bool IsMovementValid(List<Cell> path, Player p) {
        if (!Map.IsAdjacent(path[0], p.currentCell) || path.Count > p.mpCurrent) {
            return false;
        }
        return true;
    }
    #endregion //MOVEMENT

    #region CLEARING
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
    #endregion //CLEARING

    private bool IsChoicePhaseActive() {
        return match.phase == Match.TurnPhase.Choice;
    }
}

public enum ControllerState {
    Solving, // waiting for the match to solve the previous turn
    Moving, // accepting cell selection for player movement (show movement range)
    Spelling, // accepting cell selection for player spell target (show spell range)
    Spelled // show spell effect radius, can click another spell or current target spell to cancel
}

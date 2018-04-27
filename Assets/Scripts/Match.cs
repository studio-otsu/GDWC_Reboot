using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour {

    public int currentTurn;
    public enum TurnPhase { Choice, Solve }
    public TurnPhase phase;
    public int playerTurn;

    public InputController controller;
    public MatchController matchController;
    public Map map;
    public TurnSolver solver;

    public Player player {
        get { return players[playerTurn]; }
    }

    public List<Player> players = new List<Player>();
    public List<Unit> teamA = new List<Unit>();
    public List<Unit> teamB = new List<Unit>();
    public List<Unit> neutrals = new List<Unit>();

    public static Color colorTeamA = new Color(0, 0.8f, 0.2f);
    public static Color colorTeamB = new Color(0.6f, 0, 0.6f);

    public static Match PrepareMatch(string mapPath) {
        Match output = new GameObject("Match").AddComponent<Match>();
        output.map = new GameObject("Map").AddComponent<Map>();
        output.map.transform.SetParent(output.transform);
        // build map
        MapBuilder builder = new MapBuilder();
        builder.map = output.map;
        builder.cellPrefab = Resources.Load<GameObject>("Prefabs/Cell");
        builder.playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        builder.GenerateMap(mapPath); // todo : fill in cell prefab
        // build players
        Player p1 = Instantiate(builder.playerPrefab, output.transform).GetComponent<Player>();
        p1.name = "Player 1";
        p1.team = Team.TeamA;
        p1.teamColor = colorTeamA;
        output.teamA.Add(p1);
        output.players.Add(p1);
        Player p2 = Instantiate(builder.playerPrefab, output.transform).GetComponent<Player>();
        p2.name = "Player 2";
        p2.team = Team.TeamB;
        p2.teamColor = colorTeamB;
        output.teamB.Add(p2);
        output.players.Add(p2);
        // put players on map
        Cell cell1 = output.map.startingACells[0]; // player 1 start pos
        Map.AddUnitToCell(p1, cell1);
        p1.transform.position = cell1.transform.position;
        Cell cell2 = output.map.startingBCells[0]; // player 2 start pos
        Map.AddUnitToCell(p2, cell2);
        p2.transform.position = cell2.transform.position;
        // init input controller
        //output.controller = new InputController(output);
        //output.StartMatch();
        // other stuff
        output.solver = new TurnSolver();
        output.solver.match = output;
        output.solver.map = output.map;
        return output;
    }

    public void StartMatch() {
        //Init match turn
        currentTurn = 0;
        phase = TurnPhase.Choice;
        playerTurn = players.Count-1;
        StartNewTurn();
    }

    private void StartNewTurn() {
        playerTurn = (playerTurn + 1) % players.Count;
        currentTurn += playerTurn == 0 ? 1 : 0;
        matchController.OnTurnStart(currentTurn, 5, playerTurn);
    }

    public void EndTurn() {
        EndCurrentPlayerTurn();
    }

    public void EndCurrentPlayerTurn() {
        matchController.OnTurnEnd();
        Player currentPlayer = players[playerTurn];
        /*
            TODO : retrieve player action and put them in a set to process the actions in the Solve phase
         */
        /*if (playerTurn < (players.Count - 1)) {
            ++playerTurn;
        }
        else {
            StartSolvePhase();
        }*/
        if (playerTurn < (players.Count-1)) {
            StartNewTurn();
        } else {
            StartSolvePhase();
        }
    }

    private void StartSolvePhase()
    {
        StartCoroutine(DoSolvePhase());
    }

    private IEnumerator DoSolvePhase() {
        phase = TurnPhase.Solve;
        StartCoroutine(solver.DoSolveMovements());
        yield return new WaitWhile(delegate { return solver.isSolvingMovements; });
        StartCoroutine(solver.DoSolveSpells());
        yield return new WaitWhile(delegate { return solver.isSolvingSpells; });
        EndSolvePhase();
    }

    private void EndSolvePhase() {
        StartNewTurn();
    }

    //For debugging
    void OnGUI() {
        //GUI.Label(new Rect(10,10,200,20), "Turn: " + currentTurn.ToString());
        //GUI.Label(new Rect(10,30,200,20), "PlayerTurn: " + playerTurn.ToString());
    }
}

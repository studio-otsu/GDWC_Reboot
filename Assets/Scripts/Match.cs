using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour {

    public int currentTurn;

    public Map map;

    public List<Player> players = new List<Player>();
    public List<Unit> teamA = new List<Unit>();
    public List<Unit> teamB = new List<Unit>();
    public List<Unit> neutrals = new List<Unit>();

    public static Color colorTeamA = new Color(0, 0.8f, 0.2f);
    public static Color colorTeamB = new Color(0.6f, 0, 0.6f);

    public static Match StartMatch(string mapPath) {
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
        p1.currentCell = cell1;
        p1.transform.position = cell1.transform.position;
        cell1.currentUnit = p1;
        Cell cell2 = output.map.startingBCells[0]; // player 2 start pos
        p2.currentCell = cell2;
        p2.transform.position = cell2.transform.position;
        cell2.currentUnit = p2;
        return output;
    }

}

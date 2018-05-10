using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapBuilder {

    public Map map;
    public GameObject cellPrefab;
    public GameObject playerPrefab; //to be used later

    //private string path = "Assets/Data/Maps/map0";

    public void GenerateMap(string filePath) {
        CalculateWidthHeight(filePath);


        TextAsset mapAsset = Resources.Load(filePath) as TextAsset;
        string[] mapSplitted = mapAsset.text.Split('\n');

        //Read the map file and create cell for each number
        for (int y = 0; y < mapSplitted.Length; ++y) {
            string[] lineSplitted = mapSplitted[y].Split(';');
            for (int x = 0; x < lineSplitted.Length; ++x) {
                CreateCell(x, y, lineSplitted[x]);
            }
        }
    }

    private void CalculateWidthHeight(string filePath) {
        TextAsset mapAsset = Resources.Load(filePath) as TextAsset;
        string[] mapSplitted = mapAsset.text.Split('\n');
        int width = mapSplitted[0].Split(';').Length;        
        int height = mapSplitted.Length;
        map.SetDimensions(width, height);
    }

    private void CreateCell(int x, int y, string type) {
        //Create gameobject
        GameObject createdCellObj = GameObject.Instantiate(cellPrefab, map.CellPosition(x, y), Quaternion.Euler(90,0,0), map.transform);
        //Set cell attributes
        Cell createdCell = createdCellObj.GetComponent<Cell>();
        createdCell.name = "Cell(" + x + "," + y + ")";
        createdCell.SetPosition(x, y);
        if (type == "A") { // team A starting point
            createdCell.SetTypeFromString("0");
            map.startingACells.Add(createdCell);
        } else if (type == "B") { // team B starting point
            createdCell.SetTypeFromString("0");
            map.startingBCells.Add(createdCell);
        } else createdCell.SetTypeFromString(type);
        //Add Cell to Map
        map.cells[map.CellIndex(x, y)] = createdCell;
    }


}

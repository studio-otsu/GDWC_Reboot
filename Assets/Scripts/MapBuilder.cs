using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{

    public Map map;
    public GameObject cellPrefab;
    public GameObject playerPrefab; //to be used later

    private string path = "Assets/Data/Maps/map0";

    void Start()
    {
        GenerateMap(path, cellPrefab);
    }

    public void GenerateMap(string filePath, GameObject cellPrefab)
    {
        CalculateWidthHeight(filePath);

        //Read the map file and create cell for each number
        StreamReader reader = new StreamReader(filePath);
        string line = reader.ReadLine();
        for (int y = 0; line != null; ++y, line = reader.ReadLine())
        {
            string[] lineSplitted = line.Split(';');
            for (int x = 0; x < lineSplitted.Length; ++x)
            {
                CreateCell(x, y, lineSplitted[x]);
            }
        }

        reader.Close();
    }

    private void CalculateWidthHeight(string filePath)
    {
        int width, height;
        using (StreamReader r = new StreamReader(filePath))
        {
            //Determine width
            string line = r.ReadLine();
            width = line.Split(';').Length;

            //Determine height
            int h = 1;
            while (r.ReadLine() != null)
            {
                h++;
            }
            height = h;
        }
        //Send width and height to Map
        map.SetDimensions(width, height);
    }

    private void CreateCell(int x, int y, string type)
    {
        //Create gameobject
        GameObject createdCellObj = Instantiate(cellPrefab, map.CellPosition(x, y), Quaternion.identity, transform);
        //Set cell attributes
        Cell createdCell = createdCellObj.GetComponent<Cell>();
        createdCell.SetPosition(x, y);
        createdCell.SetTypeFromString(type);
        //Add Cell to Map
        map.cells[map.CellIndex(x, y)] = createdCell;
    }


}

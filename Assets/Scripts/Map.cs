using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour {
    public Cell[] cells;
    int width = 0;
    int height = 0;

    public List<Cell> startingACells = new List<Cell>();
    public List<Cell> startingBCells = new List<Cell>();

    public void SetDimensions(int widthParam, int heightParam) {
        width = widthParam;
        height = heightParam;
        cells = new Cell[width * height];
    }

    public int CellIndex(int x, int y) {
        return x + y * height;
    }

    public Vector3 CellPosition(int x, int y) {
        float xPos = -((float)(width / 2)) + x;
        float yPos = -((float)(height / 2)) - y + height;
        return new Vector3(xPos, yPos);
    }

    public Cell GetCell(int x, int y) {
        return cells[CellIndex(x, y)];
    }

    public Cell GetCell(int index) {
        return cells[index];
    }

    public Cell TopCell(Cell cell)
    {
        if (cell.y > 0)
        {
            return GetCell(cell.x, cell.y - 1);
        }
        return null;
    }

    public Cell BotCell(Cell cell)
    {
        if (cell.y < height - 1)
        {
            return GetCell(cell.x, cell.y + 1);
        }
        return null;
    }

    public Cell LeftCell(Cell cell)
    {
        if (cell.x > 0)
        {
            return GetCell(cell.x - 1, cell.y);
        }
        return null;
    }

    public Cell RightCell(Cell cell)
    {
        if (cell.x < width - 1)
        {
            return GetCell(cell.x + 1, cell.y);
        }
        return null;
    }

    /*
     *  Static functions
     */

    public static void MovePlayerToAdjacentCell(Player player, Cell cell) {
        if (!IsAdjacent(player.currentCell, cell)) throw new System.Exception("MovePlayerToCell : not adjacent");
        if (player.currentCell.currentUnit == player) { // in case someone already moved in...
            player.currentCell.currentUnit = null; // don't kick them
        }
        player.currentCell = cell;
        cell.currentUnit = player;
        player.UseMP(1);
        player.transform.position = player.currentCell.transform.position;
    }

    public static void AddUnitToCell(Unit unit, Cell cell) {
        unit.currentCell = cell;
        cell.currentUnit = unit;
    }

    public static int Distance(Cell cell1, Cell cell2) {
        return Mathf.Abs(cell1.x - cell2.x) + Mathf.Abs(cell1.y - cell2.y);
    }

    public static bool SameCell(Cell cell1, Cell cell2) {
        return cell1 == cell2 || cell1.x == cell2.x && cell1.y == cell2.y;
    }

    public static bool IsAdjacent(Cell cell1, Cell cell2)
    {
        if (SameCell(cell1, cell2)) throw new System.Exception("IsAdjacent : same position");
        int distance = Distance(cell1, cell2);
        return (distance == 1);
    }

    public List<Cell> ShortestPath(Cell start, Cell destination, int maxDistance = 30)
    {
        //BreadthFirstSearch
        Queue<Cell> cells = new Queue<Cell>();
        Dictionary<Cell, Cell> previousCell = new Dictionary<Cell, Cell>();
        List<Cell> markedCells = new List<Cell>();
        cells.Enqueue(start);
        start.marked = true;
        markedCells.Add(start);
        while (cells.Count > 0)
        {
            Cell current = cells.Dequeue();
            if (Map.Distance(start, current) <= maxDistance)
            {              
                AddNeighbor(current, RightCell(current), cells, markedCells, previousCell);
                AddNeighbor(current, TopCell(current), cells, markedCells, previousCell);
                AddNeighbor(current, LeftCell(current), cells, markedCells, previousCell);
                AddNeighbor(current, BotCell(current), cells, markedCells, previousCell);
            }
        }

        //Retrieve path from previousCells
        List<Cell> path = null;
        if (previousCell.ContainsKey(destination))
        {
            path = new List<Cell>();
            Cell current = destination;            
            while (current != start)
            {
                path.Add(current);
                current = previousCell[current];
            }
            path.Reverse();            
        }

        //Clear
        foreach(Cell cell in markedCells)
        {
            cell.marked = false;
        }

        return path;
    }

    private void AddNeighbor(Cell current, Cell neighbor,
        Queue<Cell> cells, List<Cell> markedCells, Dictionary<Cell, Cell> previousCells)
    {
        if (neighbor != null
            && !neighbor.marked
            && neighbor.type == Cell.CellType.NORMAL)
        {
            cells.Enqueue(neighbor);
            neighbor.marked = true;
            markedCells.Add(neighbor);
            previousCells.Add(neighbor, current);
        }

    }
    
    public List<Cell> GetCellsCircle(Cell center, int max, int min = 0) {
        List<Cell> output = new List<Cell>();
        for (int y = Mathf.Max(0,center.y-max); y < Mathf.Min(height, center.y + max); y++) {
            for (int x = Mathf.Max(0, center.x - max); x < Mathf.Min(width, center.x + max); x++) {
                Cell c = GetCell(x, y);
                if(c != null) {
                    int distance = Distance(center, c);
                    if (distance >= min && distance <= max) {
                        output.Add(c);
                    }
                }
            }
        }
        return output;
    }

    public List<Cell> GetCellsCross(Cell center, int max, int min = 0) {
        return new List<Cell>(GetCellsVerticalLine(center, max, min).Union(GetCellsHorizontalLine(center, max, min)));
    }

    public List<Cell> GetCellsVerticalLine(Cell center, int max, int min = 0) {
        return new List<Cell>(GetCellsTopLine(center, max, min).Union(GetCellsBottomLine(center, max, min)));
    }
    public List<Cell> GetCellsHorizontalLine(Cell center, int max, int min = 0) {
        return new List<Cell>(GetCellsLeftLine(center, max, min).Union(GetCellsRightLine(center, max, min)));
    }

    public List<Cell> GetCellsTopLine(Cell center, int max, int min = 0) {
        List<Cell> output = new List<Cell>();
        for (int i = min; i < max; ++i) {
            if (center.y - i >= 0)
                output.Add(GetCell(center.x, center.y - i));
        }
        return output;
    }
    public List<Cell> GetCellsBottomLine(Cell center, int max, int min = 0) {
        List<Cell> output = new List<Cell>();
        for (int i = min; i < max; ++i) {
            if (center.y + i < height)
                output.Add(GetCell(center.x, center.y + i));
        }
        return output;
    }
    public List<Cell> GetCellsLeftLine(Cell center, int max, int min = 0) {
        List<Cell> output = new List<Cell>();
        for (int i = min; i < max; ++i) {
            if (center.x - i >= 0)
                output.Add(GetCell(center.x - 1, center.y));
        }
        return output;
    }
    public List<Cell> GetCellsRightLine(Cell center, int max, int min = 0) {
        List<Cell> output = new List<Cell>();
        for (int i = min; i < max; ++i) {
            if (center.x + i < width)
                output.Add(GetCell(center.x + 1, center.y));
        }
        return output;
    }
}

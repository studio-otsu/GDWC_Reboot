using System;
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

    public Vector3 CellPosition(Cell c) {
        return CellPosition(c.x, c.y);
    }

    public Cell GetCell(int x, int y) {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;
        return cells[CellIndex(x, y)];
    }

    public Cell GetCell(int index) {
        return cells[index];
    }

    public Cell TopCell(Cell cell) {
        if (cell.y > 0) {
            return GetCell(cell.x, cell.y - 1);
        }
        return null;
    }

    public Cell BotCell(Cell cell) {
        if (cell.y < height - 1) {
            return GetCell(cell.x, cell.y + 1);
        }
        return null;
    }

    public Cell LeftCell(Cell cell) {
        if (cell.x > 0) {
            return GetCell(cell.x - 1, cell.y);
        }
        return null;
    }

    public Cell RightCell(Cell cell) {
        if (cell.x < width - 1) {
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

    public static bool IsAdjacent(Cell cell1, Cell cell2) {
        int distance = Distance(cell1, cell2);
        return (distance == 1);
    }

    public List<Cell> ShortestPath(Cell start, Cell destination, int maxDistance = 30) {
        //BreadthFirstSearch
        Dictionary<Cell, Cell> previousCell = BreadthFirstSearchPrevious(start, maxDistance);

        //Retrieve path from previousCells
        List<Cell> path = null;
        if (previousCell.ContainsKey(destination)) {
            path = new List<Cell>();
            Cell current = destination;
            while (current != start) {
                path.Add(current);
                current = previousCell[current];
            }
            path.Reverse();
        }
        return path;
    }

    public Dictionary<Cell, Cell> BreadthFirstSearchPrevious(Cell start, int maxDistance = 30)
    {
        Queue<Cell> cells = new Queue<Cell>();
        Dictionary<Cell, Cell> previousCells = new Dictionary<Cell, Cell>();
        List<Cell> markedCells = new List<Cell>();
        cells.Enqueue(start);
        start.marked = true;
        markedCells.Add(start);
        while (cells.Count > 0)
        {
            Cell current = cells.Dequeue();
            if (Map.Distance(start, current) < maxDistance)
            {
                AddNeighbor(current, RightCell(current), cells, markedCells, previousCells);
                AddNeighbor(current, TopCell(current), cells, markedCells, previousCells);
                AddNeighbor(current, LeftCell(current), cells, markedCells, previousCells);
                AddNeighbor(current, BotCell(current), cells, markedCells, previousCells);
            }
        }

        //Clear
        foreach (Cell cell in markedCells)
        {
            cell.marked = false;
        }

        return previousCells;
    }

    public List<Cell> BreadthFirstSearch(Cell start, int maxDistance = 30)
    {
        Dictionary<Cell, Cell> previousCells = BreadthFirstSearchPrevious(start, maxDistance);
        return previousCells.Keys.ToList<Cell>();
    }

    private void AddNeighbor(Cell current, Cell neighbor,
        Queue<Cell> cells, List<Cell> markedCells, Dictionary<Cell, Cell> previousCells) {
        if (neighbor != null
            && !neighbor.marked
            && neighbor.type == Cell.CellType.NORMAL) {
            cells.Enqueue(neighbor);
            neighbor.marked = true;
            markedCells.Add(neighbor);
            previousCells.Add(neighbor, current);
        }

    }

    public List<Cell> GetCellsArea(Cell center, AreaProfile area, bool checkLineOfSight = false) {
        switch (area.type) {
            case AreaType.Cross:
                return GetCellsCross(center, area.max, area.min, checkLineOfSight);
            default:
                return GetCellsCircle(center, area.max, area.min, checkLineOfSight);
        }
    }

    public List<Cell> GetCellsCircle(Cell center, int max, int min = 0, bool checkLineOfSight = false) {
        List<Cell> output = new List<Cell>();
        for (int y = Mathf.Max(0, center.y - Mathf.Max(min, max)); y <= Mathf.Min(height, center.y + Mathf.Max(min, max)); y++) {
            for (int x = Mathf.Max(0, center.x - Mathf.Max(min, max)); x <= Mathf.Min(width, center.x + Mathf.Max(min, max)); x++) {
                Cell c = GetCell(x, y);
                if (c != null) {
                    int distance = Distance(center, c);
                    if (distance >= Mathf.Min(min, max) && distance <= Mathf.Max(min, max) &&(!checkLineOfSight||inLineOfSight(center,c))) {
                        output.Add(c);
                    }
                }
            }
        }
        return output;
    }

    public List<Cell> GetCellsCross(Cell center, int max, int min = 0, bool checkLineOfSight = false) {
        return new List<Cell>(GetCellsVerticalLine(center, max, min, checkLineOfSight).Union(GetCellsHorizontalLine(center, max, min, checkLineOfSight)));
    }

    public List<Cell> GetCellsVerticalLine(Cell center, int max, int min = 0, bool checkLineOfSight = false) {
        return new List<Cell>(GetCellsTopLine(center, max, min, checkLineOfSight).Union(GetCellsBottomLine(center, max, min, checkLineOfSight)));
    }
    public List<Cell> GetCellsHorizontalLine(Cell center, int max, int min = 0, bool checkLineOfSight = false) {
        return new List<Cell>(GetCellsLeftLine(center, max, min, checkLineOfSight).Union(GetCellsRightLine(center, max, min, checkLineOfSight)));
    }

    public List<Cell> GetCellsTopLine(Cell center, int max, int min = 0, bool checkLineOfSight = false) {
        List<Cell> output = new List<Cell>();
        for (int i = Mathf.Min(min, max); i <= Mathf.Max(min, max); ++i) {
            Cell c = GetCell(center.x, center.y - i);
            if (c != null && (!checkLineOfSight || inLineOfSight(center, c)))
                output.Add(c);
        }
        return output;
    }
    public List<Cell> GetCellsBottomLine(Cell center, int max, int min = 0, bool checkLineOfSight = false) {
        List<Cell> output = new List<Cell>();
        for (int i = Mathf.Min(min, max); i <= Mathf.Max(min, max); ++i) {
            Cell c = GetCell(center.x, center.y + i);
            if (c != null && (!checkLineOfSight || inLineOfSight(center, c)))
                output.Add(c);
        }
        return output;
    }
    public List<Cell> GetCellsLeftLine(Cell center, int max, int min = 0, bool checkLineOfSight = false) {
        List<Cell> output = new List<Cell>();
        for (int i = Mathf.Min(min, max); i <= Mathf.Max(min, max); ++i) {
            Cell c = GetCell(center.x - i, center.y);
            if (c != null && (!checkLineOfSight || inLineOfSight(center, c)))
                output.Add(c);
        }
        return output;
    }
    public List<Cell> GetCellsRightLine(Cell center, int max, int min = 0, bool checkLineOfSight = false) {
        List<Cell> output = new List<Cell>();
        for (int i = Mathf.Min(min, max); i <= Mathf.Max(min, max); ++i) {
            Cell c = GetCell(center.x + i, center.y);
            if (c != null && (!checkLineOfSight || inLineOfSight(center,c)))
                output.Add(c);
        }
        return output;
    }

    public bool inLineOfSight(Cell c0, Cell c1) {
        int x0 = c0.x; int x1 = c1.x; int y0 = c0.y; int y1 = c1.y;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int x = x0;
        int y = y0;
        int n = -1 + dx + dy;
        int x_inc = (x1 > x0 ? 1 : -1);
        int y_inc = (y1 > y0 ? 1 : -1);
        int error = dx - dy;
        dx *= 2;
        dy *= 2;

        for (int i = 0; i < 1; i++) {
            if (error > 0) {
                x += x_inc;
                error -= dy;
            }
            else if (error < 0) {
                y += y_inc;
                error += dx;
            }
            else {
                x += x_inc;
                error -= dy;
                y += y_inc;
                error += dx;
                n--;
            }
        }

        while (n > 0) {

            if(GetCell(x,y).type == Cell.CellType.OBSTACLE) {
                return false;
            }
            else {
                if (error > 0) {
                    x += x_inc;
                    error -= dy;
                }
                else if (error < 0) {
                    y += y_inc;
                    error += dx;
                }
                else {
                    x += x_inc;
                    error -= dy;
                    y += y_inc;
                    error += dx;
                    n--;
                }

                n--;
            }
        }

        return true;
    }
}

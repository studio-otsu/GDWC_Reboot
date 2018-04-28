using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Cell[] cells;
    int width = 0;
    int height = 0;

    public List<Cell> startingACells = new List<Cell>();
    public List<Cell> startingBCells = new List<Cell>();

    public void SetDimensions(int widthParam, int heightParam)
    {
        width = widthParam;
        height = heightParam;
        cells = new Cell[width * height];
    }

    public int CellIndex(int x, int y)
    {
        return x + y * height;
    }

    public Vector3 CellPosition(int x, int y)
    {
        float xPos = -((float)(width / 2)) + x;
        float yPos = -((float)(height / 2)) - y + height;
        return new Vector3(xPos, yPos);
    }

    public Cell GetCell(int x, int y)
    {
        return cells[CellIndex(x, y)];
    }

    public Cell GetCell(int index)
    {
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

    public static void MovePlayerToCell(Player player, Cell cell)
    {

    }

    public static void AddUnitToCell(Unit unit, Cell cell)
    {
        unit.currentCell = cell;
        cell.currentUnit = unit;
    }

    public static bool IsAdjacent(Cell cell1, Cell cell2)
    {
        if (SameCell(cell1, cell2)) throw new System.Exception("IsAdjacent : same position");
        int distance = Distance(cell1, cell2);
        return (distance == 1);
    }

    public static int Distance(Cell cell1, Cell cell2)
    {
        return Mathf.Abs(cell1.x - cell2.x) + Mathf.Abs(cell1.y - cell2.y);
    }

    public static bool SameCell(Cell cell1, Cell cell2)
    {
        return cell1.x == cell2.x && cell1.y == cell2.y;
    }
}

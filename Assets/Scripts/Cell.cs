using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x;
    public int y;
    public enum CellType { NORMAL, HOLE, OBSTACLE }
    public CellType type;
    public CellType Type
    {
        get { return type; }
        set { type = value; ChangeSkin(); }
    }

    public void SetPosition(int xParam, int yParam)
    {
        x = xParam;
        y = yParam;
    }

    public void SetTypeFromString(string typeStr)
    {
        if (typeStr == "0")
        {
            Type = CellType.NORMAL;
        }
        else if (typeStr == "1")
        {
            Type = CellType.HOLE;
        }
        else if (typeStr == "2")
        {
            Type = CellType.OBSTACLE;
        }
    }

    private void ChangeSkin()
    {
        switch(Type)
        {
            case CellType.HOLE:
                GetComponent<SpriteRenderer>().color = Color.black; break;
            case CellType.OBSTACLE:
                GetComponent<SpriteRenderer>().color = Color.red; break;
            case CellType.NORMAL:
            default:
                GetComponent<SpriteRenderer>().color = Color.white; break;
        }
    }
}

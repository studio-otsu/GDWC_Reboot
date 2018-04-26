using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x;
    public int y;
    public enum CellType { NORMAL, HOLE, OBSTACLE }
    private CellType TYPE;
    public CellType type
    {
        get { return TYPE; }
        set { TYPE = value; ChangeSkin(); }
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
            type = CellType.NORMAL;
        }
        else if (typeStr == "1")
        {
            type = CellType.HOLE;
        }
        else if (typeStr == "2")
        {
            type = CellType.OBSTACLE;
        }
    }

    private void ChangeSkin()
    {
        switch(type)
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

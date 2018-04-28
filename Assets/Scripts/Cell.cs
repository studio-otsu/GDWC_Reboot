using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Unit currentUnit;
    public bool marked = false;

    public int x;
    public int y;
    public enum CellType { NORMAL, HOLE, OBSTACLE }
    private CellType TYPE;
    public CellType type
    {
        get { return TYPE; }
        set { TYPE = value; PutDefaultSkin(); }
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

    public void PutDefaultSkin()
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

    public void PutHoverSkin()
    {
        if (type == CellType.NORMAL)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    public void PutDisplayMPSkin()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.8f, 0.1f);
    }

    public void PutChosenPathSkin()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.4f, 0.9f);
    }

    private void OnMouseEnter()
    {
        MatchController.instance.OnMouseEnterNewCell(this);
        //PutHoverSkin();
    }

    private void OnMouseExit()
    {
        //PutDefaultSkin();
    }

    private void OnMouseDown()
    {
        MatchController.instance.OnMouseDownCell(this);
    }


}

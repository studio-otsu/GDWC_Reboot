using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
    public Unit currentUnit;
    public bool marked = false;

    public int x;
    public int y;
    public enum CellType { NORMAL, HOLE, OBSTACLE }
    private CellType TYPE;
    public CellType type {
        get { return TYPE; }
        set { TYPE = value; PutDefaultSkin(); }
    }

    public void SetPosition(int xParam, int yParam) {
        x = xParam;
        y = yParam;
    }

    public void SetTypeFromString(string typeStr) {
        if (typeStr == "0") {
            type = CellType.NORMAL;
        } else if (typeStr == "1") {
            type = CellType.HOLE;
        } else if (typeStr == "2") {
            type = CellType.OBSTACLE;
        }
    }

    public void PutDefaultSkin() {
        switch (type) {
            case CellType.HOLE:
                GetComponent<SpriteRenderer>().color = Color.black; break;
            case CellType.OBSTACLE:
                GetComponent<SpriteRenderer>().color = Color.gray * 0.75f; break;
            //case CellType.NORMAL:
            default:
                GetComponent<SpriteRenderer>().color = Color.white; break;
        }
    }

    public void PutHoverSkin() {
        if (type == CellType.NORMAL) {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    public void PutMoveSkin() {
        GetComponent<SpriteRenderer>().color = new Color(0.1f, 1, 0.3f);
    }

    public void PutPathSkin() {
        GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.3f, 1);
    }

    public void PutSpellRangeSkin() {
        if (type == CellType.NORMAL) {
            GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.4f, 0);
        }
    }

    public void PutSpellAreaSkin() {
        if (type == CellType.NORMAL) {
            GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.25f, 0.25f);
        }
    }

    private void OnMouseEnter() {
        MatchController.instance.OnMouseEnterNewCell(this);
        //PutHoverSkin();
    }

    private void OnMouseExit() {
        //PutDefaultSkin();
    }

    private void OnMouseDown() {
        MatchController.instance.OnMouseDownCell(this);
    }

    private void OnMouseUp() {
        MatchController.instance.OnMouseUpCell(this);
    }


}

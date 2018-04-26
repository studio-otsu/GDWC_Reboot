using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x;
    public int y;

    public void SetPosition(int xParam, int yParam)
    {
        x = xParam;
        y = yParam;
    }
}

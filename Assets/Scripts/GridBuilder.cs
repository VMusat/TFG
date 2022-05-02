using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GridBuilder
{
    private Vector3 origin;
    private int w;
    private int h;
    private int y;
    private float cellSize;
    private Building[,] gridArray;

    public GridBuilder(int w, int h, float cellSize, Vector3 origin){
        this.origin = origin - new Vector3(w,0,h);
        this.w = w;
        this.h = h;
        this.cellSize = cellSize;

        gridArray = new Building[w,h];
        //(int)Base.transform.position.x-w
        for (int x=0; x<gridArray.GetLength(0); x++){
            for (int z=0; z<gridArray.GetLength(1); z++){
                gridArray[x,z] = null;
                Debug.DrawLine(GetWorldPosition(x,z), GetWorldPosition(x,z+1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x,z), GetWorldPosition(x+1,z), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0,h), GetWorldPosition(w,h), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(h,0), GetWorldPosition(w,h), Color.white, 100f);
    }

    public Vector3 GetWorldPosition(int x, int z){
        return new Vector3(x,0,z) * cellSize + origin;
    }

    public Vector3 GetBuildingPosition(int x, int z){
        return GetWorldPosition(x,z) + new Vector3(cellSize, 0, cellSize) * 0.5f;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z){
        x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - origin).z / cellSize);
    }

    public void SetValue(int x, int z, Building value){
        if (x >= 0 && z >= 0 && x < w && z < h){
            gridArray[x,z] = value;
        }
    }

    public bool IsFree(int x, int z){
        if (x >= 0 && z >= 0 && x < w && z < h){
            return (gridArray[x,z] is null);
        }
        return false;
    }

    public int getXSize(){
        return gridArray.GetLength(0);
    }

    public int getZSize(){
        return gridArray.GetLength(1);
    }
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGrid : MonoBehaviour
{
    private GridBuilder grid;
    public int unitsOnBase;
    public List<Collider> colls;
    void Start()
    {
        GridBuilder grid = new GridBuilder(10,5,2, transform.position);
        this.grid = grid;
        this.colls = new List<Collider>();
    }

    void Update() {
        for (int j = 0; j < colls.Count; ++j)
                if (colls[j] == null) colls.RemoveAt(j--);
        unitsOnBase = colls.Count;
    }


    public bool Ocupar(Building buil){
        int x=0, z=0;
        bool free = false;
        int sizeX = grid.getXSize();
        int sizeZ = grid.getZSize();
        for (int j=0; j<sizeZ && !free; j++){
            for (int i=0; i<sizeX && !free; i++){
                if (grid.IsFree(i,j)){
                    free=true;
                    x=i;
                    z=j;
                }
            }
        }
        if(free){
            //grid.GetXZ(worldPosition,out x,out z);
            grid.SetValue(x, z, buil);
            string data = buil.data;
            GameObject b = Instantiate(Resources.Load($"Prefabs/Buildings/{data}") as GameObject, grid.GetBuildingPosition(x,z), Quaternion.identity);
            b.transform.SetParent(transform, true);
        }
        return free;
    }

    public void Liberar(){
        if (grid != null){
            int x=0, z=0;
            bool free = false;
            int sizeX = grid.getXSize();
            int sizeZ = grid.getZSize();
            for (int j=0; j<sizeZ && !free; j++){
                for (int i=0; i<sizeX && !free; i++){
                    if (!grid.IsFree(i,j)){
                        x=i;
                        z=j;
                        grid.SetValue(x, z, null);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Unit>() != null)
        {
            //unitsOnBase += 1;
            this.colls.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Unit>() != null)
        {
            //unitsOnBase -= 1;
            this.colls.Remove(other);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public string data;
    public static int cost;
    public Type resourceType;
    public float resourceAmount;
    public Building()
    {

    }
    public Building(string data)
    {
        this.data = data;
    }

    public int getCost(){
        return cost;
    }

    public enum Type{
        population,
        food,
        materials
    }

}

public class House : Building{
    public House(){
        this.data = "House";
        cost = 5;
        this.resourceType = Type.population;
        this.resourceAmount = 10;
    }
}

public class Farm : Building{
    public Farm(){
        this.data = "Farm";
        cost = 15;
        this.resourceType = Type.food;
        this.resourceAmount = 0.2f;
    }
}

public class Sawmill : Building{
    public Sawmill(){
        this.data = "Sawmill";
        cost = 10;
        this.resourceType = Type.materials;
        this.resourceAmount = 0.2f;
    }
}

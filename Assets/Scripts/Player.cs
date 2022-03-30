using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public int population {get; private set; }
    public int popMax {get; private set; }
    public int popPerSec {get; private set; }
    public int food {get; private set; }
    public int foodMax {get; private set; }
    public int foodPerSec {get; private set; }
    public int materials {get; private set; }
    public int matPerSec {get; private set; }
    public int popFoodConst {get; private set; }


    void Start()
    {
        population = 50;
        popMax = 100;
        popPerSec = 1;
        food = 50;
        foodPerSec = 1;
        materials = 50;
        matPerSec = 1;
        popFoodConst = 150;
        StopAllCoroutines();
        StartCoroutine(ResourceLoop());
    }

    void Update()
    {
        
    }

    private WaitForSeconds WaitFor1Second = new WaitForSeconds(1.0f);
    public IEnumerator ResourceLoop(){
        while(true){
            yield return WaitFor1Second;
            calcPop();
            addPop(popPerSec);
            addFood(foodPerSec);
            addMat(matPerSec);
        }
    }

    public void calcPop(){
        int popAux = popPerSec;
        if(food>0){
            popAux = ((food-population)/popFoodConst)^3+1;
        }
        if (popAux>10){ popAux = 10;}
        if (popAux<-7){ popAux = -7;}
        popPerSec = popAux;
    }

    public void addPop(int pop){
        if(population+pop < popMax){
            population += pop;
        }else{
            population = popMax;
        }
    }
    public void addFood(int fo){
        food += fo;
    }
    public void addMat(int mat){
        mat += mat;
    }
}

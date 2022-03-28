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
    public int foodPerSec {get; private set; }
    public int materials {get; private set; }
    public int matPerSec {get; private set; }


    void Start()
    {
        population = 50;
        popMax = 100;
        popPerSec = 1;
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
            addPop(popPerSec);
        }
    }

    public void addPop(int pop){
        population += pop;

    }
}

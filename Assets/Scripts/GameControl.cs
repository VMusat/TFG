using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameControl : MonoBehaviour
{
    public bool DontReset;
    public bool EnableAgent1 = true;
    public bool EnableAgent2 = true;
    public Player Team1;
    public Player Team2;
    public Tower Team1Tower;
    public Tower Team2Tower;
    public PlayerAgent AgentTeam1;
    public PlayerAgent AgentTeam2;
    public GameObject VictoryCanvas;
    public GameObject GameOverCanvas;
    public List<List<int[]>> Agent1List;
    public List<List<int[]>> Agent2List;
    public List<int> TieWinLoseList;
    public List<int[]> GameList;
    private bool obtaining = false;

     private void Start()
    {
        Agent1List = new List<List<int[]>>();
        Agent2List = new List<List<int[]>>();
        TieWinLoseList = new List<int>();
        GameList = new List<int[]>();
        GameList.Add(new int[1]);
        GameList.Add(new int[4]);
        GameList.Add(new int[1]);
        GameList.Add(new int[3]);
        Reset();
    }

    private void OnEnable()
    {
        Team1Tower.OnDestroyed += OnTeam1TowerDestroyed;
        Team2Tower.OnDestroyed += OnTeam2TowerDestroyed;
    }

    private void OnDisable()
    {
        Team1Tower.OnDestroyed -= OnTeam1TowerDestroyed;
        Team2Tower.OnDestroyed -= OnTeam2TowerDestroyed;
    }

        private void Reset()
    {
        Team1.Reset();
        Team2.Reset();
        StopAllCoroutines();
        StartCoroutine(PenalizeTime());
        StartCoroutine(EndEpisodeTime());
    }

        private IEnumerator PenalizeTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            if (EnableAgent1)
                AgentTeam1.PenalizeTime();
            if (EnableAgent2)
                AgentTeam2.PenalizeTime();
        }
    }

        private IEnumerator EndEpisodeTime()
    {
        yield return new WaitForSeconds(480.0f);
        if (EnableAgent1) AgentTeam1.Tie();
        if (EnableAgent2) AgentTeam2.Tie();
        
        if(!obtaining) ObtainResults(0);

        StopAllCoroutines();
        if (!DontReset) Reset();
    }

        private void OnTeam1TowerDestroyed()
    {
        if (EnableAgent1)
            AgentTeam1.PenalizeTowerDestroyed();
        if (EnableAgent2)
            AgentTeam2.RewardTowerDestroyed();
        if(!obtaining) ObtainResults(2);

        if (!DontReset) Reset();
        else if (!VictoryCanvas.activeSelf) GameOverCanvas.SetActive(true);
    }

    private void OnTeam2TowerDestroyed()
    {
        if (EnableAgent1)
            AgentTeam1.RewardTowerDestroyed();
        if (EnableAgent2)
            AgentTeam2.PenalizeTowerDestroyed();
        if(!obtaining) ObtainResults(1);

        if (!DontReset) Reset();
        else if (!GameOverCanvas.activeSelf) VictoryCanvas.SetActive(true);
    }

    public void OnApplicationQuit() {
        int numGame = 1;
        bool invalid = false;
        List<int> invIdx = new List<int>();
        Directory.CreateDirectory("./Resultados/"+this.name);
        using (var file = File.CreateText("./Resultados/"+this.name+"/ag1.txt"))
        foreach (List<int[]> i in Agent1List){
            Debug.Log("Agente1 en juego numero "+ numGame +":\n");
            Debug.Log(  "Total unidades: "+ i[0][0]+ "\n"+
                        "Total soldados: "+ i[1][0]+ "Total caballeros: "+ i[1][1]+ "Total catapultas: "+ i[1][2]+ "Total barbaros: "+ i[1][3]+ "\n"+
                        "Total construicciones: "+ i[2][0]+ "\n"+
                        "Total hogares: "+ i[3][0]+ "Total granjas: "+ i[3][1]+ "Total aserraderos: "+ i[3][2]+ "\n"
            );
            invalid = i[0][0] == 0 && i[2][0] == 0;
            if(!invalid){
                file.WriteLine("");
                for (int j =0; j<4; j++){
                file.Write(string.Join(",", i[j]));
                if(j<3) file.Write(",");
                }    
            }else{
                invIdx.Add(numGame-1);
            }  
            numGame = numGame +1;
        }
        using (var file = File.CreateText("./Resultados/"+this.name+"/ag2.txt"))
        foreach (List<int[]> i in Agent2List){
            Debug.Log("Agente2 en juego numero "+ numGame +":\n");
            Debug.Log(  "Total unidades: "+ i[0][0]+ "\n"+
                        "Total soldados: "+ i[1][0]+ "Total caballeros: "+ i[1][1]+ "Total catapultas: "+ i[1][2]+ "Total barbaros: "+ i[1][3]+ "\n"+
                        "Total construicciones: "+ i[2][0]+ "\n"+
                        "Total hogares: "+ i[3][0]+ "Total granjas: "+ i[3][1]+ "Total aserraderos: "+ i[3][2]+ "\n"
            );
            invalid = i[0][0] == 0 && i[2][0] == 0;
            if(!invalid){
                file.WriteLine("");
                for (int j =0; j<4; j++){
                    file.Write(string.Join(",", i[j]));
                    if(j<3) file.Write(",");
                }  
            }        
            numGame = numGame +1;
        }

        using (var file = File.CreateText("./Resultados/"+this.name+"/total.txt")){
            Debug.Log("Total de juegos: \n");
            List<int[]> x = GameList;
            Debug.Log(  "Total unidades: "+ x[0][0]+ "\n"+
                            "Total soldados: "+ x[1][0]+ "Total caballeros: "+ x[1][1]+ "Total catapultas: "+ x[1][2]+ "Total barbaros: "+ x[1][3]+ "\n"+
                            "Total construicciones: "+ x[2][0]+ "\n"+
                            "Total hogares: "+ x[3][0]+ "Total granjas: "+ x[3][1]+ "Total aserraderos: "+ x[3][2]+ "\n"
            );
            file.WriteLine("");
                for (int j =0; j<4; j++){
                    file.Write(string.Join(",", x[j]));
                    if(j<3) file.Write(",");
                }
        }
            
        using (var file = File.CreateText("./Resultados/"+this.name+"/history.txt")){
            Debug.Log("Historial de juegos: \n");
            file.WriteLine("");
            for (int y=0; y<TieWinLoseList.Count; y++){
                // Debug.Log(j.ToString());
                if(!invIdx.Contains(y)){
                    file.Write(TieWinLoseList[y]);
                    if(y<TieWinLoseList.Count-1) file.Write(",");
                } 
            }
        }
    }

    public void ObtainResults(int res){
        obtaining = true;
        List<int[]> Aux1 = new List<int[]>();
        List<int[]> Aux2 = new List<int[]>();
        Aux1 = AgentTeam1.returnStats();
        Aux2 = AgentTeam2.returnStats();
        Agent1List.Add(Aux1);
        Agent2List.Add(Aux2);

        GameList[0][0] += (Aux1[0][0] + Aux2[0][0]);

        GameList[1][0] += (Aux1[1][0] + Aux2[1][0]);
        GameList[1][1] += (Aux1[1][1] + Aux2[1][1]);
        GameList[1][2] += (Aux1[1][2] + Aux2[1][2]);
        GameList[1][3] += (Aux1[1][3] + Aux2[1][3]);

        GameList[2][0] += (Aux1[2][0] + Aux2[2][0]);

        GameList[3][0] += (Aux1[3][0] + Aux2[3][0]);
        GameList[3][1] += (Aux1[3][1] + Aux2[3][1]);
        GameList[3][2] += (Aux1[3][2] + Aux2[3][2]);

        TieWinLoseList.Add(res);
        obtaining = false;
    }

}



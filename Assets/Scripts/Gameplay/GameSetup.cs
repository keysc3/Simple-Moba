using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
    public Vector3 spawnPos;

    // Start is called before the first frame update
    void Start()
    {   
        if(GameController.instance.currentChampion != null){
            GameObject myChamp = (GameObject) Instantiate(GameController.instance.currentChampion, spawnPos, Quaternion.identity);     
            ActiveChampion.instance.champions.Add(myChamp);
            ActiveChampion.instance.activeChampion = ActiveChampion.instance.champions.Count-1;
            ActiveChampion.instance.SetActiveChamp();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Back(){
        GameController.instance.currentChampion = null;
        SceneManager.LoadScene("ChampionSelect", LoadSceneMode.Single);
    } 
}

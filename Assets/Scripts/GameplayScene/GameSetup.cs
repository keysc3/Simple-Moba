using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
* Purpose: Handles setting up the Gameplay scene.
*
* @author: Colin Keys
*/
public class GameSetup : MonoBehaviour
{
    public Vector3 spawnPos;

    // Start is called before the first frame update
    void Start()
    {   
        if(GameController.instance.currentChampion != null){
            GameObject myChamp = (GameObject) Instantiate(GameController.instance.currentChampion, spawnPos, Quaternion.identity);     
            NewActiveChampion.instance.champions.Add(myChamp);
            NewActiveChampion.instance.NewActiveChamp = NewActiveChampion.instance.champions.Count-1;
            NewActiveChampion.instance.SetActiveChamp();
        }
    }
    
    /*
    *   Back - Loads the champion select scene.
    */
    public void Back(){
        GameController.instance.SetSelectedChampion(-1);
        GameController.instance.SetCurrentChampion();
        SceneManager.LoadScene("ChampionSelect", LoadSceneMode.Single);
    } 
}

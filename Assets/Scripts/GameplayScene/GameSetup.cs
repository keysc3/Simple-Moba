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
            ActiveChampion.instance.cameraMovement = Camera.main.GetComponent<CameraMovement>();
            GameObject myChamp = (GameObject) Instantiate(GameController.instance.currentChampion, spawnPos, Quaternion.identity);     
            ActiveChampion.instance.champions.Add(myChamp);
            ActiveChampion.instance.players.Add(myChamp.GetComponent<IPlayer>());
            ActiveChampion.instance.ActiveChamp = ActiveChampion.instance.champions.Count-1;
            ActiveChampion.instance.SetActiveChamp();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*   Purpose: Singleton for making sure only one player is being controlled at once.
*
*   Author: Colin Keys
*/
public class ActiveChampion : MonoBehaviour
{
    private int activeChamp;
    public int ActiveChamp { 
        get => activeChamp;
        set {
            if(value <= champions.Count - 1)
                activeChamp = value;
        }
    }
    [field: SerializeField] public List<GameObject> champions { get; private set; } = new List<GameObject>();
    public List<IPlayer> players { get; private set; } = new List<IPlayer>();
    
    public static ActiveChampion instance { get; private set; }
    public CameraMovement cameraMovement;

    // Called when the script instance is being loaded.
    private void Awake(){
        instance = this;
        for(int i = 0; i < champions.Count; i++){
            players.Add(champions[i].GetComponent<IPlayer>());
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        cameraMovement = Camera.main.GetComponent<CameraMovement>();
        SetActiveChamp(); 
    }

    // Update is called once per frame
    private void Update()
    {
        if(GameController.instance.currentChampion == null){
            if(Input.GetKeyDown(KeyCode.J))
                StartCoroutine(PickActiveChamp());
        }
    }

    /*
    *   SetActiveChamp - Enables the active champs controls and disables every others.
    */
    public void SetActiveChamp(){
        for(int i = 0; i < champions.Count; i++){
            if(i != activeChamp){
                champions[i].GetComponent<PlayerControllerBehaviour>().enabled = false;
                champions[i].GetComponent<SpellInputBehaviour>().enabled = false;
                champions[i].tag = "Enemy"; 
                champions[i].layer = LayerMask.NameToLayer("Enemy");
                SetChampionUIActive(false, players[i].playerUI);
            }
            else{
                champions[i].GetComponent<PlayerControllerBehaviour>().enabled = true;
                champions[i].GetComponent<SpellInputBehaviour>().enabled = true;
                champions[i].tag = "Player";
                champions[i].layer = LayerMask.NameToLayer("Default");
                SetChampionUIActive(true, players[i].playerUI);
                cameraMovement.targetObject = champions[i].transform;
            }
        }
        DrawGizmos.instance.drawMethod = null;
    }

    /*
    *   PickActiveChamp - Picks an champion to set as the active champ and begin using.
    */
    private IEnumerator PickActiveChamp(){
        Debug.Log("Select active champion.");
        while(true){
            if(Input.GetKeyDown(KeyCode.C)){
                ActiveChamp = 0;
                break;
            }
            else if(Input.GetKeyDown(KeyCode.V)){
                ActiveChamp = 1;
                break;
            }
            else if(Input.GetKeyDown(KeyCode.B)){
                ActiveChamp = 2;
                break;
            }
            else if(Input.GetKeyDown(KeyCode.N)){
                ActiveChamp = 3;
                break;
            }
            else if(Input.GetKeyDown(KeyCode.M)){
                ActiveChamp = 4;
                break;
            }
            yield return null;
        }
        Debug.Log(champions[activeChamp].name + " was chosen.");
        SetActiveChamp();
    }

    /*
    *   SetChampionUIActive - Sets the champion UI to active or not.
    *   @param isActive - bool of whether or not to activate the spell cover.
    *   @param playerUI - GameObject of the playerUI being updated.
    */
    public void SetChampionUIActive(bool isActive, GameObject playerUI){
        if(playerUI != null)
            playerUI.SetActive(isActive);
    }
}

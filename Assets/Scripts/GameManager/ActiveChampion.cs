using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Sets and changes the active champion for play testing purposes.
*
* @author: Colin Keys
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

    public static ActiveChampion instance { get; private set; }
    public CameraMovement cameraMovement;

    // Called when the script instance is being loaded.
    void Awake(){
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraMovement = Camera.main.GetComponent<CameraMovement>();
        SetActiveChamp(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.instance.currentChampion == null){
            if(Input.GetKeyDown(KeyCode.J))
                StartCoroutine(PickActiveChamp());
        }
    }

    public void SetActiveChamp(){
        for(int i = 0; i < champions.Count; i++){
            if(i != activeChamp){
                champions[i].GetComponent<PlayerController>().enabled = false;
                champions[i].GetComponent<PlayerSpellInput>().enabled = false;
                champions[i].tag = "Enemy"; 
                champions[i].layer = LayerMask.NameToLayer("Enemy");
                UIManager.instance.SetChampionUIActive(false, champions[i].GetComponent<Player>().playerUI);
            }
            else{
                champions[i].GetComponent<PlayerController>().enabled = true;
                champions[i].GetComponent<PlayerSpellInput>().enabled = true;
                champions[i].tag = "Player";
                champions[i].layer = LayerMask.NameToLayer("Default");
                UIManager.instance.SetChampionUIActive(true, champions[i].GetComponent<Player>().playerUI);
                cameraMovement.targetObject = champions[i].transform;
            }
        }
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
}

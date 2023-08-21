using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewActiveChampion : MonoBehaviour
{
    private int newActiveChamp;
    public int NewActiveChamp { 
        get => newActiveChamp;
        set {
            if(value <= champions.Count - 1)
                newActiveChamp = value;
        }
    }
    [field: SerializeField] public List<GameObject> champions { get; private set; } = new List<GameObject>();
    public List<IPlayer> players { get; private set; } = new List<IPlayer>();
    
    public static NewActiveChampion instance { get; private set; }
    public CameraMovement cameraMovement;

    // Called when the script instance is being loaded.
    void Awake(){
        instance = this;
        for(int i = 0; i < champions.Count; i++){
            players.Add(champions[i].GetComponent<IPlayer>());
        }
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
            if(i != newActiveChamp){
                champions[i].GetComponent<PlayerControllerBehaviour>().enabled = false;
                champions[i].GetComponent<SpellInputBehaviour>().enabled = false;
                champions[i].tag = "Enemy"; 
                champions[i].layer = LayerMask.NameToLayer("Enemy");
                //UIManager.instance.SetChampionUIActive(false, champions[i].GetComponent<Player>().playerUI);
            }
            else{
                champions[i].GetComponent<PlayerControllerBehaviour>().enabled = true;
                champions[i].GetComponent<SpellInputBehaviour>().enabled = true;
                champions[i].tag = "Player";
                champions[i].layer = LayerMask.NameToLayer("Default");
                //UIManager.instance.SetChampionUIActive(true, champions[i].GetComponent<Player>().playerUI);
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
                NewActiveChamp = 0;
                break;
            }
            else if(Input.GetKeyDown(KeyCode.V)){
                NewActiveChamp = 1;
                break;
            }
            else if(Input.GetKeyDown(KeyCode.B)){
                NewActiveChamp = 2;
                break;
            }
            else if(Input.GetKeyDown(KeyCode.N)){
                NewActiveChamp = 3;
                break;
            }
            else if(Input.GetKeyDown(KeyCode.M)){
                NewActiveChamp = 4;
                break;
            }
            yield return null;
        }
        Debug.Log(champions[NewActiveChamp].name + " was chosen.");
        SetActiveChamp();
    }
}

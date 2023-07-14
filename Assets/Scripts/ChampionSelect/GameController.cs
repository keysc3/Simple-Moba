using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public List<GameObject> champions;
    public int selectedChampion;
    public GameObject currentChampion;

    void Awake(){
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        champions.Sort(CompareByName);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int CompareByName(GameObject g1, GameObject g2){
        return g1.name.CompareTo(g2.name);
    }
}

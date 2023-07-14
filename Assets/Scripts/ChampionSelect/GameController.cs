using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
* Purpose: Handles the game state and allows for switching between gameplay and champion select scenes.
*
* @author: Colin Keys
*/
public class GameController : MonoBehaviour
{
    public int selectedChampion { get; private set; }
    public static GameController instance { get; private set; }
    public GameObject currentChampion { get; private set; }
    [SerializeField] private List<GameObject> champions;

    // Called when the script instance is being loaded.
    void Awake(){
        // Only want one game controller.
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        // Sort the champions alphabetically.
        champions.Sort(CompareByName);
    }

    /*
    *   SetSelectedChampion - Sets the selected champions index in the champions list.
    *   @param i - int of the index the selected champion is in the champions list.
    */
    public void SetSelectedChampion(int i){
        selectedChampion = i;
    }

    /*
    *   SetCurrentChampion - Sets the currently selected champion.
    */
    public void SetCurrentChampion(){
        if(selectedChampion > -1)
            currentChampion = champions[selectedChampion];
        else
            currentChampion = null;
    }

    /*
    *   CompareByName - Compares two GameObjects by their names.
    *   @param g1 - First GameObject to compare.
    *   @param g2 - Second GameObject to compare.
    *   @return int - int representing the result of the comparison.
    */
    private int CompareByName(GameObject g1, GameObject g2){
        return g1.name.CompareTo(g2.name);
    }
}

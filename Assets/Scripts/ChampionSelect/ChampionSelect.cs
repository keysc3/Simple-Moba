using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
* Purpose: Handles the champion select UI.
*
* @author: Colin Keys
*/
public class ChampionSelect : MonoBehaviour
{

    private ScriptableChampion currentChampion;
    [SerializeField] private List<ScriptableChampion> champions;
    [SerializeField] private GameObject buttonPrefab;
    private float width;
    private Vector2 currentPos = Vector2.zero;

    // Called when the script instance is being loaded.
    void Awake(){
        // Sort the champions alphabetically.
        champions.Sort(CompareByName);
        width = buttonPrefab.GetComponent<RectTransform>().rect.width;
    }

    // Start is called before the first frame update
    void Start()
    {
        int size = champions.Count;
        currentPos.x = currentPos.x - (width * (size - 1));
        foreach(ScriptableChampion champ in champions){
            // Create and setup a new champion button.
            GameObject button = (GameObject) Instantiate(buttonPrefab, Vector2.zero, Quaternion.identity);
            button.transform.SetParent(transform.GetChild(0));
            button.GetComponent<RectTransform>().anchoredPosition = currentPos;
            currentPos.x += width*2;
            button.transform.GetChild(0).GetComponent<TMP_Text>().SetText(champ.name);
            button.name = champ.name;
            button.GetComponent<Image>().sprite = champ.icon;
            // Add the on click for the button.
            button.GetComponent<Button>().onClick.AddListener(() => ChampClick(button.transform));
        }
    }

    /*
    *   ChampClick - Sets the current and selected champion values.
    *   @param button - Transform of the button that was clicked.
    */
    private void ChampClick(Transform button){
        GameController.instance.SetSelectedChampion(button.GetSiblingIndex());
        GameController.instance.SetCurrentChampion();
    }

    /*
    *   StartClick - Loads the Gameplay scene.
    */
    public void StartClick(){
        if(GameController.instance.currentChampion != null)
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    /*
    *   CompareByName - Compares two Champions by their names.
    *   @param c1 - First Champion to compare.
    *   @param c2 - Second Champion to compare.
    *   @return int - int representing the result of the comparison.
    */
    private int CompareByName(ScriptableChampion c1, ScriptableChampion c2){
        return c1.name.CompareTo(c2.name);
    }
}

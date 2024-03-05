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
    [SerializeField] private List<SpellData> sumSpells;
    [SerializeField] private GameObject championButtonPrefab;
    [SerializeField] private GameObject summonerSpellButtonPrefab;
    private float width;
    private Vector2 currentPos = Vector2.zero;

    // Called when the script instance is being loaded.
    void Awake(){
        // Sort the champions alphabetically.
        champions.Sort(CompareByName);
        width = championButtonPrefab.GetComponent<RectTransform>().rect.width;
    }

    // Start is called before the first frame update
    void Start()
    {
        ChampionButtonSetup();
        SummonerSpellButtonSetup(transform.Find("SummonerSpells").GetChild(0).GetComponent<Button>(), 0);
        SummonerSpellButtonSetup(transform.Find("SummonerSpells").GetChild(1).GetComponent<Button>(), 1);
        SummonerSpellPanelSetup(transform.Find("SummonerSpells/SummonerSpellSelect"));
    }

    private void SummonerSpellPanelSetup(Transform parent){
        float spacing = 2f;
        int size = sumSpells.Count;
        float widthPer = 40f;
        Vector2 position = new Vector2(0f, widthPer/2f);
        for(int i = 0; i < size; i++){
            if(i%2 == 0){
                position.x = spacing + widthPer/2f;
                position.y = position.y - widthPer - spacing;
            }
            else
                position.x = position.x + widthPer + spacing;
            // Create and setup a new champion button.
            GameObject button = (GameObject) Instantiate(summonerSpellButtonPrefab, Vector2.zero, Quaternion.identity);
            button.transform.SetParent(parent);
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.0f, 1.0f);
            rt.anchorMax = new Vector2(0.0f, 1.0f);
            rt.anchoredPosition = position;
            rt.sizeDelta = Vector2.one * widthPer;
            button.name = sumSpells[i].name;
            button.GetComponent<Image>().sprite = sumSpells[i].sprite;
            // Add the on click for the button.
            //button.GetComponent<Button>().onClick.AddListener(() => ChampClick(button.transform));
        }
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2((spacing * 3f) + (widthPer * 2f), (-position.y) + spacing + widthPer/2f);
        parent.gameObject.SetActive(false);

    }

    public void SummonerSpellClick(Transform button){
        Transform select = transform.Find("SummonerSpells/SummonerSpellSelect");
        bool isActive = select.gameObject.activeSelf;
        select.gameObject.SetActive(!isActive);
        RectTransform buttonRT = button.GetComponent<RectTransform>();
        RectTransform selectRT = select.GetComponent<RectTransform>();
        selectRT.anchoredPosition = buttonRT.anchoredPosition + new Vector2(0f, selectRT.sizeDelta.y/2f + (buttonRT.sizeDelta.y/2f) + 2f);
    }

    private void SummonerSpellButtonSetup(Button button, int spellIndex){
        button.GetComponent<Image>().sprite = sumSpells[spellIndex].sprite;
        button.GetComponent<Button>().onClick.AddListener(() => SummonerSpellClick(button.transform));
    }

    private void ChampionButtonSetup(){
        int size = champions.Count;
        currentPos.x = currentPos.x - (width * (size - 1));
        foreach(ScriptableChampion champ in champions){
            // Create and setup a new champion button.
            GameObject button = (GameObject) Instantiate(championButtonPrefab, Vector2.zero, Quaternion.identity);
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
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
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

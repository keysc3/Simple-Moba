using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChampionSelect : MonoBehaviour
{

    public Champion currentChampion;
    public List<Champion> champions;
    public GameObject buttonPrefab;
    public float width;
    public Vector2 currentPos = Vector2.zero;
    public Button startButton;

    void Awake(){
        champions.Sort(CompareByName);
        width = buttonPrefab.GetComponent<RectTransform>().rect.width;
    }

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartClick);
        int size = champions.Count;
        currentPos.x = currentPos.x - (width * (size - 1));
        foreach(Champion champ in champions){
            GameObject button = (GameObject) Instantiate(buttonPrefab, Vector2.zero, Quaternion.identity);
            button.transform.SetParent(transform.GetChild(0));
            button.GetComponent<RectTransform>().anchoredPosition = currentPos;
            currentPos.x += width*2;
            button.transform.GetChild(0).GetComponent<TMP_Text>().SetText(champ.name);
            button.name = champ.name;
            button.GetComponent<Image>().sprite = champ.icon;
            button.GetComponent<Button>().onClick.AddListener(() => ChampClick(button.transform));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChampClick(Transform button){
        GameController.instance.selectedChampion = button.GetSiblingIndex();
        GameController.instance.currentChampion = GameController.instance.champions[GameController.instance.selectedChampion];
    }

    private void StartClick(){
        if(GameController.instance.currentChampion != null)
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    private int CompareByName(Champion c1, Champion c2){
        return c1.name.CompareTo(c2.name);
    }
}

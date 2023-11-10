using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
* Purpose: Updates the score portion of the players UI.
*
* @author: Colin Keys
*/
public class PlayerScoreUpdate : MonoBehaviour
{
    private TMP_Text killsText;
    private TMP_Text assistsText;
    private TMP_Text deathsText;
    private TMP_Text csText;

    // Start is called before the first frame update
    void Start()
    {
        SetupCallback();
        killsText = transform.Find("Kills/Value").GetComponent<TMP_Text>();
        assistsText = transform.Find("Assists/Value").GetComponent<TMP_Text>();
        deathsText = transform.Find("Deaths/Value").GetComponent<TMP_Text>();
        csText = transform.Find("CS/Value").GetComponent<TMP_Text>();
    }

    /*
    *   SetupCallback - Adds the UI updating methods to the necessary events.
    */
    private void SetupCallback(){
        IPlayer player = GetComponentInParent<IPlayer>();
        player.score.UpdateScoreCallback += UpdateScore;
    }

    /*
    *   UpdateScore - Updates the text for the given score type.
    *   @param score - Score object to use the data from.
    *   @param updateVariable - string of the score type being updated.
    */
    private void UpdateScore(Score score, string updateVariable){
        switch(updateVariable){
            case "kill":
                killsText.SetText(score.Kills.ToString());
                break;
            case "assist":
                assistsText.SetText(score.Assists.ToString());
                break;
            case "death":
                deathsText.SetText(score.Deaths.ToString());
                break;
            case "cs":
                csText.SetText(score.CS.ToString());
                break;
        }
    }
}

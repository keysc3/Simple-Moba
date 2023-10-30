using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScoreUpdate : MonoBehaviour
{
    private TMP_Text killsText = null;
    private TMP_Text assistsText = null;
    private TMP_Text deathsText = null;
    private TMP_Text csText = null;

    // Start is called before the first frame update
    void Start()
    {
        SetupCallback();
        killsText = transform.Find("Kills/Value").GetComponent<TMP_Text>();
        assistsText = transform.Find("Assists/Value").GetComponent<TMP_Text>();
        deathsText = transform.Find("Deaths/Value").GetComponent<TMP_Text>();
        csText = transform.Find("CS/Value").GetComponent<TMP_Text>();
    }

    private void SetupCallback(){
        IPlayer player = GetComponentInParent<IPlayer>();
        player.score.UpdateScoreCallback += UpdateScore;
    }

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

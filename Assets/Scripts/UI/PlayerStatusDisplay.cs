using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
* Purpose: Updates the status portion of a player bar UI.
*
* @author: Colin Keys
*/
public class PlayerStatusDisplay : MonoBehaviour
{
    private TMP_Text text;
    private Slider slider;
    
    // Start is called before the first frame update
    void Start()
    {
        SetupCallback();
        text = GetComponentInChildren<TMP_Text>();  
        slider = GetComponentInChildren<Slider>();
        gameObject.SetActive(false);
    }

    /*
    *   SetupCallback - Adds the UI updating methods to the necessary events.
    */
    private void SetupCallback(){
        IPlayer player = GetComponentInParent<IPlayer>();
        player.statusEffects.OnDurationUpdate += UpdateStatus;
    }

    /*
    *   UpdateStatus - Updates the status text and slider of the player bar.
    *   @param effect - Effect being updated and displayed.
    */
    private void UpdateStatus(Effect effect){
        if(effect == null || effect.effectType.keyword == "Default"){
            if(gameObject.activeSelf)
                gameObject.SetActive(false);
            return;
        }
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);
        if(text.text != effect.effectType.keyword.ToUpper()){
            text.SetText(effect.effectType.keyword.ToUpper());
        }
        slider.value = 1 - (effect.effectTimer/effect.EffectDuration);
    }
}

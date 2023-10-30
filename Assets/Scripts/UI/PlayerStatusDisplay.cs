using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusDisplay : MonoBehaviour
{
    private TMP_Text text;
    private Slider slider;
    
    /*void Awake(){
        SetupCallback();
    }*/

    // Start is called before the first frame update
    void Start()
    {
        SetupCallback();
        text = GetComponentInChildren<TMP_Text>();  
        slider = GetComponentInChildren<Slider>();
        gameObject.SetActive(false);
    }

    private void SetupCallback(){
        IPlayer player = GetComponentInParent<IPlayer>();
        player.statusEffects.OnDurationUpdate += UpdateStatus;
    }

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

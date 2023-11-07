using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CastBarUI : MonoBehaviour
{
    private Slider castBarSlider;
    private TMP_Text castBarText;

    // Start is called before the first frame update
    void Start()
    {
        castBarSlider = transform.Find("Castbar").GetComponent<Slider>();
        castBarText = transform.Find("Spell").GetComponent<TMP_Text>();
        Spell[] objSpells = GetComponentsInParent<Spell>();
        foreach(Spell spell in objSpells){
            spell.spellController.CastBarUpdateCallback += CastBarUpdate;
        }
        gameObject.SetActive(false);
    }

    /*
    *   CastTime - Stops the champion for the duration of the spells cast.
    *   @param castTime - float for the duration to stop the champion for casting.
    */
    private void CastBarUpdate(float timer, ISpell spell){
        if(spell.spellData.castTime > 0 && timer >= spell.spellData.castTime)
            gameObject.SetActive(false);
        else
            if(!gameObject.activeSelf)
                gameObject.SetActive(true);
            if(castBarText.text != spell.spellData.name)
                castBarText.text = spell.spellData.name;
            castBarSlider.value = Mathf.Clamp(timer/spell.spellData.castTime, 0f, 1f);
    }
}

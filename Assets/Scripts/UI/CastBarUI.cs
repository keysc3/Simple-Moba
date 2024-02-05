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
            SpellCallbacks(spell);
        }
        gameObject.SetActive(false);
    }

    /*
    *   CastBarUpdate - Stops the champion for the duration of the spells cast.
    *   @param timer - float of the elapsed time.
    *   @param name - string of the spells name.
    *   @param castTime - float for the duration to stop the champion for casting.
    *   @param hideOnMax - bool to hide the cast bar or not when max charge reached.
    */
    private void CastBarUpdate(float timer, string name, float castTime, bool hideOnMax){
        if(castTime > 0 && timer >= castTime && hideOnMax == true)
            gameObject.SetActive(false);
        else{
            if(!gameObject.activeSelf)
                gameObject.SetActive(true);
            if(castBarText.text != name)
                castBarText.text = name;
            castBarSlider.value = Mathf.Clamp(timer/castTime, 0f, 1f);
        }
    }

    public void SpellCallbacks(Spell spell){
        spell.spellController.CastBarUpdateCallback += CastBarUpdate;
    }
}

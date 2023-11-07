using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellUI : MonoBehaviour
{
    private GameObject castBar;
    private Slider castBarSlider;
    private TMP_Text castBarText;

    // Start is called before the first frame update
    void Start()
    {
        castBar = transform.Find("CastbarContainer").gameObject;
        castBarSlider = castBar.transform.Find("Castbar").GetComponent<Slider>();
        castBarText = castBar.transform.Find("Spell").GetComponent<TMP_Text>();
        Spell[] objSpells = GetComponentsInParent<Spell>();
        foreach(Spell spell in objSpells){
            spell.spellController.CastBarUpdateCallback += CastBarUpdate;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    *   CastTime - Stops the champion for the duration of the spells cast.
    *   @param castTime - float for the duration to stop the champion for casting.
    */
    private void CastBarUpdate(float timer, ISpell spell){
        Debug.Log(timer + " | " + spell.spellData.castTime);
        if(spell.spellData.castTime > 0 && timer >= spell.spellData.castTime)
            castBar.SetActive(false);
        else
            if(!castBar.activeSelf)
                castBar.SetActive(true);
            if(castBarText.text != spell.spellData.name)
                castBarText.text = spell.spellData.name;
            castBarSlider.value = Mathf.Clamp(timer/spell.spellData.castTime, 0f, 1f);
    }
}

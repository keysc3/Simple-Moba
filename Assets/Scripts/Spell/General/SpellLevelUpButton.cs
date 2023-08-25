using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
*   Purpose: Handles a spell level up buttons actions.
*
*   @author: Colin Keys
*/
public class SpellLevelUpButton : MonoBehaviour, IPointerDownHandler
{
    //TODO validate spell against spell string constants.
    public string spell;
    
    private ISpellInput spellInput;
    public ISpellInput SpellInput {
        get => spellInput;
        set {
            if(value != null)
                spellInput = value; 
        }
    }
    private LevelManager levelManager;
    public LevelManager LevelManager {
        get => levelManager;
        set {
            if(value != null)
                levelManager = value;
        }
    }

    /*
    *   OnPointerDown - Called when the mouse is clicked over the button.
    */
    public void OnPointerDown(PointerEventData eventData){
        if(spellInput != null)
            spellInput.ButtonClick = true;
        if(levelManager != null)
            levelManager.SpellLevelUp(spell);
    }
}

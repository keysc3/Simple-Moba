using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewSpellLevelUpButton : MonoBehaviour, IPointerDownHandler
{
    private ISpellInput _si;
    public ISpellInput _SI {
        get => _si;
        set {
            if(value != null)
                _si = value; 
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
    //TODO validate spell against spell string constants.
    public string spell;

    /*
    *   OnPointerDown - Called when the mouse is clicked over the button.
    */
    public void OnPointerDown(PointerEventData eventData){
        if(_si != null)
            _si.ButtonClick = true;
        if(levelManager != null)
            levelManager.SpellLevelUp(spell);
    }
}

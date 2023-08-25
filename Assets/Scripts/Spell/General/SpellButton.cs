using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
*   Purpose: Handles a spell buttons actions.
*
*   @author: Colin Keys
*/
public class SpellButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    //TODO? could change to properties with validation on spell and keycode based on constant values.
    public ISpell spell;
    public KeyCode keyCode;
    private ISpellInput _si;
    public ISpellInput _SI{
        get => _si;
        set {
            if(value != null){
                _si = value;
                spellInputController = new SpellInputController(value);
            }
        }
    }
    private SpellInputController spellInputController;

    /*
    *   OnPointerEnter - Called when the cursor enters the rect area of this button.
    */
    public void OnPointerEnter(PointerEventData eventData){
        if(DrawGizmos.instance.drawMethod == null)
            spell.DisplayCast();
    }

    /*
    *   OnPointerExit - Called when the cursor exits the rect area of this button.
    */
    public void OnPointerExit(PointerEventData eventData){
        if(_si.LastSpellPressed != spell && spell.IsDisplayed)
            spell.HideCast();
    }

    /*
    *   OnPointerDown - Called when the mouse is clicked over the button.
    */
    public void OnPointerDown(PointerEventData eventData){
        if(spell.SpellNum != "Passive"){
            _si.ButtonClick = true;
            if(spellInputController != null)
                spellInputController.SpellButtonPressed(keyCode, spell);
        }
    }
}
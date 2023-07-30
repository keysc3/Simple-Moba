using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Spell spell;
    public KeyCode keyCode;
    public PlayerSpellInput playerSpellInput;

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Button>().onClick.AddListener(() => ButtonClick());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData){
        spell.DisplayCast();
    }

    public void OnPointerExit(PointerEventData eventData){
        if(playerSpellInput.lastSpellPressed != spell)
            spell.HideCast();
    }

    public void OnPointerDown(PointerEventData eventData){
        playerSpellInput.buttonClick = true;
        playerSpellInput.SpellButtonPressed(keyCode, spell);
    }

    /*private void ButtonClick(){
        Debug.Log("BUTTON CLICKED");
        playerSpellInput.SpellButtonPressed(keyCode, spell);
    }*/
}

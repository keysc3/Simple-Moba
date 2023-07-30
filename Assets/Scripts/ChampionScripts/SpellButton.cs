using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Spell spell;
    public KeyCode keyCode;
    public PlayerSpellInput playerSpellInput;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => ButtonClick());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData){
        spell.DisplayCast();
    }

    public void OnPointerExit(PointerEventData eventData){
        spell.HideCast();
    }

    private void ButtonClick(){
        playerSpellInput.SpellButtonPressed(keyCode, spell);
    }
}

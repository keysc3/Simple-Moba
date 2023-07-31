using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellLevelUpButton : MonoBehaviour, IPointerDownHandler
{
    public Player player;
    public string spell;
    public PlayerSpellInput playerSpellInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData){
        playerSpellInput.buttonClick = true;
        player.levelManager.SpellLevelUp(spell);
    }
}

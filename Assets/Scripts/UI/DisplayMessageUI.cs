using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayMessageUI : MonoBehaviour
{
    private TMP_Text text;
    private float maxDisplay = 2.0f;
    private Coroutine running;
    
    // Start is called before the first frame update
    void Start()
    {
        text = transform.GetComponent<TMP_Text>();
        Spell[] objSpells = GetComponentsInParent<Spell>();
        foreach(Spell spell in objSpells){
            SpellCallbacks(spell);
        }
    }

    public void DisplayMessageUpdate(string message){
        if(running != null){
            StopCoroutine(running);
        }
        text.text = message;
        running = StartCoroutine(DisplayMessageTimer());
    }

    private IEnumerator DisplayMessageTimer(){
        float timer = 0f;
        while(timer < maxDisplay){
            timer += Time.deltaTime;
            yield return null;
        }
        text.text = "";
        running = null;
    }
    
    public void SpellCallbacks(Spell spell){
        spell.DisplayMessageCallback += DisplayMessageUpdate;
    }
}

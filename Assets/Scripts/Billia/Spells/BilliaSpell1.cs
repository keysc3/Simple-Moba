using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpell1 : DamageSpell, IHasCallback
{

    public BilliaSpell1(ChampionSpells championSpells) : base(championSpells){

    }

    public override void Cast(){
        spellHitCallback?.Invoke();
        Debug.Log("Spell1");
        Hit();
    }

    public override void Hit(){
        Debug.Log("Spell1Hit");
        AddPassiveStack();
        championSpells.StartCoroutine(TestCoroutine());
    }

    public void AddPassiveStack(){
        Debug.Log("Spell1PassiveStack");
    }

    public void SetupCallbacks(List<Spell> mySpells){
        foreach(Spell newSpell in mySpells){
            if(newSpell is DamageSpell && !(newSpell is BilliaSpell1)){
                ((DamageSpell) newSpell).spellHitCallback += AddPassiveStack;
            }
        }
    }

    public IEnumerator TestCoroutine(){
        while(true){
            Debug.Log("TestCouroutine running");
            yield return new WaitForSeconds(2f);
        }
    }
}

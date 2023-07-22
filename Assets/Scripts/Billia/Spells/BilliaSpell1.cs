using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpell1 : DamageSpell, IHasCallback
{

    private BilliaSpell1Data spell1Data;

    public BilliaSpell1(ChampionSpells championSpells, SpellData spell1Data) : base(championSpells){
        this.spell1Data = (BilliaSpell1Data) spell1Data;
    }

    public override void Cast(){
        Debug.Log("Spell1");
        Hit();
    }

    public override void Hit(){
        GameObject hitObject = gameObject;
        spellHitCallback?.Invoke(hitObject);
        Debug.Log("Spell1Hit");
        AddPassiveStack(hitObject);
        championSpells.StartCoroutine(TestCoroutine());
    }

    public void AddPassiveStack(GameObject hit){
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

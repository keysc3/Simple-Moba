using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgeSpell4 : Spell, IHasHit, IHasCast, IHasCallback
{
    public List<ISpell> callbackSet { get; } = new List<ISpell>();
    public SpellHitCallback spellHitCallback { get; set; }

    new private BurgeSpell4Data spellData;
    private float currentFill = 0f;
    private bool casted = false;
    private int castedHits = 0;

    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        this.spellData = (BurgeSpell4Data) base.spellData;
        IsQuickCast = true;
    }

    public void Cast(){
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            StartCoroutine(spellController.CastTime());
            // Use mana.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
        }      
    }

    public void BasicSpellHit(IUnit hitUnit, ISpell spellHit){
        if(!casted){
            if(currentFill < 100f)
                currentFill = Mathf.Clamp(spellData.spellFill + currentFill, 0f, 100f);
        }
        else{
            castedHits += 1;
        }
    }

    public void Hit(IUnit hit){

    }

    /*
    *   SetupCallbacks - Sets up the necessary callbacks for the spell.
    *   @param spells - Dictionary of the current spells.
    */
    public void SetupCallbacks(Dictionary<SpellType, ISpell> spells){
        // If the Spell is a DamageSpell then add this spells passive proc to its spell hit callback.
        foreach(KeyValuePair<SpellType, ISpell> entry in spells){
            if(entry.Value is IHasHit && !(entry.Value is BilliaSpell1)){
                ((IHasHit) entry.Value).spellHitCallback += BasicSpellHit;
                callbackSet.Add(entry.Value);
            }
        }
    }
}

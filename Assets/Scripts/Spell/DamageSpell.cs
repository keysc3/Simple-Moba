using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageSpell : Spell
{
    
    public DamageSpell(ChampionSpells championSpells, string spellNum) : base(championSpells, spellNum){

    }

    public delegate void SpellHitCallback(GameObject hit, Spell spellHit); 
    public SpellHitCallback spellHitCallback;

    public abstract void Hit(GameObject hit);

}

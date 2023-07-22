using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageSpell : Spell
{
    
    public DamageSpell(ChampionSpells championSpells) : base(championSpells){

    }

    public delegate void SpellHitCallback(GameObject hit); 
    public SpellHitCallback spellHitCallback;

    public abstract void Hit();

}

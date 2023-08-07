using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a damage spell.
*
* @author: Colin Keys
*/
public abstract class DamageSpell : Spell
{
    /*
    *   DamageSpell - Creates a damage spell.
    *   @param championSpells - ChampionSpells instance this damage spell is a part of.
    *   @param spellNum - string of the spell number this damage spell is.
    */
    public DamageSpell(ChampionSpells championSpells, SpellData spellData) : base(championSpells, spellData){}

    public delegate void SpellHitCallback(GameObject hit, Spell spellHit); 
    public SpellHitCallback spellHitCallback;

    /*
    *   Hit - Method for actions when the damage spell hit a GameObject.
    *   @param hit - GameObject that was hit.
    */
    public abstract void Hit(GameObject hit);
}

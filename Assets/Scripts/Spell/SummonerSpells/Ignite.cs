using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

/*
* Purpose: Implements the ignite summoner spell. Cast on a target to deal true damage over an amount of time.
*
* @author: Colin Keys
*/
public class Ignite : Spell, IHasTargetedCast
{
    new private IgniteData spellData;
    private NavMeshAgent navMeshAgent;
    private ISpellInput spellInput;

    // Called when the script instance is being loaded.
    protected override void Awake(){
        base.Awake();
        IsSummonerSpell = true;
    }

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (IgniteData) base.spellData;
        navMeshAgent = GetComponent<NavMeshAgent>();
        spellInput = GetComponent<ISpellInput>();
        IsSummonerSpell = true;
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Handles.color = Color.cyan;
        Vector3 drawPosition = transform.position;
        drawPosition.y -= (myCollider.bounds.size.y/2) + 0.01f;
        Handles.DrawWireDisc(drawPosition, Vector3.up, spellData.maxMagnitude, 1f);
    }

    /*
    *   AttemptCast - Checks to see if the spell can be cast.
    */
    public void AttemptCast(IUnit unit){
        if(!player.IsCasting){
            if(unit is IPlayer){
                if(spellController.CheckInRange(unit, spellData.maxMagnitude)){
                    Cast(unit);
                }
                else{
                    StartCoroutine(spellController.MoveTowardsSpellTarget(unit, spellData.maxMagnitude));
                }
            }
        }
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(IUnit unit){
        if(!OnCd){
            unit.statusEffects.AddEffect(spellData.dot.InitializeEffect(CalculateTotalDamage(), 0, player, unit));
            OnCd = true;
            StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[0]));
        }
    }

    /*
    * CalculateTotalDamage - Calculates the amount of damage the spell will do.
    * @return float - float of the amount of damage the spell with deal.
    */
    private float CalculateTotalDamage(){
        return 50f + (20f * player.levelManager.Level);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements a charm effect where the charmed moves towards the unit that charmed them.
*
* @author: Colin Keys
*/
public class Charm : Effect
{
    private int spellLevel;
    private Vector3 currentTarget;
    //private NavMeshAgent effectedNavMeshAgent;
    private Unit effectedUnit;
    //private Collider effectedCollider;
    private PlayerController playerController;
    private PlayerSpellInput playerSpellInput;
    
    /*
    *   Charm - Initialize a new charm effect.
    *   @param charmEffect - ScriptableCharm of the charm effect to apply.
    *   @param duration - float of the duration for the charm to last.
    *   @param unitCasted - GameObject of the unit that casted the charm.
    *   @param - unitEffected - GameObject of the unit that the charm is affecting.
    */
    public Charm(ScriptableCharm charmEffect, float duration, int spellLevel, GameObject unitCasted, GameObject unitEffected) : base(charmEffect, duration, unitCasted, unitEffected){
        this.spellLevel = spellLevel;
        effectedUnit = effected.GetComponent<Unit>();
    }

    /*
    *   StartEffect - Start the charm effect.
    */
    public override void StartEffect(){
        // If the charmed unit ia a champion disable their controls.
        if(effectedUnit.SUnit is ScriptableChampion){
            playerController = effected.GetComponent<PlayerController>();
            playerController.enabled = false;
            playerSpellInput = effected.GetComponent<PlayerSpellInput>();
            playerSpellInput.enabled = false;
        }
        // Reset the units current path.
        effectedUnit.navMeshAgent.ResetPath();
        effectedUnit.statusEffects.AddEffect(((ScriptableCharm) effectType).slow
        .InitializeEffect(spellLevel, casted, effected));
    }

    /*
    *   EndEffect - End the charm effect.
    */
    public override void EndEffect(){
        // Reset the path and speed from the charm effect.
        effectedUnit.navMeshAgent.ResetPath();
        
        // Give controls back if charmed is active GameObject.
        if(effectedUnit.SUnit is ScriptableChampion && ActiveChampion.instance.champions[ActiveChampion.instance.ActiveChamp] == effected){
            playerController.enabled = true;
            playerSpellInput.enabled = true;
        }
    }

    /*
    *   EffectTick - Tick for the charms effect.
    */
    public override void EffectTick(){
        effectedUnit.navMeshAgent.destination = casted.transform.position;
        Vector3 nextTarget;
        // If a path is set.
        if(effectedUnit.navMeshAgent.hasPath){
            nextTarget = effectedUnit.navMeshAgent.steeringTarget;
            // If a new target location exists set the target and look at the target location.
            if(currentTarget != nextTarget){
                nextTarget.y = effectedUnit.myCollider.bounds.center.y;
                effected.transform.LookAt(nextTarget);
                currentTarget = nextTarget;
            }
        } 
    }
}

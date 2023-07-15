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
    private NavMeshAgent effectedNavMeshAgent;
    private Unit effectedUnit;
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
        effectedNavMeshAgent = effected.GetComponent<NavMeshAgent>();
    }

    /*
    *   StartEffect - Start the charm effect.
    */
    public override void StartEffect(){
        // If the charmed unit ia a champion disable their controls.
        if(effectedUnit.unit is ScriptableChampion){
            playerController = effected.GetComponent<PlayerController>();
            playerController.enabled = false;
            playerSpellInput = effected.GetComponent<PlayerSpellInput>();
            playerSpellInput.enabled = false;
        }
        // Reset the units current path.
        effectedNavMeshAgent.ResetPath();
        effectedUnit.statusEffects.AddEffect(((ScriptableCharm) effectType).slow
        .InitializeEffect(spellLevel, casted, effected));
    }

    /*
    *   EndEffect - End the charm effect.
    */
    public override void EndEffect(){
        // Reset the path and speed from the charm effect.
        effectedNavMeshAgent.ResetPath();
        
        // Give controls back if charmed is active GameObject.
        if(effectedUnit.unit is ScriptableChampion && ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == effected){
            playerController.enabled = true;
            playerSpellInput.enabled = true;
        }
    }

    /*
    *   EffectTick - Tick for the charms effect.
    */
    public override void EffectTick(){
        effectedNavMeshAgent.destination = casted.transform.position;
        Vector3 nextTarget;
        // If a path is set.
        if(effectedNavMeshAgent.hasPath){
            nextTarget = effectedNavMeshAgent.steeringTarget;
            // If a new target location exists set the target and look at the target location.
            if(currentTarget != nextTarget){
                nextTarget.y = effectedCollider.bounds.center.y;
                effected.transform.LookAt(nextTarget);
                currentTarget = nextTarget;
            }
        } 
    }
}

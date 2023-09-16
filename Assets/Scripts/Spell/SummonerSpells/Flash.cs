using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class Flash : Spell, IHasCast
{
    new private FlashData spellData;
    private NavMeshAgent navMeshAgent;

    // Called when the script instance is being loaded.
    protected override void Awake(){
        base.Awake();
        IsSummonerSpell = true;
    }

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (FlashData) base.spellData;
        if(SpellNum == null)
            SpellNum = spellData.defaultSpellNum;
        navMeshAgent = GetComponent<NavMeshAgent>();
        IsQuickCast = true;
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
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = spellController.GetTargetDirection();
           // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - transform.position);
            if(targetPosition.magnitude > spellData.maxMagnitude)
                targetPosition = transform.position + (targetPosition.normalized * spellData.maxMagnitude);
            else
                targetPosition += transform.position;
            Move(spellController.GetPositionOnWalkableNavMesh(targetPosition, false));
            OnCd = true;
        }
    }

    private void Move(Vector3 destination){
        navMeshAgent.ResetPath();
        navMeshAgent.enabled = false;
        transform.position = destination;
        navMeshAgent.enabled = true;
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[0]));
    }
}

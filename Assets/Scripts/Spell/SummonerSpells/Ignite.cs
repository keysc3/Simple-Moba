using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

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
        if(SpellNum == null)
            SpellNum = spellData.defaultSpellNum;
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
    *   Cast - Casts the spell.
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

    public void Cast(IUnit unit){
        unit.statusEffects.AddEffect(spellData.dot.InitializeEffect(CalculateTotalDamage(), 0, player, unit));
        OnCd = true;
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[0]));
    }

    /*private bool CheckInRange(IUnit unit){
        float distToTarget = (transform.position - unit.Position).magnitude;
        return distToTarget <= spellData.maxMagnitude;
    }

    private IEnumerator MoveTowardsTarget(IUnit unit){
        Debug.Log(CheckInRange(unit));
        while(!CheckInRange(unit)){
            Debug.Log("Here: " + CheckInRange(unit));
            if(spellInput.LastSpellPressed != (ISpell) this && spellInput.LastSpellPressed == null){
                if(!Input.GetMouseButtonDown(1))
                    navMeshAgent.ResetPath();
                yield break;
            }
            navMeshAgent.destination = unit.Position;
            yield return null;
        }
        navMeshAgent.ResetPath();
        ApplySpell(unit);
    }*/

    private float CalculateTotalDamage(){
        return 50f + (20f * player.levelManager.Level);
    }
}

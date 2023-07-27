using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[System.Serializable]
public class Spell
{
    //ScriptableSpell spell;
    [SerializeField] public bool onCd { get; protected set; } = false;
    [SerializeField] public bool canMove = false;
    [SerializeField] public bool isQuickCast = false;
    [SerializeField] public string spellNum { get; private set; }
    [SerializeField] protected NavMeshAgent navMeshAgent;
    [SerializeField] protected LevelManager levelManager;
    [SerializeField] protected ChampionStats championStats;
    [SerializeField] protected Player player;
    protected ChampionSpells championSpells;
    protected GameObject gameObject;

    protected Collider myCollider;
    protected Camera mainCamera;
    
    public Spell(ChampionSpells championSpells, string spellNum){
        this.championSpells = championSpells;
        this.spellNum = spellNum;
        mainCamera = Camera.main;
        player = championSpells.gameObject.GetComponent<Player>();
        myCollider = championSpells.gameObject.GetComponent<Collider>();
        navMeshAgent = championSpells.gameObject.GetComponent<NavMeshAgent>();
        championStats = (ChampionStats) player.unitStats;
        levelManager = player.levelManager;
        this.gameObject = championSpells.gameObject;
    }

    protected Vector3 GetTargetDirection(){
        RaycastHit hitInfo;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        LayerMask groundMask = LayerMask.GetMask("Ground");
        Physics.Raycast(ray, out hitInfo, Mathf.Infinity, groundMask);
        Vector3 targetDirection = hitInfo.point;
        player.SetMouseOnCast(targetDirection);
        targetDirection.y = myCollider.bounds.center.y;
        return targetDirection;
    }

    /*
    *   CastTime - Stops the champion for the duration of the spells cast.
    *   @param castTime - float for the duration to stop the champion for casting.
    *   @param canMove - bool for whether or not the unit can move while casting.
    */
    protected IEnumerator CastTime(float castTime, bool canMove){
        //castCanMove = canMove;
        float timer = 0.0f;
        player.SetIsCasting(true, this);
        // While still casting spell stop the player.
        while(timer <= castTime){
            if(!canMove){
                if(!navMeshAgent.isStopped)
                    navMeshAgent.isStopped = true;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        player.SetIsCasting(false, this);
        navMeshAgent.isStopped = false;
    }

    /*
    *   Spell_Cd_Timer - Times the cooldown of a spell and sets it cd bool to false when its cooldown is complete.
    *   @param spell_cd - float representing the spells cooldown.
    *   @param myResult - Action<bool> method used for returning a value for setting the spell cooldowns onCd value back to false.
    */
    protected IEnumerator Spell_Cd_Timer(float spell_cd, string spell){
        spell_cd = CalculateCooldown(spell_cd, championStats.haste.GetValue());
        float spell_timer = 0.0f;
        //Debug.Log(spell_timer);
        while(spell_timer <= spell_cd){
            //Debug.Log(spell_timer);
            spell_timer += Time.deltaTime;
            UIManager.instance.UpdateCooldown(spell, spell_cd - spell_timer, spell_cd, player.playerUI);
            yield return null;
        }
        UIManager.instance.UpdateCooldown(spell, 0, spell_cd, player.playerUI);
        onCd = false;
    }

    /*
    *   CalculateCooldown - Calculates the cooldown of a spell after applying the champions haste value.
    *   @param baseCD - float of the base cooldown.
    *   @param haste - float of the haste value the champ has.
    */
    private float CalculateCooldown(float baseCD, float haste){
        float reducedCD = baseCD*(100f/(100f+haste));
        return Mathf.Round(reducedCD * 1000.0f) * 0.001f;
    }
}

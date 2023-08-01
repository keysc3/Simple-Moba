using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

/*
* Purpose: Implements a spell.
*
* @author: Colin Keys
*/
[System.Serializable]
public class Spell
{
    [SerializeField] public bool onCd { get; protected set; } = false;
    [SerializeField] public bool canMove { get; protected set; } = false;
    [SerializeField] public bool isQuickCast { get; protected set; } = false;
    [SerializeField] public bool isDisplayed { get; private set; } = false;
    [SerializeField] public string spellNum { get; private set; }
    protected NavMeshAgent navMeshAgent;
    protected LevelManager levelManager;
    protected ChampionStats championStats;
    protected Player player;
    protected ChampionSpells championSpells;
    protected GameObject gameObject;
    protected Collider myCollider;
    protected Camera mainCamera;
    
    /*
    *   Spell - Creates a spell object.
    *   @param championSpells - ChampionSpells instance this spell is a part of.
    *   @param spellNum - string of the spell number this spell is.
    */
    public Spell(ChampionSpells championSpells, string spellNum){
        this.championSpells = championSpells;
        this.spellNum = spellNum;
        this.gameObject = championSpells.gameObject;
        mainCamera = Camera.main;
        player = championSpells.gameObject.GetComponent<Player>();
        myCollider = championSpells.gameObject.GetComponent<Collider>();
        navMeshAgent = championSpells.gameObject.GetComponent<NavMeshAgent>();
        championStats = (ChampionStats) player.unitStats;
        levelManager = player.levelManager;
    }

    /*
    *   DisplayCast - Displays the spell by adding its DrawSpell method to the Debug drawing singleton.
    */
    public void DisplayCast(){
        if(!isDisplayed){
            DrawGizmos.instance.drawMethod += DrawSpell;
            isDisplayed = true;
        }
    }

    /*
    *   HideCast - Hides the spell by removing its DrawSpell method from the Debug drawing singleton.
    */
    public void HideCast(){
        if(isDisplayed){
            DrawGizmos.instance.drawMethod -= DrawSpell;
            isDisplayed = false;
        }
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected virtual void DrawSpell(){
        //PlaceHolder
    }

    /*
    *   GetTargetDirection - Gets the mouse world position.
    *   @return Vector3 - World position of the mouse.
    */
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
    */
    protected IEnumerator CastTime(float castTime, bool canMove){
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
        // While spell is still on CD
        while(spell_timer <= spell_cd){
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

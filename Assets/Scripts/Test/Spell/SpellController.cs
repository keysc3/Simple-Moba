using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController
{
    private ISpell spell;
    private IPlayer player;
    private Camera mainCamera;

    public SpellController(ISpell spell, IPlayer player){
        this.spell = spell;
        this.player = player;
        mainCamera = Camera.main;
    }

    /*
    *   GetTargetDirection - Gets the mouse world position.
    *   @return Vector3 - World position of the mouse.
    */
    public Vector3 GetTargetDirection(){
        RaycastHit hitInfo;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        LayerMask groundMask = LayerMask.GetMask("Ground");
        Physics.Raycast(ray, out hitInfo, Mathf.Infinity, groundMask);
        Vector3 targetDirection = hitInfo.point;
        player.MouseOnCast = targetDirection;
        targetDirection.y = player.myCollider.bounds.center.y;
        return targetDirection;
    }

    /*
    *   CastTime - Stops the champion for the duration of the spells cast.
    *   @param castTime - float for the duration to stop the champion for casting.
    */
    public IEnumerator CastTime(float castTime){
        float timer = 0.0f;
        player.IsCasting = true;
        player.CurrentCastedSpell = spell;
        // While still casting spell stop the player.
        while(timer <= castTime){
            if(!spell.CanMove){
                if(!player.navMeshAgent.isStopped)
                    player.navMeshAgent.isStopped = true;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        player.IsCasting = false;
        player.CurrentCastedSpell = spell;
        player.navMeshAgent.isStopped = false;
    }

    /*
    *   Spell_Cd_Timer - Times the cooldown of a spell and sets it cd bool to false when its cooldown is complete.
    *   @param spell_cd - float representing the spells cooldown.
    *   @param myResult - Action<bool> method used for returning a value for setting the spell cooldowns onCd value back to false.
    */
    public IEnumerator Spell_Cd_Timer(float spell_cd){
        spell_cd = CalculateCooldown(spell_cd, player.unitStats.haste.GetValue());
        float spell_timer = 0.0f;
        // While spell is still on CD
        while(spell_timer <= spell_cd){
            spell_timer += Time.deltaTime;
            //UIManager.instance.UpdateCooldown(spell, spell_cd - spell_timer, spell_cd, player.playerUI);
            yield return null;
        }
        //UIManager.instance.UpdateCooldown(spell, 0, spell_cd, player.playerUI);
        spell.OnCd = false;
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

    public void SpellCDChildrenSetActive(Transform parent, bool isActive){
        for(int i = 0; i < parent.childCount; i++){
            parent.GetChild(i).gameObject.SetActive(isActive);
        }
    }
}

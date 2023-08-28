using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

/*
*   Purpose: Handles a spells actions.
*
*   @author: Colin Keys
*/
public class SpellController
{
    private ISpell spell;
    private IPlayer player;
    private Camera mainCamera;
    private NavMeshAgent navMeshAgent;

    /*
    *   SpellController - Sets up new SpellController.
    *   @param spell - ISpell to use with methods.
    *   @param player - IPlayer to use with methods.
    */
    public SpellController(ISpell spell, IPlayer player){
        this.spell = spell;
        this.player = player;
        mainCamera = Camera.main;
        navMeshAgent = (spell as MonoBehaviour).gameObject.GetComponent<NavMeshAgent>();
    }

    /*
    *   GetTargetDirection - Gets the mouse world position.
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
                if(navMeshAgent != null){
                    if(!navMeshAgent.isStopped)
                        navMeshAgent.isStopped = true;
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
        player.IsCasting = false;
        player.CurrentCastedSpell = spell;
        if(navMeshAgent != null)
            navMeshAgent.isStopped = false;
    }

    /*
    *   Spell_Cd_Timer - Times the cooldown of a spell and sets it cd bool to false when its cooldown is complete.
    *   @param spell_cd - float representing the spells cooldown.
    */
    public IEnumerator Spell_Cd_Timer(float spell_cd){
        SpellCDChildrenSetActive(spell.spellCDTransform, true);
        spell_cd = CalculateCooldown(spell_cd);
        float spell_timer = 0.0f;
        // While spell is still on CD
        while(spell_timer <= spell_cd){
            spell_timer += Time.deltaTime;
            if(spell.spellCDTransform != null){
                // Update the UI cooldown text and slider.
                float cooldownLeft = spell_cd - spell_timer;
                spell.spellCDText.SetText(Mathf.Ceil(cooldownLeft).ToString());
                float fill = Mathf.Clamp(cooldownLeft/spell_cd, 0f, 1f);
                spell.spellCDImage.fillAmount = fill;
            }
            yield return null;
        }
        spell.OnCd = false;
        SpellCDChildrenSetActive(spell.spellCDTransform, false);
    }

    /*
    *   CalculateCooldown - Calculates the cooldown of a spell after applying the champions haste value.
    *   @param baseCD - float of the base cooldown.
    */
    private float CalculateCooldown(float baseCD){
        float reducedCD = baseCD*(100f/(100f+player.unitStats.haste.GetValue()));
        return Mathf.Round(reducedCD * 1000.0f) * 0.001f;
    }

    /*
    *   SpellCDChildrenSetActive - Sets the children of a transform as active or inactive based on given bool.
    *   @param parent - Transform of parent.
    *   @param isActive - bool of wether to set children active or inactive.
    */
    public void SpellCDChildrenSetActive(Transform parent, bool isActive){
        if(parent != null){
            for(int i = 0; i < parent.childCount; i++){
                parent.GetChild(i).gameObject.SetActive(isActive);
            }
        }
    }

    /*
    *   UpdateActiveSpellSlider - Updates a spells UI component representing an active duration.
    *   @param imageSlider - Image component to update.
    *   @param duration - float of the total duration.
    *   @param active - float of the active duration.
    */
    public void UpdateActiveSpellSlider(Image imageSlider, float duration, float active){
        if(imageSlider != null){
            // Get value between 0 and 1 representing the percent of the spell duration left.
            float fill = 1.0f - (active/duration);
            fill = Mathf.Clamp(fill, 0f, 1f);
            // Set the fill on the active spells slider.
            imageSlider.fillAmount = fill;
        }
    }
}

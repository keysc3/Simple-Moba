using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellController
{
    private ISpell spell;
    private IPlayer player;
    private Camera mainCamera;
    public Transform spellCDTransform;
    public TMP_Text spellCDText;
    public Image spellCDImage;

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
            if(spellCDTransform != null){
                // Update the UI cooldown text and slider.
                float cooldownLeft = spell_cd - spell_timer;
                spellCDText.SetText(Mathf.Ceil(cooldownLeft).ToString());
                float fill = Mathf.Clamp(cooldownLeft/spell_cd, 0f, 1f);
                spellCDImage.fillAmount = fill;
            }
            yield return null;
        }
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

    public void SpellCDChildrenSetActive(bool isActive){
        for(int i = 0; i < spellCDTransform.childCount; i++){
            spellCDTransform.GetChild(i).gameObject.SetActive(isActive);
        }
    }

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

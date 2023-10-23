using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
* Purpose: Adds and animates status effects for a players UI.
*
* @author: Colin Keys
*/
public class StatusEffectUI : MonoBehaviour
{
    public IPlayer player;
    public Effect effect;
    private StatusEffects statusEffects;
    private float buffDebuffUIWidth;
    private float xOffset = 2f;
    private Transform statusEffectsUI;

    // Called when the script instance is being loaded.
    private void Start(){
        if(player.playerUI != null){
            buffDebuffUIWidth = player.playerUI.transform.Find("Player/StatusEffects/BuffsContainer").GetComponent<RectTransform>().rect.width;
            statusEffectsUI = player.playerUI.transform.Find("Player/StatusEffects");
            statusEffects = player.statusEffects;
        }
        if(statusEffectsUI != null){
            AddStatusEffectUI();
        }
    }

    /*
    *   AddStatusEffectUI - Adds a status effect indicator to the UI. Left side is buffs, right side is debuffs.
    */
    // TODO: Handle truncation of effects when there are too many to fit the initial container.
    // Could set a max size per row, if number of children % max size per row  > 0 -> 
    // increase size of container, increase y offset.
    public void AddStatusEffectUI(){
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        gameObject.name = effect.effectType.name;
        gameObject.transform.Find("InnerContainer/Sprite").GetComponent<Image>().sprite = effect.effectType.sprite;
        if(effect.EffectDuration == -1f)
            gameObject.transform.Find("InnerContainer/Slider").gameObject.SetActive(false);
        // Set color and position of the UI element.
        if(effect.effectType.isBuff){
            gameObject.transform.Find("Background").GetComponent<Image>().color = Color.blue;
            SetStatusEffectUIPosition(statusEffectsUI.Find("BuffsContainer"), true);
        }
        else{
            gameObject.transform.Find("Background").GetComponent<Image>().color = Color.red;
            SetStatusEffectUIPosition(statusEffectsUI.Find("DebuffsContainer"), false);
        }
        // Start effect timer animation coroutine.
        if(effect.effectType.isStackable)
            // Use player incase GameObject is inactive.
            (player as MonoBehaviour).StartCoroutine(AnimateStackableStatusEffect());
        else
            (player as MonoBehaviour).StartCoroutine(AnimateStatusEffect());
    }

    /*
    *   SetStatusEffectUIPosition - Sets the position of the given status effect UI element.
    */
    private void SetStatusEffectUIPosition(Transform UI, bool isBuff){
        // Set up variables
        float effectWidth = gameObject.GetComponent<RectTransform>().rect.width;
        Vector2 offset = Vector2.zero;
        // Set parent.
        gameObject.transform.SetParent(UI);
        int index = gameObject.transform.GetSiblingIndex();
        // Calculate the position to put the new UI element in its parent.
        if(index > 0){
            // Offset new element based on last elements position.
            Vector2 prevPos = UI.GetChild(index-1).GetComponent<RectTransform>().anchoredPosition;
            Vector2 sizeOffset = new Vector2(effectWidth + xOffset, 0f);
            // Debuff is opposite from buff.
            if(!isBuff)
                sizeOffset = -(sizeOffset);
            offset = prevPos + sizeOffset;
        }
        else{
            // Calculate first elements position from center of the parent.
            offset = new Vector2(0f - (buffDebuffUIWidth/2f) + (effectWidth/2f), 0f);
            // Debuff is opposite from buff.
            if(!isBuff)
                offset = -(offset);
        }
        // Apply position.
        gameObject.GetComponent<RectTransform>().anchoredPosition = offset;
    }

    /*
    *   AnimateStatusEffect - Coroutine to animate the UI to show time left on a non-stackable effect.
    */
    private IEnumerator AnimateStatusEffect(){
        float elapsedDuration;
        // Get the timer image component.
        Image slider = transform.Find("InnerContainer/Slider").GetComponent<Image>();
        TMP_Text value = null;
        if(effect is PersonalSpell)
            value = transform.Find("InnerContainer/Value").GetComponent<TMP_Text>();
        // While the effect still exists on the GameObject.
        while(statusEffects.statusEffects.Contains(effect)){
            if(value != null){
                value.SetText(((PersonalSpell) effect).Stacks.ToString());
                if(effect.EffectDuration == -1f)
                    yield return null;
            }
            // Update status effect timer.
            elapsedDuration = 1f - effect.effectTimer/effect.EffectDuration;
            slider.fillAmount = elapsedDuration;
            yield return null;
        }
        // Update UI positions based on what position the ended effect was in.
        UpdateStatusEffectsPositions();
        // Remove UI component.
        Destroy(gameObject);
    }

    /*
    *   AnimateStackableStatusEffect - Coroutine to animate the UI to show time left on a stackable effect.
    */
    private IEnumerator AnimateStackableStatusEffect(){
        // Set stack text active.
        transform.Find("InnerContainer/Value").gameObject.SetActive(true);
        // Setup variables.
        Effect displayEffect = effect;
        int stacks = 0;
        float duration = 0f;
        float elapsedDuration;
        // Reduced amount is used when a stack expires.
        // The percentage of fill is calculated based on the duration left on the stack being displayed from the first frame is was displayed.
        // This allows its timer to animate from 100% -> 0% fill instead of starting at a percentage that isn't 100.
        // This is necessary for stacks that falloff over time instead of at the same time.
        float reduceAmount = 0f;
        // Get the timer image component.
        Image slider = transform.Find("InnerContainer/Slider").GetComponent<Image>();
        TMP_Text value = transform.Find("InnerContainer/Value").GetComponent<TMP_Text>();
        // While the effect still exists on the GameObject.
        while(statusEffects.statusEffects.Contains(effect)){
            // Get how many stacks the effect has.
            int newStacks = statusEffects.GetEffectsByName(effect.effectType.name).Count;
            // If stacks aren't equal then a stack expired or was added.
            if(stacks != newStacks){
                // Set the stacks text and get the next expiring stack to display.
                value.SetText(newStacks.ToString());
                displayEffect = statusEffects.GetNextExpiringStack(effect);
                // If a stack expired.
                if(newStacks < stacks){
                    // Get the duration left on the next expiring stack.
                    duration = displayEffect.EffectDuration - displayEffect.effectTimer;
                    // Get the stacks active time.
                    reduceAmount = displayEffect.effectTimer;
                }
                // If a new stack was added use the regular duration.
                else{
                    duration = displayEffect.EffectDuration;
                    reduceAmount = 0f;
                }
            }
            // Update status effect timer.
            // 1 - ((effectTimer - effectTimer at frame of first display)/duration left at frame of first display.
            elapsedDuration = 1f - ((displayEffect.effectTimer - reduceAmount)/duration);
            slider.fillAmount = elapsedDuration;
            stacks = newStacks;
            yield return null;
        }
        // Update UI positions based on what position the ended effect was in.
        UpdateStatusEffectsPositions();
        // Remove UI component.
        Destroy(gameObject);
    }

    /*
    *   UpdateStatusEffectsPositions - Moves the status effect UI components that were created after the one that ended.
    *   This is to prevent gaps between UI components.
    */
    private void UpdateStatusEffectsPositions(){
        // Get the UI components child index and its current position.
        int index = transform.GetSiblingIndex();
        Vector2 newPos = GetComponent<RectTransform>().anchoredPosition;
        Vector2 lastPos = Vector2.zero;
        Transform UI;
        // Get the appropriate UI container.
        if(effect.effectType.isBuff)
            UI = statusEffectsUI.Find("BuffsContainer");
        else
            UI = statusEffectsUI.Find("DebuffsContainer");
        // Update every status effect UI components position after the one being removed.
        for(int i = index + 1; i < UI.childCount; i++){
            // Store the position to move the next child to.
            lastPos = UI.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
            // Set the position to the previous components old position.
            UI.GetChild(i).GetComponent<RectTransform>().anchoredPosition = newPos;
            // Set where to move next UI component to.
            newPos = lastPos;
        }
    }
}

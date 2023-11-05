using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
* Purpose: Updates the players status effects section of the UI.
*
* @author: Colin Keys
*/
public class StatusEffectsUI : MonoBehaviour
{
    private StatusEffects statusEffects;
    private float buffDebuffUIWidth;
    private float xOffset = 2f;
    [SerializeField] private GameObject statusEffectPrefab;
    private Transform buffsContainer;
    private Transform debuffsContainer;
    private float effectWidth;
    private IPlayer player;
    

    // Called when the script instance is being loaded.
    private void Start(){
        player = GetComponentInParent<IPlayer>();
        statusEffects = player.statusEffects;
        statusEffects.EffectAdded += AddStatusEffectUI;
        buffsContainer = transform.Find("BuffsContainer");
        debuffsContainer = transform.Find("DebuffsContainer");
        buffDebuffUIWidth = buffsContainer.GetComponent<RectTransform>().rect.width;
        effectWidth = statusEffectPrefab.GetComponent<RectTransform>().rect.width;
        // Add any effects to UI that were initialized before this.
        if(statusEffects.statusEffects.Count > 0){
            foreach(Effect effect in statusEffects.statusEffects){
                AddStatusEffectUI(effect);
            }
        }
    }

    /*
    *   AddStatusEffectUI - Adds a status effect indicator to the UI. Left side is buffs, right side is debuffs.
    *   @param effect - Effect being added to the UI.
    */
    // TODO: Handle truncation of effects when there are too many to fit the initial container.
    // Could set a max size per row, if number of children % max size per row  > 0 -> 
    // increase size of container, increase y offset.
    public void AddStatusEffectUI(Effect effect){
        // If a stackable effect already has 1 stack, don't create a new UI element.
        if(effect.effectType.isStackable){
            if(statusEffects.GetEffectsByName(effect.effectType.name).Count > 1)
                return;
        }
        //Instantiate status effect prefab.
        Transform myEffect = (Instantiate(statusEffectPrefab, Vector3.zero, Quaternion.identity)).transform;
        myEffect.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        myEffect.name = effect.effectType.name;
        myEffect.Find("InnerContainer/Sprite").GetComponent<Image>().sprite = effect.effectType.sprite;
        if(effect.EffectDuration == -1f)
            myEffect.Find("InnerContainer/Slider").gameObject.SetActive(false);
        // Set color and position of the UI element.
        if(effect.effectType.isBuff){
            myEffect.Find("Background").GetComponent<Image>().color = Color.blue;
            myEffect.SetParent(buffsContainer);
            SetStatusEffectUIPosition(myEffect, true);
        }
        else{
            myEffect.Find("Background").GetComponent<Image>().color = Color.red;
            myEffect.SetParent(debuffsContainer);
            SetStatusEffectUIPosition(myEffect, false);
        }
        // Start effect timer animation coroutine.
        if(effect.effectType.isStackable)
            // Use player incase GameObject is inactive.
            (player as MonoBehaviour).StartCoroutine(AnimateStackableStatusEffect(effect, myEffect));
        else
            (player as MonoBehaviour).StartCoroutine(AnimateStatusEffect(effect, myEffect));
    }

    /*
    *   SetStatusEffectUIPosition - Sets the position of the given status effect UI element.
    *   @param myEffect - Transform of the effect UI object.
    *   @param isBuff - bool of wether the effect is a buff or not.
    */
    private void SetStatusEffectUIPosition(Transform myEffect, bool isBuff){
        // Set up variables
        Vector2 offset = Vector2.zero;
        // Set parent.
        //gameObject.transform.SetParent(UI);
        int index = myEffect.GetSiblingIndex();
        // Calculate the position to put the new UI element in its parent.
        if(index > 0){
            // Offset new element based on last elements position.
            Vector2 prevPos = myEffect.parent.GetChild(index-1).GetComponent<RectTransform>().anchoredPosition;
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
        myEffect.GetComponent<RectTransform>().anchoredPosition = offset;
    }

    /*
    *   AnimateStatusEffect - Coroutine to animate the UI to show time left on a non-stackable effect.
    *   @param effect - Effect being updated.
    *   @param myEffect - Transform of the effects UI object.
    */
    private IEnumerator AnimateStatusEffect(Effect effect, Transform myEffect){
        float elapsedDuration;
        // Get the timer image component.
        Image slider = myEffect.Find("InnerContainer/Slider").GetComponent<Image>();
        TMP_Text value = null;
        if(effect is PersonalSpell)
            value = myEffect.Find("InnerContainer/Value").GetComponent<TMP_Text>();
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
        UpdateStatusEffectsPositions(effect, myEffect);
        // Remove UI component.
        Destroy(myEffect.gameObject);
    }

    /*
    *   AnimateStackableStatusEffect - Coroutine to animate the UI to show time left on a stackable effect.
    *   @param effect - Effect being updated.
    *   @param myEffect - Transform of the effects UI object.
    */
    private IEnumerator AnimateStackableStatusEffect(Effect effect, Transform myEffect){
        // Set stack text active.
        myEffect.Find("InnerContainer/Value").gameObject.SetActive(true);
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
        Image slider = myEffect.Find("InnerContainer/Slider").GetComponent<Image>();
        TMP_Text value = myEffect.Find("InnerContainer/Value").GetComponent<TMP_Text>();
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
        UpdateStatusEffectsPositions(effect, myEffect);
        // Remove UI component.
        Destroy(myEffect.gameObject);
    }

    /*
    *   UpdateStatusEffectsPositions - Moves the status effect UI components that were created after the one that ended.
    *   This is to prevent gaps between UI components.
    *   @param effect - Effect being updated.
    *   @param myEffect - Transform of the effects UI object.
    */
    private void UpdateStatusEffectsPositions(Effect effect, Transform myEffect){
        // Get the UI components child index and its current position.
        int index = myEffect.GetSiblingIndex();
        Vector2 newPos = myEffect.GetComponent<RectTransform>().anchoredPosition;
        Vector2 lastPos = Vector2.zero;
        Transform UI = myEffect.parent;
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

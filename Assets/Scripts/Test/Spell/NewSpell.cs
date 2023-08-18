using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

public class NewSpell
{
    private Transform spellCDTransform;
    private TMP_Text spellCDText;
    private Image spellCDImage;

    private bool onCd = false;
    public bool OnCd { 
        get => onCd;
        set {
            onCd = value;
            if(spellCDTransform != null)
                if(value == true)
                    SpellCDChildrenSetActive(true);
                else
                    SpellCDChildrenSetActive(false);
        }
    }
    public bool canMove { get; protected set; } = false;
    public bool isQuickCast { get; protected set; } = false;
    public bool isDisplayed { get; private set; } = false;
    protected string spellNum;
    public string SpellNum { 
        get => spellNum;
        set {
            if(new List<string>(){"Passive", "Spell_1", "Spell_2", "Spell_3", "Spell_4"}.Contains(value)){
                spellNum = value;
                if(player != null && player.playerUI != null){
                    spellCDTransform = player.playerUI.transform.Find("Player/Combat/SpellsContainer/" + value + "_Container/SpellContainer/Spell/CD");
                    spellCDText = spellCDTransform.Find("Value").GetComponent<TMP_Text>();
                    spellCDImage = spellCDTransform.Find("Slider").GetComponent<Image>();
                }
            }
        }
    }
    public SpellData spellData { get; }
    protected ChampionStats championStats;
    protected NewChampionSpells championSpells;
    protected Camera mainCamera;
    protected IPlayer player;
    
    public delegate void SpellHitCallback(GameObject hit, Spell spellHit); 
    public SpellHitCallback spellHitCallback;

    /*
    *   Spell - Creates a spell object.
    *   @param championSpells - ChampionSpells instance this spell is a part of.
    *   @param spellNum - string of the spell number this spell is.
    */
    public NewSpell(NewChampionSpells championSpells, SpellData spellData, IPlayer player){
        this.championSpells = championSpells;
        this.spellData = spellData;
        mainCamera = Camera.main;
        championStats = (ChampionStats) player.unitStats;
        this.player = player;
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
    protected virtual void DrawSpell(){}

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
        championSpells.mouseOnCast = targetDirection;
        targetDirection.y = player.myCollider.bounds.center.y;
        return targetDirection;
    }

    /*
    *   CastTime - Stops the champion for the duration of the spells cast.
    *   @param castTime - float for the duration to stop the champion for casting.
    */
    protected IEnumerator CastTime(float castTime, bool canMove){
        float timer = 0.0f;
        championSpells.isCasting = true;
        championSpells.CurrentCastedSpell = this;
        // While still casting spell stop the player.
        while(timer <= castTime){
            if(!canMove){
                if(!player.navMeshAgent.isStopped)
                    player.navMeshAgent.isStopped = true;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        championSpells.isCasting = false;
        championSpells.CurrentCastedSpell = this;
        player.navMeshAgent.isStopped = false;
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
            if(spellCDTransform != null){
                // Update the UI cooldown text and slider.
                float cooldownLeft = spell_cd - spell_timer;
                spellCDText.SetText(Mathf.Ceil(cooldownLeft).ToString());
                float fill = Mathf.Clamp(cooldownLeft/spell_cd, 0f, 1f);
                spellCDImage.fillAmount = fill;
            }
            yield return null;
        }
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

    public void SpellCDChildrenSetActive(bool isActive){
        for(int i = 0; i < spellCDTransform.childCount; i++){
            spellCDTransform.GetChild(i).gameObject.SetActive(isActive);
        }
    }

    public virtual void SpellRemoved(){}
}

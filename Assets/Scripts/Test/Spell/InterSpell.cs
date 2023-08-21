using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InterSpell : MonoBehaviour, ISpell
{
    protected SpellController sc;
    
    public Transform spellCDTransform { get; set; }
    public TMP_Text spellCDText { get; set; }
    public Image spellCDImage { get; set; }

    private bool onCd = false;
    public bool OnCd { 
        get => onCd;
        set {
            onCd = value;
            if(spellCDTransform != null)
                if(value == true)
                    sc.SpellCDChildrenSetActive(spellCDTransform, true);
                else
                    sc.SpellCDChildrenSetActive(spellCDTransform, false);
        }
    }
    public bool CanMove { get; set; } = false;
    public bool IsQuickCast { get; set; } = false;
    public bool IsDisplayed { get; set; } = false;
    private string spellNum;
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
    public int SpellLevel { get => player.levelManager.spellLevels[SpellNum]-1; }
    public SpellData spellData { get; }
    protected ChampionStats championStats;
    protected Camera mainCamera;
    protected IPlayer player;

    protected virtual void Awake(){
        player = GetComponent<IPlayer>();
        championStats = (ChampionStats) player.unitStats;
        mainCamera = Camera.main;
        sc = new SpellController(this, player);
        SpellNum = spellData.defaultSpellNum;
    }

    /*
    *   DisplayCast - Displays the spell by adding its DrawSpell method to the Debug drawing singleton.
    */
    public void DisplayCast(){
        if(!IsDisplayed){
            DrawGizmos.instance.drawMethod += DrawSpell;
            IsDisplayed = true;
        }
    }

    /*
    *   HideCast - Hides the spell by removing its DrawSpell method from the Debug drawing singleton.
    */
    public void HideCast(){
        if(IsDisplayed){
            DrawGizmos.instance.drawMethod -= DrawSpell;
            IsDisplayed = false;
        }
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected virtual void DrawSpell(){}

    public virtual void SpellRemoved(){}
}

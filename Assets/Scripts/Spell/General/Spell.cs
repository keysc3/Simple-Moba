using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
*   Purpose: Implements a Spell class.
*
*   @author: Colin Keys
*/
public class Spell : MonoBehaviour, ISpell
{
    public Transform spellCDTransform { get; set; }
    public TMP_Text spellCDText { get; set; }
    public Image spellCDImage { get; set; }

    protected bool onCd = false;
    public bool OnCd { 
        get => onCd;
        set => onCd = value;
    }
    public bool CanMove { get; set; } = false;
    public bool IsQuickCast { get; set; } = false;
    public bool IsDisplayed { get; set; } = false;
    public bool IsSummonerSpell { get; set; } = false;
    private SpellType spellNum;
    public SpellType SpellNum { 
        get => spellNum;
        set {
            spellNum = value;
            if(player != null && player.playerUI != null){
                spellCDTransform = player.playerUI.transform.Find("Player/Combat/SpellsContainer/" + value.ToString() + "_Container/SpellContainer/Spell/CD");
                spellCDText = spellCDTransform.Find("Value").GetComponent<TMP_Text>();
                spellCDImage = spellCDTransform.Find("Slider").GetComponent<Image>();
            }
        }
    }
    public int SpellLevel { get => player.levelManager.spellLevels[SpellNum]-1; }
    [field: SerializeField] public SpellData spellData { get; set; }
    protected ChampionStats championStats;
    protected Camera mainCamera;
    protected IPlayer player;
    public SpellController spellController { get; private set; }
    protected Collider myCollider;

    // Called when the script instance is being loaded.
    protected virtual void Awake(){
        player = GetComponent<IPlayer>();
        mainCamera = Camera.main;
        spellController = new SpellController(this, player);
        myCollider = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    protected virtual void Start(){
        //spellController = new SpellController(this, player);
        championStats = (ChampionStats) player.unitStats;
        if(spellNum == SpellType.None)
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
}

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
        set => spellNum = value;
    }
    public int SpellLevel { get => player.levelManager.spellLevels[SpellNum]-1; }
    [field: SerializeField] public SpellData spellData { get; set; }
    protected ChampionStats championStats;
    protected Camera mainCamera;
    protected IPlayer player;
    public SpellController spellController { get; private set; }
    protected Collider myCollider;

    public delegate void SpellCDSetActive(SpellType spellType, bool isActive);
    public event SpellCDSetActive SpellCDSetActiveCallback;

    public delegate void SpellSliderUpdate(SpellType spellType, float duration, float active);
    public event SpellSliderUpdate SpellSliderUpdateCallback;

    public delegate void SetComponentActive(SpellType spellType, SpellComponent component, bool isActive);
    public event SetComponentActive SetComponentActiveCallback;

    public delegate void SetSprite(SpellType spellType, SpellComponent component, Sprite sprite);
    public event SetSprite SetSpriteCallback;

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

    /*
    *   RaiseSpellCDSetActiveEvent - Raises the spell cd active event.
    *   @param spellType - SpellType for which UI element to adjust.
    *   @param isActive - bool whether to active UI or not.
    */
    public void RaiseSpellCDSetActiveEvent(SpellType spellType, bool isActive){
        SpellCDSetActiveCallback?.Invoke(spellType, isActive);
    }

    /*
    *   RaiseSpellSliderUpdateEvent - Raises the spell slider update event.
    *   @param spellType - SpellType for which UI element to adjust.
    *   @param isActive - bool whether to active UI or not.
    */
    public void RaiseSpellSliderUpdateEvent(SpellType spellType, float duration, float active){
        SpellSliderUpdateCallback?.Invoke(spellType, duration, active);
    }

    /*
    *   RaiseSetComponentActiveEvent - Raises the set active event.
    *   @param spellType - SpellType for which UI element to adjust.
    *   @param component - SpellComponent of the component to set active.
    *   @param isActive - bool whether to active UI or not.
    */
    public void RaiseSetComponentActiveEvent(SpellType spellType, SpellComponent component, bool isActive){
        SetComponentActiveCallback?.Invoke(spellType, component, isActive);
    }

    /*
    *   RaiseSetSpriteEvent - Raises the set sprite event.
    *   @param spellType - SpellType for which UI element to adjust.
    *   @param component - SpellComponent of the component to set active.
    *   @param sprite - Sprite to set the image to.
    */
    public void RaiseSetSpriteEvent(SpellType spellType, SpellComponent component, Sprite sprite){
        SetSpriteCallback?.Invoke(spellType, component, sprite);
    }
}

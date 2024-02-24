using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    protected LayerMask hitboxMask;
    protected List<RectTransform> spellUIDisplay = new List<RectTransform>();
    protected RectTransform canvas; 

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
        hitboxMask = LayerMask.GetMask("Hitbox");
        canvas = transform.Find("DrawSpell").GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    protected virtual void Start(){
        //spellController = new SpellController(this, player);
        championStats = (ChampionStats) player.unitStats;
        if(spellNum == SpellType.None)
            SpellNum = spellData.defaultSpellNum;
        foreach(GameObject obj in spellData.drawSpellImages){
            CreateSpellUIObject(obj);
        }
    }

    protected void CreateSpellUIObject(GameObject create){
        GameObject UIObject = (GameObject) Instantiate(create, Vector3.zero, Quaternion.identity);
        //canvas = transform.Find("DrawSpell");
        UIObject.transform.SetParent(canvas, false);
        UIObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
        UIObject.SetActive(false);
        spellUIDisplay.Add(UIObject.GetComponent<RectTransform>());
        //return UIObject;
    }

    protected void DrawSpellUIHitbox(int objectNum, float offset, Vector2 size, bool lookAt){
        spellUIDisplay[objectNum].gameObject.SetActive(true);
        if(lookAt){
            Vector3 direction = transform.position + (spellController.GetTargetDirection() - transform.position).normalized;
            direction.y = transform.position.y + canvas.anchoredPosition3D.y;
            canvas.LookAt(direction);
        }
        RectTransform rect = spellUIDisplay[objectNum];
        rect.sizeDelta = size;
        rect.anchoredPosition3D = new Vector3(0f, 0f, offset);
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
            foreach(RectTransform myObj in spellUIDisplay){
                myObj.gameObject.SetActive(false);
            }
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

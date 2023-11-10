using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{   
    private Gradient gradient;

    void Awake(){
        SetupGradient();
    }

    // Start is called before the first frame update
    void Start()
    {
        IPlayer player = GetComponentInParent<IPlayer>();
        player.levelManager.SkillPointsCallback += SetSkillLevelUpActive;
        player.levelManager.SpellLevelUpCallback += SpellLearned;
        player.levelManager.SkillPointAvailableCallback += SkillLevelUpGradient;
    }

    /*
    *   SetSkillLevelUpActive - Activates or deactivates spell level up buttons.
    *   @param levelManager - LevelManager of the relative player.
    */
    public void SetSkillLevelUpActive(LevelManager levelManager){
        // For each spell.
        foreach(KeyValuePair<SpellType, int> spell in levelManager.spellLevels){
            string find = spell.Key.ToString() + "_Container";
            GameObject spellLevelUpObj = transform.Find(find + "/LevelUp").gameObject;
            // If the UI should be active.
            if(levelManager.SpellLevelPoints > 0){
                // If the kvp is spell 4.
                if(spell.Key == SpellType.Spell4){
                    int spell_4_level = levelManager.spellLevels[SpellType.Spell4];
                    // If spell 4 can be leveled.
                    if((spell_4_level < 1 && levelManager.Level > 5) || (spell_4_level < 2 && levelManager.Level > 10) || (spell_4_level < 3 && levelManager.Level > 15)){
                        spellLevelUpObj.SetActive(true);
                    }
                    else
                        spellLevelUpObj.SetActive(false);
                }
                // If a basic spell then activate its level up available UI if it isn't max spell level.
                else{
                    if(spell.Value < 5){
                        spellLevelUpObj.SetActive(true);  
                    }
                    else{
                        spellLevelUpObj.SetActive(false);
                    }
                }
            }
            else{
                spellLevelUpObj.SetActive(false);  
            }
        }
    }

    /*
    *   SpellLearned - Removes the spell cover the first time a spell is leveled and fills in the new level.
    *   @param spell - SpellType being leveled.
    *   @param level - int of the spells level.
    */
    private void SpellLearned(SpellType spell, int level){
        if(level == 1){
            Transform spellCover = transform.Find(spell + "_Container/SpellContainer/Spell/CD/Cover");
            spellCover.gameObject.SetActive(false);
        }
        Transform spellLevels = transform.Find(spell + "_Container/Levels");
        spellLevels.Find("Level" + level + "/Fill").gameObject.SetActive(false);
    }

    /*
    *   SkillLevelUpGradient - Animates the spell level up buttons.
    *   @param currentTime - float of the current game time.
    */
    public void SkillLevelUpGradient(float currentTime){
        // For each spell.
        for(int i = 0; i < 4; i++){
            transform.Find("Spell" + (i+1) + "_Container/LevelUp/Background")
            .gameObject.GetComponent<Image>().color = gradient.Evaluate(Mathf.PingPong(currentTime, 1));
        }
    }

    /*
    *   SetupGradient - Creates a new Gradient object to use for animating the spell level up buttons.
    */
    private void SetupGradient(){
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;
        Color defaultBorderColor = new Color(167f/255f, 126f/255f, 69f/255f);
        gradient = new Gradient();
        // Two color gradient.
        colorKey = new GradientColorKey[2];
        // Set colors and set their time to opposite ends.
        colorKey[0].color = new Color(167f/255f, 126f/255f, 69f/255f);
        colorKey[0].time = 0.0f;
        colorKey[1].color = new Color(230f/255f, 219f/255f, 204f/255f);
        colorKey[1].time = 1.0f;
        // One alpha gradient.
        alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        // Set the gradient.
        gradient.SetKeys(colorKey, alphaKey);
    }

}

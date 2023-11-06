using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{   
    // Start is called before the first frame update
    void Start()
    {
        IPlayer player = GetComponentInParent<IPlayer>();
        player.levelManager.SkillPointsCallback += SetSkillLevelUpActive;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
* Purpose: Updates the players portrait section of the UI.
*
* @author: Colin Keys
*/
public class UpdatePortraitUI : MonoBehaviour
{
    private TMP_Text levelText;
    private Slider xpSlider;
    private GameObject iconCover;
    private TMP_Text respawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        SetupCallback();
        Transform levelTransform = transform.Find("IconContainer/Level/Value");
        xpSlider = transform.Find("Experience").GetComponent<Slider>();
        iconCover = transform.Find("IconContainer/IconCover").gameObject;
        respawnTimer = iconCover.transform.Find("RespawnTimer").GetComponent<TMP_Text>();
        if(levelTransform != null)
            levelText = levelTransform.GetComponent<TMP_Text>(); 
    }

    /*
    *   SetupCallback - Adds the UI updating methods to the necessary events.
    */
    private void SetupCallback(){
        Player player = GetComponentInParent<Player>();
        if(player != null){
            player.levelManager.UpdateLevelCallback += UpdateLevelUI;
            player.levelManager.UpdateExperienceCallback += UpdateExperienceUI;
            player.UpdateRespawnTimerCallback += UpdateRespawnTimerUI;
        }
    }

    /*
    *   UpdateLevelUI - Updates the players level text.
    *   @param level - int of the level to display.
    */
    private void UpdateLevelUI(int level){
        levelText.SetText(level.ToString());
    }

    /*
    *   UpdateExperienceUI - Updates the experience bar with current values.
    *   @param currentXP - float of the players current experience.
    *   @param requiredXP - float of the required experience for the next level.
    */
    private void UpdateExperienceUI(float currentXP, float requiredXP){
        xpSlider.value = Mathf.Round((currentXP/requiredXP) * 100f);
    }

    /*
    *   UpdateRespawnTimerUI - Updates the portraits respawn timer.
    *   @param timer - float of the dead duration.
    *   @param respawn - float of the total time to respawn.
    */
    private void UpdateRespawnTimerUI(float timer, float respawn){
        float timeLeft = Mathf.Ceil(respawn - timer);
        respawnTimer.SetText(timeLeft.ToString());
        if(timeLeft > 0){
            if(!iconCover.activeSelf)
                iconCover.SetActive(true);
        }
        else
            iconCover.SetActive(false);
    }
}

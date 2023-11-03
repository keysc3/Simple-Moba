using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private void SetupCallback(){
        Player player = GetComponentInParent<Player>();
        if(player != null){
            player.levelManager.UpdateLevelCallback += UpdateLevelUI;
            player.levelManager.UpdateExperienceCallback += UpdateExperienceUI;
            player.UpdateRespawnTimerCallback += UpdateRespawnTimerUI;
        }
    }

    private void UpdateLevelUI(int level){
        levelText.SetText(level.ToString());
    }

    private void UpdateExperienceUI(float currentXP, float requiredXP){
        xpSlider.value = Mathf.Round((currentXP/requiredXP) * 100f);
    }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdatePortraitUI : MonoBehaviour
{
    private TMP_Text levelText;
    private Slider xpSlider;

    // Start is called before the first frame update
    void Start()
    {
        SetupCallback();
        Transform levelTransform = transform.Find("IconContainer/Level/Value");
        xpSlider = transform.Find("Experience").GetComponent<Slider>();
        if(levelTransform != null)
            levelText = levelTransform.GetComponent<TMP_Text>(); 
    }

    private void SetupCallback(){
        IPlayer player = GetComponentInParent<IPlayer>();
        if(player != null){
            player.levelManager.UpdateLevelCallback += UpdateLevelUI;
            player.levelManager.UpdateExperienceCallback += UpdateExperienceUI;
        }
    }

    private void UpdateLevelUI(int level){
        levelText.SetText(level.ToString());
    }

    private void UpdateExperienceUI(float currentXP, float requiredXP){
        xpSlider.value = Mathf.Round((currentXP/requiredXP) * 100f);
    }
}

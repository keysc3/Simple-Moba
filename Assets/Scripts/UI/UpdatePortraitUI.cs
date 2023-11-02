using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdatePortraitUI : MonoBehaviour
{
    private TMP_Text levelText;

    // Start is called before the first frame update
    void Start()
    {
        SetupCallback();
        Transform levelTransform = transform.Find("IconContainer/Level/Value");
        if(levelTransform != null)
            levelText = levelTransform.GetComponent<TMP_Text>(); 
    }

    private void SetupCallback(){
        IPlayer player = GetComponentInParent<IPlayer>();
        if(player != null){
            player.levelManager.UpdateLevelCallback += UpdateLevelUI;
        }
    }

    private void UpdateLevelUI(int level){
        levelText.SetText(level.ToString());
    }
}

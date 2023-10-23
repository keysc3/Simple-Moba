using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
* Purpose: Updates the UI to display a units health on their bar.
*
* @author: Colin Keys
*/
public class UpdateHealthBarUI : MonoBehaviour
{
    private IUnit unit; 
    private Slider health;

    // Called when the script instance is being loaded.
    private void Awake(){
        health = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    private void Start(){
        unit = GetComponentInParent<IUnit>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(unit != null){
            // If the unit is not dead.
            if(!unit.IsDead){
                // Get the health percent the unit is at and set the health bar text to currenthp/maxhp.
                float healthPercent = Mathf.Clamp(unit.unitStats.CurrentHealth/unit.unitStats.maxHealth.GetValue(), 0f, 1f);
                // Set the fill based on units health percent.
                health.value = healthPercent;
            }
            else{
                if(unit is IPlayer){
                    // Set players health text and fill to 0.
                    ((IPlayer) unit).playerBar.SetActive(false);
                }
            }
        }
    } 
}

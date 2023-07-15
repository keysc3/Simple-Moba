using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
* Purpose: Animates the drowsy timer on a GameObject.
*
* @author: Colin Keys
*/
public class BilliaDrowsyVisual : MonoBehaviour
{

    private Image drowsyVisual;
    private RectTransform rectTransform;
    private StatusEffects statusEffects;
    public float drowsyDuration;
    public GameObject source;
    public float yOffset = 1f;
    public ScriptableDrowsy drowsy;

    // Start is called before the first frame update
    void Start()
    {
        drowsyVisual = transform.GetChild(0).GetComponent<Image>();
        statusEffects = transform.parent.GetComponent<Unit>().statusEffects;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition3D = new Vector3(0f, (-(transform.parent.GetComponent<Collider>().bounds.size.y/2f)) + yOffset, 0f);
        rectTransform.rotation = Quaternion.Euler(90f, 0f , 0f);
        StartCoroutine(AnimateDrowsy());
    }

    /*
    *   AnimateDrowsy - Changes the fill of the drowsy visual to animate when it will be finished.
    */
    private IEnumerator AnimateDrowsy(){
        float timer = 0.0f;
        while(statusEffects.CheckForEffectWithSource(drowsy, source)){
            // Change the fill amount based on percentage of drowsy time passed.
            float fill = timer/drowsyDuration;
            drowsyVisual.fillAmount = fill;
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}

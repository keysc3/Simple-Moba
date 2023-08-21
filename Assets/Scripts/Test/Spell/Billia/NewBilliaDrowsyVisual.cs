using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBilliaDrowsyVisual : MonoBehaviour
{
    public float drowsyDuration;
    public ScriptableDrowsy drowsy;
    public GameObject source;
    private Image drowsyVisual;
    private RectTransform rectTransform;
    private NewStatusEffects statusEffects;
    private float yOffset = 1f;

    // Start is called before the first frame update
    private void Start()
    {
        drowsyVisual = transform.GetChild(0).GetComponent<Image>();
        statusEffects = transform.parent.GetComponent<IUnit>().statusEffects;
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

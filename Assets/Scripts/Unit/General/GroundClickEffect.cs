using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundClickEffect : MonoBehaviour
{
    public GameObject clickEffect;
    public static GroundClickEffect instance { get; private set; }
    public float animationTime = 0.5f;
    public float startingScale = 1.0f;

    private void Awake(){
        if(instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void CreateGroundClick(Vector3 position){
        GameObject click = (GameObject) Instantiate(clickEffect, position + new Vector3(0f, 0.01f, 0f), Quaternion.identity);
        StartCoroutine(AnimateClick(click.transform.GetChild(0).GetComponent<RectTransform>()));
    }

    public IEnumerator AnimateClick(RectTransform click){
        float timer = 0.0f;
        while(timer < animationTime){
            float percent = Mathf.Clamp01(timer/animationTime);
            //float newScale = Mathf.Lerp(startingScale, 0f, timer);
            float newScale = (1f - percent) * startingScale;
            click.localScale = Vector3.one * newScale;
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(click.transform.parent.gameObject);
    }
}

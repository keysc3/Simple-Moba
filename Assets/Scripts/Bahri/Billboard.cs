using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a billboard effect for a canvas object.
* @author: Colin Keys
*/
public class Billboard : MonoBehaviour
{

    private Camera mainCamera;

    private void Awake(){
        mainCamera = Camera.main;
    }

    private void LateUpdate(){
        transform.LookAt((transform.position + mainCamera.transform.forward) + new Vector3(0.0f, -0.1f, 0.0f));
        /*if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] != transform.parent.gameObject){
            Vector3 activeChampPos = ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion].transform.position;
            Vector3 vect3 = GetComponent<RectTransform>().anchoredPosition3D;
            GetComponent<RectTransform>().anchoredPosition3D =  new Vector3(((activeChampPos.x-transform.parent.position.x)*0.1f), vect3.y, ((activeChampPos.z-transform.parent.position.z)*0.1f));
        }*/
    }
}

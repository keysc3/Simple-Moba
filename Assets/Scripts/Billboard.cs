using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a billboard effect for a canvas object.
*
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
    }
}

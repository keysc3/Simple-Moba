using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
* Purpose: Handles drawing debug information.
*
* @author: Colin Keys
*/
public class DrawGizmos : MonoBehaviour
{
    public static DrawGizmos instance { get; private set; }

    public delegate void DrawMethod();
    public DrawMethod drawMethod;

    // Called when the script instance is being loaded.
    void Awake(){
        instance = this;
    }

    /*
    *   OnDrawGizmos - Draws gizmos for debugging.
    */
    private void OnDrawGizmos(){
        drawMethod?.Invoke();
    }
}

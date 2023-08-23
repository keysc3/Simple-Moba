using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInter : MonoBehaviour, ITester
{
    public float value1 { get; set; } = 5f;
    public Testing testing { get; set; }
    //public float value1 { get; set; } = 2f;
    public void Damage(){
        Debug.Log("ICHILD");
    }
}

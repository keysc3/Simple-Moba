using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChild : MonoBehaviour
{
    public GameObject obj;
    private ITester inter;
    void Awake(){
         inter = obj.GetComponent<ITester>();
         //inter.testing += Test1;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Destroy((inter as MonoBehaviour));
        //Debug.Log(inter.value1);
        //inter.testing -= Test2;
        /*IChild myChild = obj.GetComponent<IChild>();
        if(myChild is ITester){
            Debug.Log("MHM: " + ((ITester) myChild).value1);
        }
        else{
            Debug.Log("NO tester");
        }
        if(myChild is IBase){
            Debug.Log("MHM: " + ((IBase) myChild).value);
        }
        else{
            Debug.Log("NO base");
        }*/
    }

    public void Test1(){}

    public void Test2(){}
}

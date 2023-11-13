using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LockY : MonoBehaviour
{
    private float yPosition;
    //private NavMeshAgent navMeshAgent;
    //int layerMask = 1 << 6;
    private Collider myCollider;

    // Start is called before the first frame update
    void Start()
    {
        //navMeshAgent = GetComponentInParent<NavMeshAgent>();
        yPosition = GameController.instance.collisionPlane;
        //myCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*if(navMeshAgent == null || !navMeshAgent.enabled){
            RaycastHit hit;
            if(Physics.Raycast(transform.parent.position, Vector3.down, out hit, Mathf.Infinity, layerMask)){
                transform.position = new Vector3(hit.point.x, (hit.point.y + myCollider.bounds.size.y/2), hit.point.z);
            }
        }
        else
            transform.localPosition = Vector3.zero;*/
        transform.position = new Vector3(transform.parent.position.x, yPosition, transform.parent.position.z);
    }
}

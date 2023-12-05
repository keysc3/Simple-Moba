using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
*   Purpose: Handles receiving input for players movement and non-spells.
*
*   @author: Colin Keys
*/
public class PlayerControllerBehaviour : MonoBehaviour, IPlayerMover
{
    public IUnit TargetedEnemy { get; set; }
    public GameObject PlayerObj { get => gameObject; }
    public Vector3 Destination { 
        get => navMeshAgent.destination; 
        set => navMeshAgent.destination = value;
    }
    private Vector3 currentTarget;
    public Vector3 CurrentTarget { 
        get => currentTarget; 
        set {
            currentTarget = value;
            currentTarget.y = transform.position.y;
            transform.LookAt(currentTarget);
        }
    }
    public Vector3 NextDestination { 
        get {
            if(IsMoving)
                return navMeshAgent.steeringTarget;
            else
                return (transform.position + transform.forward);
        }
    }
    public bool IsMoving { 
        get {
            if(navMeshAgent.enabled){
                if(navMeshAgent.hasPath && !navMeshAgent.isStopped)
                    return true;
            }
            return false;
        }
    }

    private PlayerController playerController;
    private NavMeshAgent navMeshAgent;
    private PlayerSpells playerSpells;
    private Collider myCollider;

    // Called when the script instance is being loaded.
    private void Awake(){
        myCollider = GetComponent<Collider>();
        playerSpells = GetComponent<PlayerSpells>();
        playerController = new PlayerController(this, GetComponent<IPlayer>());
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update.
    private void Start()
    {
        navMeshAgent.updateRotation = false;
    }

    // Update is called once per frame.
    private void Update()
    {
        if(navMeshAgent.enabled){
            // Set the players destination.
            if(Input.GetMouseButtonDown(1)){
                // Ray cast from mouse position.
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = GetRaycastHits(ray);
                // If any hits get first hit.
                if(hits.Length > 0){
                    if(!(hits.Length == 1 && (hits[0].collider.gameObject == gameObject || hits[0].collider.transform.name == "Hitbox"))){
                        RaycastHit firstHit = GetFirstHit(hits);
                        playerController.RightClick(firstHit.transform.GetComponent<IUnit>(), firstHit.point);
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.S))
                playerController.StopPlayer();
        }
        // Point players forward at the direction they are cast or moving.
        playerController.PlayerLookDirection();
        // Set target destination.
        playerController.SetPlayerDestinationUsingTarget();
    }

    /*
    *   GetFirstHit - Return the first hit ray from a list of RaycastHits.
    *   @param hits - List of RayCastHits to iterate over.
    *   @return RaycastHit - First ray hit.
    */
    private RaycastHit GetFirstHit(RaycastHit[] hits){
        RaycastHit firstHit = hits[0];
        for(int i = 1; i < hits.Length; i++){
            RaycastHit hit = hits[i];
            if(hit.collider.gameObject != gameObject && hit.collider.transform.name != "Hitbox"){
                if(hit.distance < firstHit.distance || firstHit.collider.gameObject == gameObject){
                    firstHit = hit;
                }
            }
        }
        return firstHit;
    }

    /*
    *   GetRayCastHits - Returns a list of RaycastHits from a ray from the camera to the player plus 50f.
    *   @param ray - Ray to cast with.
    *   @return RaycastHit[] - List of of RaycastHits from the Raycast.
    */
    private RaycastHit[] GetRaycastHits(Ray ray){
        return Physics.RaycastAll(ray, (Camera.main.transform.position - transform.position).magnitude + 50f);
    }
}

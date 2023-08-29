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
            if(myCollider != null)
                currentTarget.y = myCollider.bounds.center.y;
            transform.LookAt(currentTarget);
        }
    }
    public Vector3 NextDestination { 
        get {
            if(navMeshAgent.hasPath)
                return navMeshAgent.steeringTarget;
            else
                return transform.position;
        }
    }

    private Camera mainCamera;
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
        mainCamera = Camera.main;
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
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if(Physics.Raycast(ray, out hitInfo)){
                    playerController.RightClick(hitInfo.transform.GetComponent<IUnit>(), hitInfo.point);
                }
            }

            // Stop player if they are at their destination or stop input received. 
            if(navMeshAgent.hasPath){
                if(Input.GetKeyDown(KeyCode.S))
                    playerController.StopPlayer();
            }
        }
        // Point players forward at the direction they are cast or moving.
        playerController.PlayerLookDirection();
        // Move player towards target.
        playerController.MovePlayerToEnemy();
    }
}

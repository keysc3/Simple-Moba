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
    private GameObject targetedEnemy;
    public GameObject TargetedEnemy { 
        get => targetedEnemy;
        set{
            if(value == null)
                navMeshAgent.ResetPath();
            else
                targetedEnemy = value;
        }
    }
    private Camera mainCamera;
    public Vector3 Destination { 
        get => navMeshAgent.destination; 
        set => navMeshAgent.destination = value;
    }
    private Vector3 currentTarget;
    public Vector3 CurrentTarget { 
        get => currentTarget; 
        set {
            currentTarget = value;
            if(player.myCollider != null)
                currentTarget.y = player.myCollider.bounds.center.y;
            transform.LookAt(currentTarget);
        }
    }
    public Vector3 Position { get => transform.position; set => transform.position = value; }
    public float Range { get => player.unitStats.autoRange.GetValue(); }

    private PlayerController playerController;
    private NavMeshAgent navMeshAgent;
    private PlayerSpells playerSpells;
    private IPlayer player;

    // Called when the script instance is being loaded.
    private void Awake(){
        playerSpells = GetComponent<PlayerSpells>();
        playerController = new PlayerController(this);
        navMeshAgent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
        player = GetComponent<IPlayer>();
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
                        playerController.RightClick(hitInfo, gameObject);
                    }
            }

            // Stop player if they are at their destination or stop input received. 
            if(navMeshAgent.hasPath){
                if(transform.position == navMeshAgent.destination || Input.GetKeyDown(KeyCode.S)){
                    TargetedEnemy = null;
                    navMeshAgent.ResetPath();
                }
            }
        }

        // Point players forward at the direction they are cast or moving.
        // The player should never be casting something if currentspell is null but just to be safe check for null.
        if(player.IsCasting && player.CurrentCastedSpell != null && player.CurrentCastedSpell.CanMove)
            playerController.PlayerLookDirection(player.MouseOnCast);
        else if(navMeshAgent.hasPath)
            playerController.PlayerLookDirection(navMeshAgent.steeringTarget);

        if(TargetedEnemy != null && !player.IsCasting){
            if(!TargetedEnemy.GetComponent<IUnit>().IsDead)
                playerController.MovePlayerToEnemy();
            else
                TargetedEnemy = null;
        }
    }
}

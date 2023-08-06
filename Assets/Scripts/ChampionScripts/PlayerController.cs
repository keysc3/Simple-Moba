using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements movement and targeting for a champion.
*
* @author: Colin Keys
*/
public class PlayerController : MonoBehaviour
{
    public GameObject targetedEnemy { get; private set; } = null;
    private float attackTime;
    private Camera mainCamera;
    private ChampionStats championStats;
    private RaycastHit hitInfo;
    private NavMeshAgent navMeshAgent;
    private Collider myCollider;
    private Vector3 dest;
    private Vector3 currentTarget;
    private BasicAttack basicAttack;
    private Player player;

    // Called when the script instance is being loaded.
    private void Awake(){
        navMeshAgent = GetComponent<NavMeshAgent>();
        myCollider = GetComponent<Collider>();
        basicAttack = GetComponent<BasicAttack>();
        mainCamera = Camera.main;
        player = GetComponent<Player>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        championStats = (ChampionStats) player.unitStats;
        navMeshAgent.updateRotation = false;
        navMeshAgent.speed = championStats.speed.GetValue();
        attackTime = 1.0f/championStats.attackSpeed.GetValue();

    }

    // Called when the script is enabled.
    private void OnEnable(){
        navMeshAgent.ResetPath();
    }

    // Update is called once per frame
    private void Update()
    {
        // Stop the players attack windup if casting or moving.
        if(player.isCasting || navMeshAgent.hasPath){
            basicAttack.windingUp = false;
        }

        if(navMeshAgent.enabled){
            // Set the players destination.
            if(Input.GetMouseButtonDown(1)){
                RightClick();
            }

            // Stop the players movement.
            if(Input.GetKeyDown(KeyCode.S)){
                navMeshAgent.ResetPath();
            }

            // Stop player if they are at their destination.
            if(navMeshAgent.hasPath && !player.isCasting){
                if(transform.position == navMeshAgent.destination)
                    navMeshAgent.ResetPath();
            }
        }

        // Point players forward at the direction they are cast or moving.
        // The player should never be casting something but just to be safe check for null.
        if(player.isCasting && player.CurrentCastedSpell != null && !player.CurrentCastedSpell.canMove)
            PlayerLookDirection(player.mouseOnCast);
        else if(navMeshAgent.hasPath)
            PlayerLookDirection(navMeshAgent.steeringTarget);

        if(targetedEnemy != null && !player.isCasting){
            if(!targetedEnemy.GetComponent<Unit>().isDead)
                MovePlayerToEnemy();
            else
                targetedEnemy = null;
        }
        else
            basicAttack.windingUp = false;
    }

    /*
    *   RightClick - Moves the player to the click position if no enemy is targeted, otherwise sets the target.
    */
    private void RightClick(){
        // Get the location on the navmesh to move the player to.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hitInfo)){
            dest = hitInfo.point;
            dest.y = myCollider.bounds.center.y;
            // If the player clicked an enemy set the target, otherwise set the destination.
            if(hitInfo.collider.tag == "Enemy" && hitInfo.collider.gameObject != gameObject && !hitInfo.collider.gameObject.GetComponent<Unit>().isDead){
                targetedEnemy = hitInfo.collider.gameObject;
            }
            else{
                targetedEnemy = null;
                navMeshAgent.destination = dest;
            }
        }
    }

    /*
    *   PlayerLookDirection - Turns the players transforms forward direction in the direction they are moving.
    *   @param nextTarget - Vector3 of the direction to look at.
    */
    private void PlayerLookDirection(Vector3 nextTarget){
        if(currentTarget != nextTarget){
            nextTarget.y = myCollider.bounds.center.y;
            transform.LookAt(nextTarget);
            currentTarget = nextTarget;
        }
    }

    /*
    *   MovePlayerToEnemy - Moves the player into range of their targeted enemy whenever they have one.
    */
    private void MovePlayerToEnemy(){
        // Get the targets distance from the player.
        Vector3 myTarget = targetedEnemy.transform.position;
        myTarget.y = 0.0f;
        float distToEnemy = (transform.position - myTarget).magnitude;
        // If the enemy is in auto range then start autoing.
        if(distToEnemy < championStats.autoRange.GetValue()){
            // Stop navmesh
            navMeshAgent.ResetPath();
            // If the time since last auto is greater than the next time the player is allowed to auto.
            // Make sure player isn't already winding up an auto.
            if(Time.time > basicAttack.nextAuto && !basicAttack.windingUp){
                basicAttack.windingUp = true;
                StartCoroutine(basicAttack.BasicAttackWindUp());
            }
        }
        else{
            // Stop the auto wind up since the enemy is no longer in range.
            StopCoroutine(basicAttack.BasicAttackWindUp());
            basicAttack.windingUp = false;
            // Move the player into range of the target.
            Vector3 enemyDest = targetedEnemy.transform.position;
            enemyDest.y = myCollider.bounds.center.y;
            navMeshAgent.destination = enemyDest;
        }
    }
}

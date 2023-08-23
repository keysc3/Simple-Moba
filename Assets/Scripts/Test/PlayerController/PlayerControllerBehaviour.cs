using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControllerBehaviour : MonoBehaviour, IPlayerMover
{
    private PlayerController newpc;

    private NavMeshAgent nma;

    private GameObject targetedEnemy;
    public GameObject TargetedEnemy { 
        get => targetedEnemy;
        set{
            if(value == null)
                nma.ResetPath();
            else
                targetedEnemy = value;
        }
    }
    //private float attackTime;
    private Camera mainCamera;
    //private RaycastHit hitInfo;
    //private Vector3 destination = Vector3.zero;
    public Vector3 Destination { 
        get => nma.destination; 
        set => nma.destination = value;
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
    //private Player player;
    //private UnitStats unitStats;
    private NewChampionSpells championSpells;
    private IPlayer player;

    // Called when the script instance is being loaded.
    private void Awake(){
        championSpells = GetComponent<NewChampionSpells>();
        newpc = new PlayerController(this);
        nma = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
        player = GetComponent<IPlayer>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        //championStats = (ChampionStats) player.unitStats;
        nma.updateRotation = false;
        //nma.speed = championStats.speed.GetValue();
        //attackTime = 1.0f/championStats.attackSpeed.GetValue();

    }

    // Called when the script is enabled.
    /*private void OnDisable(){
        if(player.navMeshAgent.isOnNavMesh && player.navMeshAgent != null)
            player.navMeshAgent.ResetPath();
    }*/

    // Update is called once per frame
    private void Update()
    {
        // Stop the players attack windup if casting or moving.
        /*if(player.isCasting || player.navMeshAgent.hasPath){
            player.basicAttack.windingUp = false;
        }*/

        if(nma.enabled){
            // Set the players destination.
            if(Input.GetMouseButtonDown(1)){
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if(Physics.Raycast(ray, out hitInfo)){
                        newpc.RightClick(hitInfo, gameObject);
                    }
            }

            // Stop player if they are at their destination or stop input received. 
            //if(nma.hasPath && !player.isCasting){
            if(nma.hasPath){
                if(transform.position == nma.destination || Input.GetKeyDown(KeyCode.S)){
                    TargetedEnemy = null;
                    nma.ResetPath();
                }
            }
        }

        // Point players forward at the direction they are cast or moving.
        // The player should never be casting something if currentspell is null but just to be safe check for null.
        if(player.IsCasting && player.CurrentCastedSpell != null && player.CurrentCastedSpell.CanMove)
            newpc.PlayerLookDirection(player.MouseOnCast);
        else if(nma.hasPath)
            newpc.PlayerLookDirection(nma.steeringTarget);

        if(TargetedEnemy != null && !player.IsCasting){
            if(!TargetedEnemy.GetComponent<IUnit>().IsDead)
                newpc.MovePlayerToEnemy();
            else
                TargetedEnemy = null;
        }
        //else
            //player.basicAttack.windingUp = false;
    }
}

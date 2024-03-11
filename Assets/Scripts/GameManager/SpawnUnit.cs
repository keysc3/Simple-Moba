using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnUnit : MonoBehaviour
{
    public GameObject spawnMenu;
    public List<GameObject> prefabs = new List<GameObject>();
    private LayerMask groundMask;
    private LayerMask enemyMask;
    private bool canMove = false;

    // Start is called before the first frame update
    void Start()
    {
        groundMask = LayerMask.GetMask("Ground");
        enemyMask = LayerMask.GetMask("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift)){
            if(Input.GetKeyDown(KeyCode.M)){
                if(spawnMenu.activeSelf)
                    spawnMenu.SetActive(false);
                else
                    spawnMenu.SetActive(true);
            }  
        }
    }

    public void UnitButtonClick(int prefabIndex){
        spawnMenu.SetActive(false);
        StartCoroutine(UnitPlacement(prefabs[prefabIndex]));
    }

    private IEnumerator UnitPlacement(GameObject spawn){
        bool breakBool = false;
        Stat stat = spawn.GetComponent<IUnit>().unitStats.maxHealth;
        while(!breakBool){
            print("Place!");
            if(Input.GetMouseButtonDown(0)){
                // Ray cast from mouse position.
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask)){
                    print("Hit ground");
                    Vector3 position = FindBestPosition(hit.point);
                    position.y = spawn.transform.position.y;
                    if(stat == null){
                        Instantiate(spawn, position, Quaternion.identity);
                        breakBool = true;
                    }
                    else{
                        if(canMove){
                            spawn.transform.position = position;
                            breakBool = true;
                        }
                    }
                }
            }
            else if(Input.GetKeyDown(KeyCode.Escape)){
                print("Cancel unit placement.");
                breakBool = true;
            }
            yield return null;
        }
        spawnMenu.SetActive(true);
    }

    private Vector3 FindBestPosition(Vector3 point){
        NavMeshHit meshHit;
        int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
        // Sample for a point on the walkable navmesh within 4 units of target position.
        if(NavMesh.SamplePosition(point, out meshHit, 4.0f, walkableMask)){
            point = meshHit.position;
        }
        return point;
    }

    public void DespawnUnitClick(){
        spawnMenu.SetActive(false);
        StartCoroutine(DespawnUnit());
    }

    private IEnumerator DespawnUnit(){
        bool breakBool = false;
        while(!breakBool){
            print("Click unit!");
            if(Input.GetMouseButtonDown(0)){
                // Ray cast from mouse position.
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, enemyMask)){
                    print("Hit unit!");
                    Destroy(hit.transform.gameObject);
                    breakBool = true;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Escape)){
                print("Cancel despawn unit.");
                breakBool = true;
            }
            yield return null;
        }
        spawnMenu.SetActive(true);
    }

    public void MoveUnitClick(){
        spawnMenu.SetActive(false);
        StartCoroutine(MoveUnit());
    }

    private IEnumerator MoveUnit(){
        bool breakBool = false;
        while(!breakBool){
            print("Click unit!");
            if(Input.GetMouseButtonDown(0)){
                // Ray cast from mouse position.
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, enemyMask)){
                    print("Hit unit!");
                    canMove = false;
                    StartCoroutine(UnitPlacement(hit.transform.gameObject));
                    canMove = true;
                    spawnMenu.SetActive(false);
                    yield break;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Escape)){
                print("Cancel unit selection.");
                breakBool = true;
            }
            yield return null;
        }
        spawnMenu.SetActive(true);
    }
}


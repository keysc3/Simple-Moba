using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnUnit : MonoBehaviour
{
    public GameObject panel;
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
                if(panel.activeSelf)
                    panel.SetActive(false);
                else
                    panel.SetActive(true);
            }  
        }
        if(Input.GetKeyDown(KeyCode.Escape) && panel.activeSelf)
            panel.SetActive(false);
    }

    public void UnitButtonClick(int prefabIndex){
        StartCoroutine(UnitPlacement(prefabs[prefabIndex]));
        panel.SetActive(false);
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
        panel.SetActive(true);
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
        StartCoroutine(DespawnUnit());
        panel.SetActive(false);
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
        panel.SetActive(true);
    }

    public void MoveUnitClick(){
        StartCoroutine(MoveUnit());
        panel.SetActive(false);
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
                    yield break;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Escape)){
                print("Cancel unit selection.");
                breakBool = true;
            }
            yield return null;
        }
        panel.SetActive(true);
    }
}


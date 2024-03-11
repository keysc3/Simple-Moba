using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnUnit : MonoBehaviour
{
    public GameObject spawnMenu;
    public List<GameObject> prefabs = new List<GameObject>();
    private LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        groundMask = LayerMask.GetMask("Ground");
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
                    Instantiate(spawn, position, Quaternion.identity);
                    breakBool = true;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Escape)){
                print("Cancel unit placement");
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
}


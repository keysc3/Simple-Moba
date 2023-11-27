using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements camera movement.
*
* @author: Colin Keys
*/
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 19.5f, -12f);
    public Transform targetObject;

    [SerializeField] private float cameraSpeed = 25.0f;

    private bool isLocked = true;
    private float screenWidth;
    private float screenHeight;

    // Start is called before the first frame update
    private void Start()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
    }

    // Update is called once per frame
    private void Update()
    {
        // Toggle camera being locked to the champion.
        if(Input.GetKeyDown(KeyCode.Y))
            isLocked = !isLocked;
        // If camera has a target.
        if(targetObject != null){
            // If the camera is locked follow the champion.
            if(isLocked){
                Vector3 newPosition = targetObject.transform.position + cameraOffset;
                newPosition.y = cameraOffset.y;
                transform.position = newPosition;
            }
            // Let the player move the camera by moving their mouse to the edges of the screen.
            else{
                Vector3 mousePos = Input.mousePosition;
                if(mousePos.x >= screenWidth)
                    transform.position += Vector3.right * cameraSpeed * Time.deltaTime;
                if(mousePos.x <= 0f)
                    transform.position += Vector3.left * cameraSpeed * Time.deltaTime;
                if(mousePos.y >= screenHeight)
                    transform.position += Vector3.forward * cameraSpeed * Time.deltaTime;
                if(mousePos.y <= 0f)
                    transform.position += Vector3.back * cameraSpeed * Time.deltaTime;
            }
        } 
    }
}

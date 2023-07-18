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
    /*[SerializeField] private float xOffset = -11.0f;
    [SerializeField] private float yOffset = 23.4f;
    [SerializeField] private float zOffset = 15.0f;*/
    public Transform targetObject;

    //[SerializeField] private float smoothness = 0.5f;
    [SerializeField] private float cameraSpeed = 25.0f;

    private bool isLocked = true;
    private float screenWidth;
    private float screenHeight;
    private Vector3 initialOffset;

    // Start is called before the first frame update
    private void Start()
    {
        initialOffset = transform.position - targetObject.transform.position;
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
                Vector3 newPosition = targetObject.transform.position + initialOffset;
                //Vector3 newPosition = new Vector3(targetObject.transform.position.x + xOffset, targetObject.transform.position.y + yOffset, 
                //targetObject.transform.position.z - zOffset);
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

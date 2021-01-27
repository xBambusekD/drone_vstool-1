using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private bool topCameraMode = false;

    private bool left = false;
    private bool right = false;
    private bool up = false;
    private bool down = false;

    private bool freeCam = false;

    public GameObject DroneGameObject;
    public GameObject DroneModel;
    public float CameraSpeed = 0.5f;
	
	// Update is called once per frame

    public void reset(){
        transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
    }

    public void SetCokcpitMode()
    {
        transform.SetParent(DroneModel.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, -90, 0);
    }

    public void SetStandardMode()
    {
        transform.SetParent(DroneGameObject.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void SetFreeMode()
    {
        transform.parent = null;
    }

	void Update () {
        // if (Input.GetKeyUp("k")) //set initial position to 3rd person view
        // {
        //     transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
        // }
        // else if (Input.GetKeyUp("j")) //set initial position to 3rd person view
        // {
        //     transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        // }
        // else
        // {

        HandleCameraInputKeys();

            //movement
            float moveVertical = 0;
            float moveHorizontal = 0;
            if(up)
                moveVertical += 0.5f;
            
            if(down)
                moveVertical -= 0.5f;
            
            if(left)
                moveHorizontal -= 0.5f;
            
            if(right)
                moveHorizontal += 0.5f;

            Vector3 rotace = new Vector3(moveVertical * -CameraSpeed, moveHorizontal * CameraSpeed, 0);

            //move up 
            // if (transform.localRotation.eulerAngles.x >= 90.0f && transform.localRotation.eulerAngles.x < 180.0f && rotace.x > 0) rotace.x = 0;
    
            // //down
            // if (transform.localRotation.eulerAngles.x <= 330.0f && transform.localRotation.eulerAngles.x > 180.0f && rotace.x < 0) rotace.x = 0;

            // //move to sides
            // if (transform.localRotation.eulerAngles.y >= 45.0f && transform.localRotation.eulerAngles.y < 180.0f && rotace.y > 0) rotace.y = 0;
            // if (transform.localRotation.eulerAngles.y <= 315.0f && transform.localRotation.eulerAngles.y > 180.0f && rotace.y < 0) rotace.y = 0;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotace);


        // }
    }

    private void HandleCameraInputKeys()
    {
        float moveVertical = Input.GetAxis("CameraRotateY");
        float moveHorizontal = Input.GetAxis("CameraRotateX");
        Vector3 rotace = new Vector3(moveVertical * -CameraSpeed, moveHorizontal * CameraSpeed, 0);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotace);
    }

    public void upHold(){
        up = true;
    }

    public void upRelease(){
        up = false;
    }

    public void downHold(){
        down = true;
    }

    public void downRelease(){
        down = false;
    }

    public void leftHold(){
        left = true;
    }

    public void leftRelease(){
        left = false;
    }

    public void rightHold(){
        right = true;
    }

    public void rightRelease(){
        right = false;
    }
}

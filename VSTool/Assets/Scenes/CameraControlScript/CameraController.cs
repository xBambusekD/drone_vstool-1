using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private bool topCameraMode = false;

    // Use this for initialization
    void Start () {
		
	}

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp("k")) //set initial position to 3rd person view
        {
            transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
        }
        else if (Input.GetKeyUp("j")) //set initial position to 3rd person view
        {
            transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        }
        else
        {
            //movement
            float moveVertical = Input.GetAxis("Vertical");
            float moveHorizontal = Input.GetAxis("Horizontal");

            Vector3 rotace = new Vector3(moveVertical * -0.8f, moveHorizontal * 0.8f, 0);

            //move up 
            if (transform.localRotation.eulerAngles.x >= 90.0f && transform.localRotation.eulerAngles.x < 180.0f && rotace.x > 0) rotace.x = 0;

            //down
            if (transform.localRotation.eulerAngles.x <= 330.0f && transform.localRotation.eulerAngles.x > 180.0f && rotace.x < 0) rotace.x = 0;

            //move to sides
            if (transform.localRotation.eulerAngles.y >= 45.0f && transform.localRotation.eulerAngles.y < 180.0f && rotace.y > 0) rotace.y = 0;
            if (transform.localRotation.eulerAngles.y <= 315.0f && transform.localRotation.eulerAngles.y > 180.0f && rotace.y < 0) rotace.y = 0;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotace);
        }
    }
}

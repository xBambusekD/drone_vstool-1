using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    private bool topCameraMode = false;

    private bool left = false;
    private bool right = false;
    private bool up = false;
    private bool down = false;

    private bool leftFree = false;
    private bool rightFree = false;
    private bool upFree = false;
    private bool downFree = false;

    private bool freeCam = false;
    private bool cockpit = false;

    public GameObject DroneGameObject;
    public GameObject DroneModel;
    public GameObject VideoScreen;
    public GameObject DownDangerGameObject;
    public GameObject UPDangerGameObject;
    public GameObject DroneDangerGameObject;
    public GameObject VideoUI;
    public GameObject Buttons;
    public GameObject CompassGameObject;
    public GameObject DroneModelGameObject;
    public IconManager Icons;
    public GameObject IconsGO;
    public GameObject PitchAndScroollGameObject;

    public CameraZoom CameraZoom;
    // Update is called once per frame

    public void reset(){
        transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
    }

    public void SetCokcpitMode()
    {
        Icons.unsetFreeMode();
        CameraZoom.unsetFreeMode();
        SetGameObjects(true);
        transform.SetParent(DroneModel.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, -90, 0);
        cockpit = true;
    }

    public void SetStandardMode()
    {
        CameraZoom.unsetFreeMode();
        Icons.unsetFreeMode();
        SetGameObjects(true);
        transform.SetParent(DroneGameObject.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        cockpit = false;
    }

    private void SetGameObjects(bool cameraBool)
    {
        IconsGO.transform.GetChild(0).gameObject.SetActive(!cameraBool);
        VideoUI.SetActive(!cameraBool);
        VideoScreen.SetActive(cameraBool);
        DownDangerGameObject.SetActive(cameraBool);
        UPDangerGameObject.SetActive(cameraBool);
        DroneDangerGameObject.SetActive(cameraBool);
        CompassGameObject.SetActive(cameraBool);
        PitchAndScroollGameObject.SetActive(!cameraBool);
    }
    public void SetFreeMode()
    {
        CameraZoom.setFreeMode();
        Icons.setFreeMode();
        cockpit = false;
        transform.parent = null;
        freeCam = true;
        SetGameObjects(false);
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
        //Free Cam Movement

        if (cockpit)
            CompassGameObject.transform.eulerAngles = new Vector3(0, 0, DroneModelGameObject.transform.eulerAngles.x);
        else
        {
            CompassGameObject.transform.eulerAngles = new Vector3(0, 0, 0);

        }

        if (upFree)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 3.5f, Space.Self); //LEFT

        }

        if (downFree)
        {
            transform.Translate(Vector3.down * Time.deltaTime * 3.5f, Space.Self); //LEFT
        }

        if (leftFree)
        {
            transform.Translate(Vector3.left * Time.deltaTime * 3.5f, Space.Self); //LEFT
        }

        if (rightFree)
        {
            transform.Translate(Vector3.right * Time.deltaTime * 3.5f, Space.Self); //LEFT
        }

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

            Vector3 rotace = new Vector3(moveVertical * -0.8f, moveHorizontal * 0.8f, 0);

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


    public void upFreeHold()
    {
        upFree = true;
    }

    public void downFreeHold()
    {
        downFree = true;
    }

    public void leftFreeHold()
    {
        leftFree = true;
    }

    public void rightFreeHold()
    {
        rightFree = true;
    }

    public void upFreeRelease()
    {
        upFree = false;
    }

    public void downFreeRelease()
    {
        downFree = false;
    }

    public void leftFreeRelease()
    {
        leftFree = false;
    }

    public void rightFreeRelease()
    {
        rightFree = false;
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

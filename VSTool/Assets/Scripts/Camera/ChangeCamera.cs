using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour {
    public Camera thirdPersonCamera;
    public Camera topCamera;
    public GameObject videoBoard;

    public static Camera currentCam;

    public void ShowTopView()
    {
        thirdPersonCamera.enabled = false;
        topCamera.enabled = true;
        currentCam = topCamera;
        videoBoard.SetActive(false); 
    }

    public void ShowThirdPersonView()
    {
        thirdPersonCamera.enabled = true;
        topCamera.enabled = false;
        currentCam = thirdPersonCamera;
        videoBoard.SetActive(true);
    }

    // Use this for initialization
    void Start () {
        ShowThirdPersonView();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyUp("q"))
        {
            ShowThirdPersonView();
        }
        if (Input.GetKeyUp("w"))
        {
            ShowTopView();
        }
        */
 
        if (Input.GetKeyUp("b"))
        {
            videoBoard.SetActive(!videoBoard.activeSelf);
        }
    }
}

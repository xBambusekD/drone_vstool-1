using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    //bool isUpButtonPressed = false;
    //bool isDownButtonPressed = false;

    // Use this for initialization
    void Start()
    {

    }

    private bool zoom;
    private bool unzoom;
    // Update is called once per frame
    void Update()
    {
        HandleCameraInputKeys();

        float scroll=0;

        /*
        if (Input.GetKeyDown(KeyCode.PageUp)) isUpButtonPressed = true;
        if (Input.GetKeyUp(KeyCode.PageUp)) isUpButtonPressed = false;
        if (Input.GetKeyDown(KeyCode.PageDown)) isDownButtonPressed = true;
        if (Input.GetKeyUp(KeyCode.PageDown)) isDownButtonPressed = false;
        
        if(isUpButtonPressed) scroll = 0.2f;
        else if (isDownButtonPressed) scroll = -0.2f;
        else scroll = Input.GetAxis("Mouse ScrollWheel");
        */
        // scroll = Input.GetAxis("CameraZoom")*0.5f;
        if(zoom)
            scroll += 0.1f;
        if(unzoom)
            scroll -=0.1f;
        if ((transform.localPosition.z > -7 || scroll > 0) && (transform.localPosition.z < 0.8f || scroll < 0))
        {
            transform.localPosition = transform.localPosition + new Vector3(0, 0, scroll * 0.4f);
        }
    }

    private void HandleCameraInputKeys()
    {
        float zoomIn = Input.GetAxis("CameraZoomIn");
        float zoomOut = Input.GetAxis("CameraZoomOut");
        if(zoomIn > 0)
        {
            transform.localPosition = transform.localPosition + new Vector3(0, 0, zoomIn * 0.2f);
        }
        if(zoomOut > 0)
        {
            transform.localPosition = transform.localPosition + new Vector3(0, 0, -zoomOut * 0.2f);
        }
    }

    public void zoomHold(){
        zoom = true;
    }
    public void zoomRelease(){
        zoom = false;
    }

    public void unzoomHold(){
        unzoom = true;
    }
    public void unzoomRelease(){
        unzoom = false;
    }
}

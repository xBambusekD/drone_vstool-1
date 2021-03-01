using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private bool zoom;
    private bool unzoom;
    private float ZoomSpeed = 0.02f;

    public SliderManager CameraSpeedSlider;

    private bool zoom;
    private bool unzoom;

    private bool freeMode;
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
        if (freeMode)
        {
            transform.localPosition = transform.localPosition + new Vector3(0, 0, scroll * 0.4f);
        }
        else
        {
            if ((transform.localPosition.z > -7 || scroll > 0) && (transform.localPosition.z < 0.8f || scroll < 0))
            {
                transform.localPosition = transform.localPosition + new Vector3(0, 0, scroll * 0.4f);
            }
        }
        
    }

    public void setFreeMode()
    {
        freeMode = true;
    }

    public void unsetFreeMode()
    {
        freeMode = false;
    }


    private void HandleCameraInputKeys()
    {
        float zoomIn = Input.GetAxis("CameraZoomIn");
        float zoomOut = Input.GetAxis("CameraZoomOut");
        if(zoomIn > 0)
        {
            transform.localPosition = transform.localPosition + new Vector3(0, 0, zoomIn * ZoomSpeed);
        }
        if(zoomOut > 0)
        {
            transform.localPosition = transform.localPosition + new Vector3(0, 0, -zoomOut * ZoomSpeed);
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

    public void ChangeCameraSpeed(float cameraSpeed) {
        ZoomSpeed = cameraSpeed * 0.01f;
    }
}

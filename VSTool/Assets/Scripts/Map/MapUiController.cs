/*
Author: Róbert Hubinák (xhubin03@stud.fit.vutbr.cz)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapUiController : MonoBehaviour
{
    public GameObject MainCanvas;
    public GameObject MapCanvas;
    public GameObject MainCameras;
    public GameObject MapCamera;

    public Transform Drone;
    private bool follow = false;

    private void LateUpdate()
    {
        if (follow)
        {
            Vector3 newPosition = Drone.position;
            newPosition.y = MapCamera.transform.position.y;
            MapCamera.transform.position = newPosition;
            //MapCamera.transform.rotation = Quaternion.Euler(90f, Drone.eulerAngles.y, 0f);
        }
    }

    public void BackToDroneView()
    {
        GuiController.isMap = false;
        ShowBuildings.BuildingsHidden = false;
        MainCanvas.gameObject.SetActive(true);
        MapCanvas.gameObject.SetActive(false);
        MainCameras.gameObject.SetActive(true);
        MapCamera.gameObject.SetActive(false);
    }

    public void FollowDrone()
    {
        follow = !follow;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDrone : MonoBehaviour
{
    public GameObject newDrone;
    public Transform ourDrone;
    private int droneNumber = 1;
    public Transform tarfetTransform;
    public GameObject dronesPrefab;

    public Transform iconTransform;
    public GameObject icon;
    public GameObject PopUp;

    public RenderTexture RenderTexture;

    
    public void addDrone(Transform position){ 
        Vector3 newPosition = position.position;

        GameObject Clone = Instantiate(newDrone, newPosition, ourDrone.rotation);
        Clone.name = "DroneObject" + droneNumber.ToString();
        //DroneList.drones.Add(Clone);ß
        //DisplayDrones.dronesList.Add(Clone);
        Drones.drones.Add(new Drone(Clone, new DroneFlightData()));
        Drones.DroneAdded(tarfetTransform,dronesPrefab,iconTransform,icon, PopUp, RenderTexture);
        Debug.Log(Clone.name + "added");
        droneNumber++;
        //Clone.GetComponent("DroneController").enabled = false;
    }
}

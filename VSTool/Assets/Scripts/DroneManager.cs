using System;
using System.Collections;
using System.Collections.Generic;
using DroCo;
using UnityEngine;

public class DroneManager : Singleton<DroneManager> {

    public GameObject newDrone;
    public Transform ourDrone;
    private int droneNumber = 1;
    public Transform dronesPanelGrid;
    public GameObject dronesPrefab;
    public Transform iconTransform;
    public GameObject icon;
    public GameObject PopUp;
    public RenderTexture PopUpRenderTexture;


    public void AddDrone(DroneFlightData flightData) {
        Mapbox.Utils.Vector2d mapboxPosition = new Mapbox.Utils.Vector2d(flightData.Latitude, flightData.Longitude);
        Vector3 position3d = MapController.Instance.Map.GeoToWorldPosition(mapboxPosition, false);
        //float groundAltitude = MapController.Instance.Map.QueryElevationInUnityUnitsAt(MapController.Instance.Map.WorldToGeoPosition(position3d));
        position3d.y = (float) flightData.Altitude;

        GameObject Clone = Instantiate(newDrone, position3d, ourDrone.rotation);
        Clone.name = "DroneObject" + droneNumber.ToString();
        Drones.drones.Add(new Drone(Clone, flightData));
        Drones.DroneAdded(dronesPanelGrid, dronesPrefab, iconTransform, icon, PopUp, PopUpRenderTexture);
        droneNumber++;
        Clone.transform.SetParent(transform);
    }



    public void HandleReceivedDroneData(string data) {
        DroneFlightData flightData = JsonUtility.FromJson<DroneFlightData>(data);
        foreach (Drone drone in Drones.drones) {
            // Drone is already present and instaciated, we found it, just update position
            if (drone.FlightData.DroneId == flightData.DroneId) {
                drone.UpdateDroneFlightData(flightData);
                return;
            }
        }
        // Drone is new one in the system, we need to instanciate it
        AddDrone(flightData);
    }


}

[Serializable]
public class DroneFlightData {
    public string DroneId;
    public double Altitude;
    public double Latitude;
    public double Longitude;
    public double Pitch;
    public double Roll;
    public double Yaw;
    public double Compass;

    public DroneFlightData() {
        DroneId = "unset";
        Altitude = 0;
        Latitude = 0;
        Longitude = 0;
        Pitch = 0;
        Roll = 0;
        Yaw = 0;
        Compass = 0;
    }

    public void SetData(double height, double latitude, double longitute, double pitch, double roll, double yaw, double compass) {
        Altitude = height;
        Latitude = latitude;
        Longitude = longitute;
        Pitch = pitch;
        Roll = roll;
        Yaw = yaw;
        Compass = compass;
    }
}

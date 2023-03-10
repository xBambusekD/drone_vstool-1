using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using UnityEngine;

public class DroneManager : Singleton<DroneManager> {

    public List<Drone> Drones = new List<Drone>();

    public GameObject DronePrefab;
    public ArcGISMapComponent Map;

    public void HandleReceivedDroneData(string data) {
        DroneFlightData flightData = JsonUtility.FromJson<DroneFlightData>(data);
        //Debug.Log(data);
        foreach (Drone drone in Drones) {
            // Drone is already present and instaciated, we found it, just update position
            if (drone.FlightData.DroneId == flightData.DroneId) {
                drone.UpdateDroneFlightData(flightData);
                return;
            }
        }
        // Drone is new one in the system, we need to instanciate it
        AddDrone(flightData);
    }

    private void AddDrone(DroneFlightData flightData) {
        if (double.IsNaN(flightData.Latitude)) {
            Debug.LogError("Latitude NAN ");
            flightData.Latitude = 49.226564;
        }
        if (double.IsNaN(flightData.Longitude)) {
            Debug.LogError("Longitude NAN");
            flightData.Longitude = 16.596639;
        }

        GameObject newDroneGameObj = Instantiate(DronePrefab, Map.transform);
        Drone newDrone = newDroneGameObj.GetComponent<Drone>();
        newDrone.InitDrone(flightData);
        Drones.Add(newDrone);
    }
}

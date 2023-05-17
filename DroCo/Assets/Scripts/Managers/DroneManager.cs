using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using UnityEngine;

public class DroneManager : Singleton<DroneManager> {

    public IDictionary<string, Drone> Drones = new Dictionary<string, Drone>();

    public GameObject DronePrefab;
    public ArcGISMapComponent Map;


    public void HandleReceivedDroneList(DroneStaticData[] droneStaticDatas) {
        foreach (DroneStaticData dsd in droneStaticDatas) {
            if (Drones.ContainsKey(dsd.client_id)) {
                Drones[dsd.client_id].StaticData = dsd;
            } else {
                AddDrone(dsd);
            }
        }
    }

    public void HandleReceivedDroneData(DroneFlightData flightData) {
        if (Drones.ContainsKey(flightData.client_id)) {
            //GameManager.Instance.CenterMap(flightData);
            Drones[flightData.client_id].UpdateDroneFlightData(flightData);
        } else { //prisla data s neznamym drone ID -> pozadame server o novy seznam dronu
            WebSocketManager.Instance.SendDroneListRequest();
        }
    }

    public void HandleReceivedVehicleData(DroneVehicleData vehicleData) {
        if (Drones.ContainsKey(vehicleData.client_id)) {
            Drones[vehicleData.client_id].UpdateDroneVehicleData(vehicleData);
        } else { //prisla data s neznamym drone ID -> pozadame server o novy seznam dronu
            WebSocketManager.Instance.SendDroneListRequest();
        }
    }

    private void AddDrone(DroneStaticData dsd) {
        Debug.Log("adding new drone with id: " + dsd.client_id);

        GameObject newDroneGameObj = Instantiate(DronePrefab, Map.transform);
        Drone newDrone = newDroneGameObj.GetComponent<Drone>();
        newDrone.InitDrone(dsd);
        Drones.Add(dsd.client_id, newDrone);
    }
}

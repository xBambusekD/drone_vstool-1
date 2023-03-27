using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using UnityEngine;

public class DroneManager : Singleton<DroneManager> {

    public IDictionary<string, Drone> Drones = new Dictionary<string, Drone>();

    public GameObject DronePrefab;
    public ArcGISMapComponent Map;


    public void HandleHandshakeDone() {
        WebSocketManager.Instance.SendDroneListRequest();
    }

    public void HandleReceivedDroneList(DroneStaticData[] droneStaticDatas) {
        foreach (DroneStaticData dsd in droneStaticDatas) {
            if (Drones.ContainsKey(dsd.ClientID)) {
                Drones[dsd.ClientID].StaticData = dsd;
            } else {
                AddDrone(dsd);
            }
        }
    }

    public void HandleReceivedDroneData(DroneFlightData flightData) {
        if (Drones.ContainsKey(flightData.ClientID)) {
            Drones[flightData.ClientID].UpdateDroneFlightData(flightData);
        } else { //prisla data s neznamym drone ID -> pozadame server o novy seznam dronu
            WebSocketManager.Instance.SendDroneListRequest();
        }
    }

    private void AddDrone(DroneStaticData dsd) {
        Debug.Log("adding new drone with id: " + dsd.ClientID);

        GameObject newDroneGameObj = Instantiate(DronePrefab, Map.transform);
        Drone newDrone = newDroneGameObj.GetComponent<Drone>();
        newDrone.InitDrone(dsd);
        Drones.Add(dsd.ClientID, newDrone);
    }
}

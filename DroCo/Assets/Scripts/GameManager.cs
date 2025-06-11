using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : Singleton<GameManager> {

    [SerializeField]
    private GameObject DronePrefab;
    [SerializeField]
    private Transform RootScene;

    public IDictionary<string, Drone> Drones = new Dictionary<string, Drone>();

    private void Start() {
        WebSocketServer.Instance.StartServer();
    }

    public void OnClientConnected(DroneStaticData newDroneData) {
        // Instantiate new drone object and put it into a dictionary
        GameObject newDroneGameObj = Instantiate(DronePrefab, RootScene);
        Drone drone = newDroneGameObj.GetComponent<Drone>();
        drone.InitDrone(newDroneData);
        Drones.Add(newDroneData.client_id, drone);
    }

    public void OnClientDisconnected() {
        
    }

    public void HandleReceivedDroneData(DroneFlightData flightData) {
        // Update flight data of connected drone
        if (Drones.ContainsKey(flightData.client_id)) {
            Drones[flightData.client_id].UpdateDroneFlightData(flightData);
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : Singleton<GameManager> {

    [SerializeField]
    private GameObject DronePrefab;
    [SerializeField]
    private Transform RootScene;
    [SerializeField]
    private DroneVirtualStickController VirtualStickController;

    public IDictionary<string, Drone> Drones = new Dictionary<string, Drone>();

    private bool droneControlEnabled = false;
    private IEnumerator droneControlCommandSender;


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

    public void EnableDroneControls(bool enable) {
        // Just try to get the ID of the first drone in the drone dictionary (if multiple drones are connected, some drone selector should be implemented)
        if (Drones.Count > 0) {
            string droneID = Drones.First().Key;

            // Send message to enable drone virtual stick controls
            SendEnableDroneControl(Drones.First().Key, enable);
            droneControlEnabled = enable;

            // Start sending command messages in frequency defined by "interval"
            if (enable) {
                droneControlCommandSender = SendDroneControlCommandsCoroutine(droneID, 0.1f);
                StartCoroutine(droneControlCommandSender);
                Debug.Log("Enabled virtual sticks drone controls.");
            } else {
                StopCoroutine(droneControlCommandSender);
                Debug.Log("Disabled virtual sticks drone controls.");
            }
        } else {
            Debug.LogError("Cannot enable drone controls, because no drones are connected.");
        }
    }

    private IEnumerator SendDroneControlCommandsCoroutine(string droneID, float interval) {
        while (true) {
            SendDroneControlCommand(droneID, VirtualStickController.CurrentCommand);
            yield return new WaitForSeconds(interval);
        }
    }

    private void SendEnableDroneControl(string droneID, bool enable) {
        EnableDroneControl enableDrone = new EnableDroneControl() { enable = enable };
        string msg = $"{{\"type\":\"enable_control\", \"data\": {enableDrone.ToString()}}}";
        WebSocketServer.Instance.SendMessageToClient(droneID, msg);
    }

    private void SendDroneControlCommand(string droneID, DroneControlCommand command) {
        string msg = $"{{\"type\":\"control_command\", \"data\": {command.ToString()}}}";
        WebSocketServer.Instance.SendMessageToClient(droneID, msg);
    }

}

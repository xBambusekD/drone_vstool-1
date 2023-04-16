using System;
using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Elevation.Base;
using Esri.GameEngine.Geometry;
using UnityEngine;

public class Drone : MonoBehaviour {

    public DroneFlightData FlightData {
        get; set;
    }

    public DroneStaticData StaticData {
        get; set;
    }

    public CustomPipelinePlayer2 StreamPlayer;
    public Material VideoMaterial;
    public MeshRenderer VideoScreen;

    private ArcGISLocationComponent GPSLocation;

    public Drone(GameObject droneGameObject) {
    }

    /*public void Start() {
        Instantiate(StreamPlayer);
    }*/


    public void InitDrone(DroneStaticData staticData) {
        GPSLocation = GetComponent<ArcGISLocationComponent>();
        GPSLocation.enabled = true;

        //Material mat = new Material(VideoMaterial);
        //VideoScreen.material = mat;
        //StreamPlayer = new CustomPipelinePlayer2();
        //StreamPlayer = AddComponent()
        //Instantiate(StreamPlayer);


        try {
            //StreamPlayer.TargetMaterial = mat;
            StreamPlayer.pipeline = "rtmpsrc location=rtmp://" + GameManager.Instance.ServerIP + ":" + GameManager.Instance.RTMPPort + "/live/" + staticData.ClientID + " ! decodebin";
            StreamPlayer.gameObject.SetActive(true);
        } catch {

        }
    }

    public void UpdateDroneFlightData(DroneFlightData flightData) {
        FlightData = flightData;

        GPSLocation.Position = new ArcGISPoint(flightData.Longitude, flightData.Latitude, flightData.Altitude, new ArcGISSpatialReference(4326));
        GPSLocation.Rotation = new ArcGISRotation(flightData.Yaw, flightData.Pitch, flightData.Roll);     
    }

    public void UpdateDroneVehicleData(DroneVehicleData vehicleData) {
        //if(StreamPlayer == null) return;
        StreamPlayer.VehicleData = vehicleData;
    }
}

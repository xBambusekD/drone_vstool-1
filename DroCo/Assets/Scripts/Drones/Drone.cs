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

    public VehicleDataRenderer VehicleRenderer;
    public CustomPipelinePlayer StreamPlayer;
    //public Material VideoMaterial;
    //public MeshRenderer VideoScreen;

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
            StreamPlayer.pipeline = "rtmpsrc location=rtmp://" + GameManager.Instance.ServerIP + ":" + GameManager.Instance.RTMPPort + "/live/" + staticData.client_id + " ! decodebin";
            StreamPlayer.gameObject.SetActive(true);
        } catch {

        }
    }

    public void UpdateDroneFlightData(DroneFlightData flightData) {
        FlightData = flightData;

        GPSLocation.Position = new ArcGISPoint(flightData.gps.longitude, flightData.gps.latitude, flightData.altitude, new ArcGISSpatialReference(4326));
        GPSLocation.Rotation = new ArcGISRotation(flightData.aircraft_orientation.yaw, flightData.aircraft_orientation.pitch, flightData.aircraft_orientation.roll);     
    }

    public void UpdateDroneVehicleData(DroneVehicleData vehicleData) {
        if(StreamPlayer == null && VehicleRenderer == null) return;
        VehicleRenderer.gameObject.SetActive(true);
        VehicleRenderer.UpdateVehicleData(vehicleData);
    }
}

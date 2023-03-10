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

    public CustomPipelinePlayer StreamPlayer;
    public Material VideoMaterial;
    public MeshRenderer VideoScreen;

    private ArcGISLocationComponent GPSLocation;

    public Drone(GameObject droneGameObject, DroneFlightData flightData) {
        UpdateDroneFlightData(flightData);
    }


    public void InitDrone(DroneFlightData flightData) {
        GPSLocation = GetComponent<ArcGISLocationComponent>();
        GPSLocation.enabled = true;

        UpdateDroneFlightData(flightData);
        //Material mat = new Material(VideoMaterial);
        //VideoScreen.material = mat;
        try {
            //StreamPlayer.TargetMaterial = mat;
            StreamPlayer.pipeline = "rtmpsrc location=rtmp://" + GameManager.Instance.ServerIP + ":" + GameManager.Instance.RTMPPort + "/live/" + flightData.DroneId.ToLower().Replace("-", "_") + " ! decodebin";
            StreamPlayer.gameObject.SetActive(true);
        } catch {

        }
    }

    public void UpdateDroneFlightData(DroneFlightData flightData) {
        FlightData = flightData;

        GPSLocation.Position = new ArcGISPoint(flightData.Longitude, flightData.Latitude, flightData.Altitude, new ArcGISSpatialReference(4326));
        GPSLocation.Rotation = new ArcGISRotation(flightData.Yaw, flightData.Pitch, flightData.Roll);     
    }

}

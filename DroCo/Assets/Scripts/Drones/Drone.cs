using System;
using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Elevation.Base;
using Esri.GameEngine.Geometry;
using UnityEngine;

public class Drone : InteractiveObject {

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

    public GameObject DroneModel;

    public ListItemButton DroneListItem;

    public GameObject Drone2DPrefab;
    private Drone2D drone2DRepresentation;


    public Drone(GameObject droneGameObject) {
    }

    /*public void Start() {
        Instantiate(StreamPlayer);
    }*/


    public void InitDrone(DroneStaticData staticData) {
        GPSLocation = GetComponent<ArcGISLocationComponent>();
        GPSLocation.enabled = true;

        drone2DRepresentation = Instantiate(Drone2DPrefab, GameManager.Instance.Map2DViewArcGISMap.transform).GetComponent<Drone2D>();
        drone2DRepresentation.InitDrone();

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

        drone2DRepresentation.UpdateFlightData(flightData);

        DroneListItem.UpdateHeight(flightData.altitude);
        DroneListItem.UpdateDistance(Vector3.Distance(Camera.main.transform.position, this.transform.position));
    }

    public void UpdateDroneVehicleData(DroneVehicleData vehicleData) {
        if(StreamPlayer == null && VehicleRenderer == null) return;
        VehicleRenderer.gameObject.SetActive(true);
        VehicleRenderer.UpdateVehicleData(vehicleData);
    }

    public override void Highlight(bool highlight) {
        base.Highlight(highlight);
        drone2DRepresentation.Highlight(highlight);
    }

}

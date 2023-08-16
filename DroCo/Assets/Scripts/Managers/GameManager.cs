using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Samples.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Extent;
using Esri.GameEngine.Geometry;
using Esri.Unity;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager> {

    public enum DisplayState {
        Scene3DView,
        Map2DView
    }

    public string ServerIP = "butcluster.ddns.net";
    public int ServerPort = 5555;
    public int RTMPPort = 1935;

    public string APIKey = "";
    public ArcGISMapComponent Scene3DViewArcGISMap;
    public ArcGISMapComponent Map2DViewArcGISMap;
    public ArcGISCameraComponent MainCamera;
    public ArcGISCameraComponent MinimapCamera;

    private bool carDetectorRunning = false;

    private bool mapCentered = false;
    private DroneFlightData firstDroneFlightData;

    [SerializeField]
    private MinimapUI minimapUI;
    private ArcGISCameraControllerTouch sceneViewCameraController;
    private ArcGISCameraControllerTouch minimapCameraController;

    public DisplayState CurrentDisplayState {
        get; private set;
    }


    private void Start() {
        WebSocketManager.Instance.ConnectToServer(ServerIP, ServerPort);

        sceneViewCameraController = MainCamera.GetComponent<ArcGISCameraControllerTouch>();
        minimapCameraController = MinimapCamera.GetComponent<ArcGISCameraControllerTouch>();

        StartCoroutine(InitSceneView());
    }

    private IEnumerator InitSceneView() {
        yield return new WaitForEndOfFrame();
        Open3DSceneView();
    }

    private void Update() {
        //if (Keyboard.current[Key.C].wasPressedThisFrame) {
        //    carDetectorRunning = !carDetectorRunning;
        //    foreach (KeyValuePair<string, Drone> drone in DroneManager.Instance.Drones) {
        //        WebSocketManager.Instance.SendCarDetectionRequest(drone.Key, carDetectorRunning);
        //    }
        //}
    }

    public void SwitchSceneMapView() {
        switch (CurrentDisplayState) {
            case DisplayState.Map2DView:
                Open3DSceneView();
                break;
            case DisplayState.Scene3DView:
                Open2DMapView();
                break;
        }
    }

    private void Open3DSceneView() {
        CurrentDisplayState = DisplayState.Scene3DView;
        minimapUI.SetSceneView();

        sceneViewCameraController.enabled = CameraManager.Instance.FollowingTarget ? false : true;
        minimapCameraController.enabled = false;
    }

    private void Open2DMapView() {
        CurrentDisplayState = DisplayState.Map2DView;
        minimapUI.SetMinimapView();

        sceneViewCameraController.enabled = false;
        minimapCameraController.enabled = true;
    }


    private void ListAllGameObjects() {
        Object[] tempList = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (object go in tempList) {
            if (go is GameObject goObj) {
                Debug.Log(goObj.name + " hide flags: " + goObj.hideFlags);
                goObj.hideFlags = HideFlags.DontSaveInEditor;
            }
        }
    }

    public void HandleHandshakeDone() {
        WebSocketManager.Instance.SendDroneListRequest();
    }

    public void CenterMap(DroneFlightData flightData) {
        if (!mapCentered) {
            mapCentered = true;
            firstDroneFlightData = flightData;

            // Set scene view map and load all possible 3d structures and layers
            Scene3DViewArcGISMap.OriginPosition = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, firstDroneFlightData.altitude, new ArcGISSpatialReference(4326));
            Scene3DViewArcGISMap.MapType = Esri.GameEngine.Map.ArcGISMapType.Local;
            Scene3DViewArcGISMap.MapTypeChanged += new ArcGISMapComponent.MapTypeChangedEventHandler(CreateArcGISMap);
            CreateArcGISMap();

            // Set 2d minimap and center it to the position of the first drone
            Map2DViewArcGISMap.OriginPosition = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, 0, new ArcGISSpatialReference(4326));
            Map2DViewArcGISMap.MapType = Esri.GameEngine.Map.ArcGISMapType.Local;

            // Center the main camera
            var cameraLocationComponent = MainCamera.GetComponent<ArcGISLocationComponent>();
            cameraLocationComponent.Position = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, firstDroneFlightData.altitude + 50, new ArcGISSpatialReference(4326));
            cameraLocationComponent.Rotation = new ArcGISRotation(65, 68, 0);

            // Center the minimap camera
            var minimapCameraLocationComponent = MinimapCamera.GetComponent<ArcGISLocationComponent>();
            minimapCameraLocationComponent.Position = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, 500, new ArcGISSpatialReference(4326));
            
        }
    }

    public void CreateArcGISMap() {
        Debug.Log("CREATE ARCGIS MAP");
        var arcGISm = new Esri.GameEngine.Map.ArcGISMap(Scene3DViewArcGISMap.MapType);
        arcGISm.Basemap = new Esri.GameEngine.Map.ArcGISBasemap(Esri.GameEngine.Map.ArcGISBasemapStyle.ArcGISImagery, APIKey);

        // Check whether the drone is flying in some known location
        MapManager.Location knownLocation = MapManager.Instance.GetKnownLocation(firstDroneFlightData.gps);

        // Load Scene Layer data for the known location
        foreach (string layerData in MapManager.Instance.Get3DObjectSceneLayerData()) {
            var objectSceneLayer = new Esri.GameEngine.Layers.ArcGIS3DObjectSceneLayer(layerData, knownLocation + "_3DObjectSceneLayer", 1.0f, true, "");
            arcGISm.Layers.Add(objectSceneLayer);
        }

        // Add base elevation layer
        arcGISm.Elevation.ElevationSources.Add(new Esri.GameEngine.Elevation.ArcGISImageElevationSource("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer", "Terrain 3D", ""));

        // If location has some specific or more detailed elevation layer, add it on top of the base layer
        foreach (string elevationData in MapManager.Instance.GetElevationData()) {
            arcGISm.Elevation.ElevationSources.Add(new Esri.GameEngine.Elevation.ArcGISImageElevationSource(elevationData, knownLocation + "_elevation", ""));
        }

        Scene3DViewArcGISMap.EnableExtent = true;

        var extentCenter = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, firstDroneFlightData.altitude, new ArcGISSpatialReference(4326));
        var extent = new ArcGISExtentCircle(extentCenter, 10000);

        arcGISm.ClippingArea = extent;

        Scene3DViewArcGISMap.View.Map = arcGISm;
    }
}


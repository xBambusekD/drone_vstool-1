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
using Esri.GameEngine;
using PimDeWitte.UnityMainThreadDispatcher;
using Esri.GameEngine.Map;
using Esri.GameEngine.Layers.Base;
using Esri.GameEngine.Elevation.Base;

public class GameManager : Singleton<GameManager> {

    public enum DisplayState {
        Scene3DView,
        Map2DView
    }

    public enum ConnectionStatus {
        Connected,
        Disconnected
    }

    public string ServerIP = "butcluster.ddns.net";
    public int ServerPort = 5555;
    public int RTMPPort = 1935;

    public string APIKey = "";
    public ArcGISMapComponent Scene3DViewArcGISMap;
    public ArcGISMapComponent Map2DViewArcGISMap;
    public ArcGISCameraComponent MainCamera;
    public ArcGISCameraComponent MinimapCamera;
    public GameObject Scene3DView;

    private bool carDetectorRunning = false;

    private bool mapCentered = false;
    private DroneFlightData firstDroneFlightData;

    [SerializeField]
    private MinimapUI minimapUI;
    [SerializeField]
    private ConnectionBar connectionBar;

    private ArcGISCameraControllerTouch sceneViewCameraController;
    private ArcGISCameraControllerTouch minimapCameraController;

    
    public float BuildingScale = 1.5f;
    public float BuildingAltitudeOffset = 25f;

    public DisplayState CurrentDisplayState {
        get; private set;
    }


    private void Start() {
        //LoadLastServerIP();

        sceneViewCameraController = MainCamera.GetComponent<ArcGISCameraControllerTouch>();
        minimapCameraController = MinimapCamera.GetComponent<ArcGISCameraControllerTouch>();

        StartCoroutine(InitSceneView());
    }

    private void LoadLastServerIP() {
        ServerIP = PlayerPrefs.GetString("serverIP", null);
        connectionBar.SetConnectionStatus(ConnectionStatus.Disconnected);
        if (!string.IsNullOrEmpty(ServerIP)) {
            //WebSocketManager.Instance.ConnectToServer(ServerIP, ServerPort);
            connectionBar.SetServerIP(ServerIP);
        }
    }

    public void SaveServerIP(string serverIP) {
        ServerIP = serverIP;
        PlayerPrefs.SetString("serverIP", ServerIP);
        //WebSocketManager.Instance.ConnectToServer(ServerIP, ServerPort);
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
        //WebSocketManager.Instance.SendDroneListRequest();
        connectionBar.SetConnectionStatus(ConnectionStatus.Connected);
    }

    public void HandleConnectionFailed() {
        connectionBar.SetConnectionStatus(ConnectionStatus.Disconnected);
    }

    public void CenterMap(DroneFlightData flightData) {
        if (!mapCentered) {
            mapCentered = true;
            firstDroneFlightData = flightData;

            //DestroyCurrentArcGISMap();

            //Scene3DViewArcGISMap = Scene3DView.AddComponent<ArcGISMapComponent>();

            // Set scene view map and load all possible 3d structures and layers
            Scene3DViewArcGISMap.OriginPosition = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, firstDroneFlightData.altitude, new ArcGISSpatialReference(4326));
            Scene3DViewArcGISMap.MapType = ArcGISMapType.Global;
            Scene3DViewArcGISMap.MapTypeChanged += new ArcGISMapComponent.MapTypeChangedEventHandler(CreateArcGISMap);
            CreateArcGISMap();

            // Set 2d minimap and center it to the position of the first drone
            Map2DViewArcGISMap.OriginPosition = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, 0, new ArcGISSpatialReference(4326));
            Map2DViewArcGISMap.MapType = ArcGISMapType.Global;

            // Center the main camera
            ArcGISLocationComponent cameraLocationComponent = MainCamera.GetComponent<ArcGISLocationComponent>();
            cameraLocationComponent.enabled = true;
            cameraLocationComponent.Position = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, firstDroneFlightData.altitude + 50, new ArcGISSpatialReference(4326));
            //cameraLocationComponent.Rotation = new ArcGISRotation(65, 68, 0);

            // Center the minimap camera
            ArcGISLocationComponent minimapCameraLocationComponent = MinimapCamera.GetComponent<ArcGISLocationComponent>();
            minimapCameraLocationComponent.enabled = true;
            minimapCameraLocationComponent.Position = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, 500, new ArcGISSpatialReference(4326));

        }
    }


    private void DestroyCurrentArcGISMap() {
        Scene3DViewArcGISMap.enabled = false;
        DestroyImmediate(Scene3DViewArcGISMap);
        //DestroyImmediate(Scene3DViewArcGISMap.GetComponentInChildren<ArcGISRendererComponent>().gameObject);
    }


    public void CreateArcGISMap() {
        Debug.Log("CREATE ARCGIS MAP");

        ArcGISMap arcGISm = Scene3DViewArcGISMap.View.Map;

        //ArcGISMap arcGISm = new ArcGISMap(Scene3DViewArcGISMap.MapType);
        //arcGISm.Basemap = new ArcGISBasemap(ArcGISBasemapStyle.ArcGISImagery, APIKey);
             
        //arcGISm.LoadStatusChanged += OnArcGISLoadStatusChanged;

        // Check whether the drone is flying in some known location
        MapManager.Location knownLocation = MapManager.Instance.GetKnownLocation(firstDroneFlightData.gps);

        // Load Scene Layer data for the known location
        foreach (string layerData in MapManager.Instance.Get3DObjectSceneLayerData()) {
            ArcGISLayer layer = IsLayerInMap(layerData);
            if (layer != null) {
                layer.IsVisible = true;
            } else {
                arcGISm.Layers.Add(new Esri.GameEngine.Layers.ArcGIS3DObjectSceneLayer(layerData, knownLocation + "_3DObjectSceneLayer", 1.0f, true, ""));
            }
        }

        // Add base elevation layer
        //arcGISm.Elevation.ElevationSources.Add(new Esri.GameEngine.Elevation.ArcGISImageElevationSource("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer", "Terrain 3D", ""));

        // If location has some specific or more detailed elevation layer, add it on top of the base layer
        foreach (string elevationData in MapManager.Instance.GetElevationData()) {
            ArcGISElevationSource elevationSource = IsElevationSourceInMap(elevationData);
            if (elevationSource != null) {
                elevationSource.IsEnabled = true;
            } else {
                arcGISm.Elevation.ElevationSources.Add(new Esri.GameEngine.Elevation.ArcGISImageElevationSource(elevationData, knownLocation + "_elevation", ""));
            }
        }

        Scene3DViewArcGISMap.EnableExtent = true;

        var extentCenter = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, firstDroneFlightData.altitude, new ArcGISSpatialReference(4326));
        var extent = new ArcGISExtentCircle(extentCenter, 10000);

        arcGISm.ClippingArea = extent;

        Scene3DViewArcGISMap.View.Map = arcGISm;

        //ArcGISRendererComponent arcGISRenderer = Scene3DViewArcGISMap.GetComponentInChildren<ArcGISRendererComponent>();
        //arcGISRenderer.gameObject.AddComponent<BuildingHeightCorrector>();
    }

    private ArcGISLayer IsLayerInMap(string layerData) {
        ArcGISMap map = Scene3DViewArcGISMap.View.Map;        
        int i_max = (int)map.Layers.GetSize();
        for (int i = 0; i < i_max; i++) {
            ArcGISLayer layer = map.Layers.At((ulong) i);

            if (layer.Source.Equals(layerData)) {
                return layer;
            }
        }
        return null;
    }

    private ArcGISElevationSource IsElevationSourceInMap(string elevationData) {
        ArcGISMap map = Scene3DViewArcGISMap.View.Map;
        int i_max = (int) map.Elevation.ElevationSources.GetSize();
        for (int i = 0; i < i_max; i++) {
            ArcGISElevationSource elevationSource = map.Elevation.ElevationSources.At((ulong) i);

            if (elevationSource.Source.Equals(elevationData)) {
                return elevationSource;
            }
        }
        return null;
    }

    //private void OnArcGISLoadStatusChanged(ArcGISLoadStatus loadStatus) {
    //    Debug.Log("ARCGIS – Load status changed to " + loadStatus);



    //    //if (loadStatus == ArcGISLoadStatus.Loaded) {
    //    //    UnityMainThreadDispatcher.Instance().Enqueue(CorrectBuildingHeight(BuildingScale, BuildingAltitudeOffset));
    //    //}
    //}

    //private IEnumerator CorrectBuildingHeight(float scale, float altitudeOffset) {
    //    yield return new WaitForSecondsRealtime(10f);
    //    Debug.Log("Correcting building heights");
    //    foreach (Transform go in Scene3DViewArcGISMap.GetComponentsInChildren<Transform>(includeInactive:true)) {
    //        if (go.name.StartsWith("ArcGISGameObject")) {
    //            if (go.GetComponent<MeshRenderer>().material.name.StartsWith("SceneNodeSurface")) {
    //                Debug.Log("Changing building: " + go.name);
    //                go.localScale = new Vector3(1f, scale, 1f);
    //                go.position += new Vector3(0f, altitudeOffset, 0f);
    //                go.gameObject.layer = LayerMask.NameToLayer("Buildings");
    //            }
    //        }
    //    }
    //    yield return null;
    //}

}


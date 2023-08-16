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

    //private ArcGISMapComponent arcGISMapComponent;
    //private ArcGISCameraComponent cameraComponent;
    //private ArcGISPoint geographicCoordinates = new ArcGISPoint(-74.054921, 40.691242, 3000, ArcGISSpatialReference.WGS84());
    //private ArcGISPoint geographicCoordinates = new ArcGISPoint(-74.054921, 40.691242, 3000, ArcGISSpatialReference.WGS84());

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
        //InvokeRepeating("ListAllGameObjects", 0f, 20f);

        sceneViewCameraController = MainCamera.GetComponent<ArcGISCameraControllerTouch>();
        minimapCameraController = MinimapCamera.GetComponent<ArcGISCameraControllerTouch>();

        StartCoroutine(InitSceneView());
    }

    private IEnumerator InitSceneView() {
        yield return new WaitForEndOfFrame();
        Open3DSceneView();
    }

    private void Update() {
        if (Keyboard.current[Key.C].wasPressedThisFrame) {
            carDetectorRunning = !carDetectorRunning;
            foreach (KeyValuePair<string, Drone> drone in DroneManager.Instance.Drones) {
                WebSocketManager.Instance.SendCarDetectionRequest(drone.Key, carDetectorRunning);
            }
        }
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

            Scene3DViewArcGISMap.OriginPosition = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, firstDroneFlightData.altitude, new ArcGISSpatialReference(4326));
            Scene3DViewArcGISMap.MapType = Esri.GameEngine.Map.ArcGISMapType.Local;
            Scene3DViewArcGISMap.MapTypeChanged += new ArcGISMapComponent.MapTypeChangedEventHandler(CreateArcGISMap);

            var cameraLocationComponent = MainCamera.GetComponent<ArcGISLocationComponent>();
            cameraLocationComponent.Position = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, firstDroneFlightData.altitude + 50, new ArcGISSpatialReference(4326));
            cameraLocationComponent.Rotation = new ArcGISRotation(65, 68, 0);
        }
    }

    //private void CreateArcGISMapComponent() {
    //    arcGISMapComponent = FindObjectOfType<ArcGISMapComponent>();

    //    if (!arcGISMapComponent) {
    //        GameObject arcGISMapGameObject = new GameObject("ArcGISMap");
    //        arcGISMapComponent = arcGISMapGameObject.AddComponent<ArcGISMapComponent>();
    //    }

    //    arcGISMapComponent.OriginPosition = geographicCoordinates;
    //    arcGISMapComponent.MapType = Esri.GameEngine.Map.ArcGISMapType.Local;

    //    arcGISMapComponent.MapTypeChanged += new ArcGISMapComponent.MapTypeChangedEventHandler(CreateArcGISMap);
    //}

    public void CreateArcGISMap() {
        var arcGISm = new Esri.GameEngine.Map.ArcGISMap(Scene3DViewArcGISMap.MapType);
        arcGISm.Basemap = new Esri.GameEngine.Map.ArcGISBasemap(Esri.GameEngine.Map.ArcGISBasemapStyle.ArcGISImagery, APIKey);

        //arcGISm.Elevation = new Esri.GameEngine.Map.ArcGISMapElevation(new Esri.GameEngine.Elevation.ArcGISImageElevationSource("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer", "Terrain 3D", ""));
        //arcGISm.Elevation = new Esri.GameEngine.Map.ArcGISMapElevation(new Esri.GameEngine.Elevation.ArcGISImageElevationSource("https://gis.brno.cz/ags1/rest/services/OMI/omi_dtm_2019_wgs_1m_e/ImageServer", "BrnoElevation", ""));

        //var layer_1 = new Esri.GameEngine.Layers.ArcGISImageLayer("https://tiles.arcgis.com/tiles/nGt4QxSblgDfeJn9/arcgis/rest/services/UrbanObservatory_NYC_TransitFrequency/MapServer", "MyLayer_1", 1.0f, true, "");
        //arcGISMap.Layers.Add(layer_1);

        //var layer_2 = new Esri.GameEngine.Layers.ArcGISImageLayer("https://tiles.arcgis.com/tiles/nGt4QxSblgDfeJn9/arcgis/rest/services/New_York_Industrial/MapServer", "MyLayer_2", 1.0f, true, "");
        //arcGISMap.Layers.Add(layer_2);

        //var layer_3 = new Esri.GameEngine.Layers.ArcGISImageLayer("https://tiles.arcgis.com/tiles/4yjifSiIG17X0gW4/arcgis/rest/services/NewYorkCity_PopDensity/MapServer", "MyLayer_3", 1.0f, true, "");
        //arcGISMap.Layers.Add(layer_3);

        //var buildingLayer = new Esri.GameEngine.Layers.ArcGIS3DObjectSceneLayer("https://gis.brno.cz/ags1/rest/services/Hosted/3D_budovy_LOD2_WM/SceneServer", "Building Layer", 1.0f, true, "");
        //ArcGISMap.Layers.Add(buildingLayer);

        Scene3DViewArcGISMap.EnableExtent = true;

        var extentCenter = new ArcGISPoint(firstDroneFlightData.gps.longitude, firstDroneFlightData.gps.latitude, firstDroneFlightData.altitude, new ArcGISSpatialReference(4326));
        var extent = new ArcGISExtentCircle(extentCenter, 10000);

        arcGISm.ClippingArea = extent;

        Scene3DViewArcGISMap.View.Map = arcGISm;
    }

    //private void CreateArcGISCamera() {
    //    cameraComponent = Camera.main.gameObject.GetComponent<ArcGISCameraComponent>();

    //    if (!cameraComponent) {
    //        var cameraGameObject = Camera.main.gameObject;

    //        cameraGameObject.transform.SetParent(arcGISMapComponent.transform, false);

    //        cameraComponent = cameraGameObject.AddComponent<ArcGISCameraComponent>();

    //        cameraGameObject.AddComponent<ArcGISCameraControllerComponent>();

    //        cameraGameObject.AddComponent<ArcGISRebaseComponent>();
    //    }

    //    var cameraLocationComponent = cameraComponent.GetComponent<ArcGISLocationComponent>();
    //    cameraLocationComponent.Position = geographicCoordinates;
    //    cameraLocationComponent.Rotation = new ArcGISRotation(65, 68, 0);

    //    if (!cameraLocationComponent) {
    //        cameraLocationComponent = cameraComponent.gameObject.AddComponent<ArcGISLocationComponent>();

    //        cameraLocationComponent.Position = geographicCoordinates;
    //        cameraLocationComponent.Rotation = new ArcGISRotation(65, 68, 0);
    //    }
    //}
}


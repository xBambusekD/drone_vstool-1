using System;
using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Elevation.Base;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract class Drone : InteractiveObject, IPointerNotifier {

    public DroneFlightData FlightData {
        get; set;
    }

    public DroneStaticData StaticData {
        get; set;
    }

    public VehicleDataRenderer VehicleRenderer;
    public MeshRenderer VideoScreen;

    public Transform DroneVideoScreen;
    public Transform ThirdPersonView;

    public ListItemButton DroneListItem;

    public GameObject Drone2DPrefab;
    private Drone2D drone2DRepresentation;

    private Queue<DroneFlightData> flightDataBuffer = new Queue<DroneFlightData>();
    private float flightDataDelay = 0f;
    public int FlightDataBufferSize = 0;
    private float dataFrameRate = 0.1f;
    private float lastMessageReceiveTime = 0f;

    private bool decoded = false;

    private Texture2D JpegPlayerTexture;

    public Material OcclusionMaterial;

    // Store the drone's current position in world coordinates
    private Vector3 dronePosition = Vector3.zero;

    private DateTime lastTime;

    public RawImage ExperimentDroneVideoFrame;


    private IEnumerator PlayReceivedFlightData() {
        while (true) {
            if (flightDataBuffer.Count > 0) {
                DroneFlightData flightData = flightDataBuffer.Dequeue();
                UpdateDroneFlightData(flightData);
                yield return new WaitForSeconds(dataFrameRate);
            } else {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(flightDataDelay);
            flightDataDelay = 0f;
        }
    }

    public virtual void InitDrone(DroneStaticData staticData) {
        drone2DRepresentation = Instantiate(Drone2DPrefab, MapManager.Instance.CurrentMapType == MapManager.MapType.ArcGIS ? GameManager.Instance.Map2DViewArcGISMap.transform : GameManager.Instance.Map2DViewCesium).GetComponent<Drone2D>();
        drone2DRepresentation.InitDrone();

        MissionManager.Instance.ChangeTargetFaceCamera(FPVOnlyCamera.transform);
        MissionManager.Instance.ChangeTarget(transform);

        JpegPlayerTexture = new Texture2D(1, 1);
    }

    public void DeliverNewFlightData(DroneFlightData flightData) {
        dataFrameRate = Time.time - lastMessageReceiveTime;
        lastMessageReceiveTime = Time.time;
        Debug.Log(dataFrameRate);
        flightDataBuffer.Enqueue(flightData);
        FlightDataBufferSize = flightDataBuffer.Count;
    }

    public void UpdateDroneFlightData(DroneFlightData flightData, bool droneCopy = false) {
        //if (DroneManager.Instance.UseKalman) {
        //    UpdateDroneByKalman(flightData);
        //} else {
        //    UpdateDroneByGPS(flightData);
        //}

        UpdateDroneByGPS(flightData);
    }

    private void UpdateDroneByGPS(DroneFlightData flightData) {
        FlightData = flightData;

        UpdateDroneLocation(flightData);

        DroneVideoScreen.localRotation = Quaternion.Euler(-(float) flightData.gimbal_orientation.pitch, (float) (flightData.aircraft_orientation.yaw + flightData.gimbal_orientation.yaw_relative), -(float) flightData.gimbal_orientation.roll);
        ThirdPersonView.localRotation = Quaternion.Euler(0f, (float) flightData.aircraft_orientation.yaw, 0f);

        drone2DRepresentation.UpdateFlightData(flightData);

        DroneListItem.UpdateHeight(flightData.altitude);
        DroneListItem.UpdateDistance(Vector3.Distance(Camera.main.transform.position, this.transform.position));

        if (flightData.frame != "") {
            byte[] frame = Convert.FromBase64String(flightData.frame);

            JpegPlayerTexture.LoadImage(frame);

            if (ArCameraBackground != null) {
                ArCameraBackground.texture = JpegPlayerTexture;
            }
            VideoScreen.material.mainTexture = JpegPlayerTexture;
            OcclusionMaterial.SetTexture("_CameraFeedTexture", JpegPlayerTexture);

            if (ExperimentDroneVideoFrame != null) {
                ExperimentDroneVideoFrame.texture = JpegPlayerTexture;
            }
        }
    }

    protected abstract void UpdateDroneLocation(DroneFlightData flightData);

    public abstract GPS GetDroneLocation();

    //private void UpdateDroneByKalman(DroneFlightData flightData) {
    //    //// Extract velocity and orientation
    //    //Vector3 velocity = new Vector3(
    //    //    (float) flightData.aircraft_velocity.velocity_x,
    //    //    (float) flightData.aircraft_velocity.velocity_y,
    //    //    (float) flightData.aircraft_velocity.velocity_z
    //    //);

    //    // Extract velocity and orientation
    //    Vector3 velocity = new Vector3(
    //        -(float) flightData.aircraft_velocity.velocity_y,
    //        -(float) flightData.aircraft_velocity.velocity_z,
    //        (float) flightData.aircraft_velocity.velocity_x
    //    );

    //    float roll = (float) flightData.aircraft_orientation.roll;
    //    float pitch = (float) flightData.aircraft_orientation.pitch;
    //    float yaw = (float) flightData.aircraft_orientation.yaw;

    //    // Convert orientation to a quaternion (from drone's local space to world space)
    //    Quaternion rotation = Quaternion.Euler(-pitch, yaw, -roll);

    //    // Transform velocity from local to world coordinates
    //    Vector3 worldVelocity = rotation * velocity;

    //    // Calculate time difference between the current and last timestamp
    //    DateTime currentTime = DateTime.Parse(flightData.timestamp);
    //    TimeSpan timeDifference = currentTime - lastTime;
    //    float deltaTime = (float) timeDifference.TotalSeconds;
    //    lastTime = currentTime;

    //    // Update the drone's position in world coordinates
    //    dronePosition += worldVelocity * deltaTime;

    //    // Update the drone's position in Unity
    //    //hpTransform.LocalPosition = dronePosition;
    //    transform.localPosition = dronePosition;

    //    // Set rotation for the drone's 3D model (optional if you want to visualize its orientation)
    //    transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

    //    // Update video feed orientation
    //    DroneVideoScreen.localRotation = Quaternion.Euler(
    //        -(float) flightData.gimbal_orientation.pitch,
    //        (float) flightData.aircraft_orientation.yaw + (float) flightData.gimbal_orientation.yaw_relative,
    //        -(float) flightData.gimbal_orientation.roll
    //    );

    //    // Update third-person view (if needed)
    //    ThirdPersonView.localRotation = Quaternion.Euler(0f, yaw, 0f);

    //    // Update drone 2D representation and UI
    //    drone2DRepresentation.UpdateFlightData(flightData);
    //    DroneListItem.UpdateHeight(flightData.altitude);
    //    DroneListItem.UpdateDistance(Vector3.Distance(Camera.main.transform.position, this.transform.position));

    //    // Handle frame data for the video feed
    //    if (flightData.frame != "") {
    //        byte[] frame = Convert.FromBase64String(flightData.frame);
    //        JpegPlayerTexture.LoadImage(frame);

    //        if (ArCameraBackground != null) {
    //            ArCameraBackground.texture = JpegPlayerTexture;
    //        }

    //        VideoScreen.material.mainTexture = JpegPlayerTexture;
    //        OcclusionMaterial.SetTexture("_CameraFeedTexture", JpegPlayerTexture);
    //    }
    //}

    //private void UpdateDroneByKalmanHP(DroneFlightData flightData) {
    //    //// Extract velocity and orientation
    //    //Vector3 velocity = new Vector3(
    //    //    (float) flightData.aircraft_velocity.velocity_x,
    //    //    (float) flightData.aircraft_velocity.velocity_y,
    //    //    (float) flightData.aircraft_velocity.velocity_z
    //    //);

    //    // Extract velocity and orientation
    //    Vector3 velocity = new Vector3(
    //        -(float) flightData.aircraft_velocity.velocity_y,
    //        -(float) flightData.aircraft_velocity.velocity_z,
    //        (float) flightData.aircraft_velocity.velocity_x
    //    );

    //    float roll = (float) flightData.aircraft_orientation.roll;
    //    float pitch = (float) flightData.aircraft_orientation.pitch;
    //    float yaw = (float) flightData.aircraft_orientation.yaw;

    //    // Convert orientation to a quaternion (from drone's local space to world space)
    //    Quaternion rotation = Quaternion.Euler(-pitch, yaw, -roll);

    //    // Transform velocity from local to world coordinates
    //    Vector3 worldVelocity = rotation * velocity;

    //    // Calculate time difference between the current and last timestamp
    //    DateTime currentTime = DateTime.Parse(flightData.timestamp);
    //    TimeSpan timeDifference = currentTime - lastTime;
    //    float deltaTime = (float) timeDifference.TotalSeconds;
    //    lastTime = currentTime;

    //    // Update the drone's position in world coordinates
    //    dronePosition += worldVelocity * deltaTime;

    //    // Update the drone's position in Unity
    //    //hpTransform.LocalPosition = dronePosition;
    //    //hpTransform.LocalPosition = new Unity.Mathematics.double3((double) dronePosition.x, (double) dronePosition.y, (double) dronePosition.z);

    //    // Set rotation for the drone's 3D model (optional if you want to visualize its orientation)
    //    //hpTransform.LocalRotation = Quaternion.Euler(0f, yaw, 0f);

    //    // Update video feed orientation
    //    DroneVideoScreen.localRotation = Quaternion.Euler(
    //        -(float) flightData.gimbal_orientation.pitch,
    //        (float) flightData.aircraft_orientation.yaw + (float) flightData.gimbal_orientation.yaw_relative,
    //        -(float) flightData.gimbal_orientation.roll
    //    );

    //    // Update third-person view (if needed)
    //    ThirdPersonView.localRotation = Quaternion.Euler(0f, yaw, 0f);

    //    // Update drone 2D representation and UI
    //    drone2DRepresentation.UpdateFlightData(flightData);
    //    DroneListItem.UpdateHeight(flightData.altitude);
    //    DroneListItem.UpdateDistance(Vector3.Distance(Camera.main.transform.position, this.transform.position));

    //    // Handle frame data for the video feed
    //    if (flightData.frame != "") {
    //        byte[] frame = Convert.FromBase64String(flightData.frame);
    //        JpegPlayerTexture.LoadImage(frame);

    //        if (ArCameraBackground != null) {
    //            ArCameraBackground.texture = JpegPlayerTexture;
    //        }

    //        VideoScreen.material.mainTexture = JpegPlayerTexture;
    //        OcclusionMaterial.SetTexture("_CameraFeedTexture", JpegPlayerTexture);
    //    }
    //}

    public override Texture GetCameraTexture() {
        return JpegPlayerTexture;
    }

    public void UpdateDroneVehicleData(DroneVehicleData vehicleData) {
        //if(PipelinePlayer == null && VehicleRenderer == null) return;
        //VehicleRenderer.gameObject.SetActive(true);
        //VehicleRenderer.UpdateVehicleData(vehicleData);
    }

    public override void Highlight(bool highlight) {
        base.Highlight(highlight);
        drone2DRepresentation.Highlight(highlight);
    }

    public override void ChangeFlightDataDelay(float delay) {
        flightDataDelay = delay;
    }

    public void OnClicked(IPointerNotifier.ClickObject clickObject) {
        if (clickObject == IPointerNotifier.ClickObject.DroneScreen) {
            DroneManager.Instance.SetActiveDrone(this);
            CameraManager.Instance.SetCurrentInteractiveObject(this);
            CameraManager.Instance.SetCameraView(CameraManager.CameraView.FirstPerson);
        }
    }

    public void OnDestroy() {
        Destroy(DroneListItem.gameObject);
        Destroy(drone2DRepresentation.gameObject);
    }

    public abstract void SetGPSOffset(GPS offset);
}

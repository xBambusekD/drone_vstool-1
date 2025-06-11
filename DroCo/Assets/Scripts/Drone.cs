using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Drone : MonoBehaviour {

    public DroneFlightData FlightData {
        get; set;
    }

    public DroneStaticData StaticData {
        get; set;
    }

    [SerializeField]
    private Transform DroneModel;

    [SerializeField]
    private MeshRenderer VideoScreenRenderer;

    [SerializeField]
    private Transform DroneVideoScreen;


    private Texture2D JpegPlayerTexture;

    // === Persistent state ===
    private Vector3 estimatedPosition = Vector3.zero;
    private DateTime? lastTimestamp = null;

    // Kalman filter state for velocity
    private Vector3 kalmanVelocityEstimate = Vector3.zero;
    private Vector3 kalmanVelocityError = new Vector3(1, 1, 1); // initial error
    private float kalmanProcessNoise = 0.125f;
    private float kalmanMeasurementNoise = 1f;


    public void InitDrone(DroneStaticData staticData) {
        JpegPlayerTexture = new Texture2D(1, 1);
    }


    public void UpdateDroneFlightData(DroneFlightData flightData) {
        FlightData = flightData;

        UpdateDroneLocation(flightData);

        DroneVideoScreen.localRotation = Quaternion.Euler(-(float) flightData.gimbal_orientation.pitch, (float) (flightData.aircraft_orientation.yaw + flightData.gimbal_orientation.yaw_relative), -(float) flightData.gimbal_orientation.roll);


        if (flightData.frame != null && flightData.frame.Length > 0) {
            JpegPlayerTexture.LoadImage(flightData.frame);

            VideoScreenRenderer.material.mainTexture = JpegPlayerTexture;
        }
    }

    private void UpdateDroneLocation(DroneFlightData flightData) {
        DroneModel.localRotation = Quaternion.Euler(-(float) flightData.aircraft_orientation.pitch, (float) flightData.aircraft_orientation.yaw, -(float) flightData.aircraft_orientation.roll);

        // Timestamp
        if (!DateTime.TryParse(flightData.timestamp, out DateTime parsedTime)) {
            Debug.LogWarning("Invalid timestamp format");
            return;
        }

        if (lastTimestamp == null) {
            lastTimestamp = parsedTime;
            return; // First frame — no delta yet
        }

        float deltaTime = (float) (parsedTime - lastTimestamp.Value).TotalSeconds;
        lastTimestamp = parsedTime;

        if (deltaTime <= 0f || deltaTime > 5f) {
            Debug.Log($"Skipped frame — unreasonable deltaTime: {deltaTime}");
            return;
        }

        // Drone velocity in drone frame (x = forward, y = right, z = up)
        Vector3 droneVelocity = new Vector3(
            (float) flightData.aircraft_velocity.velocity_x,
            (float) flightData.aircraft_velocity.velocity_y,
            (float) flightData.aircraft_velocity.velocity_z
        );

        // Kalman filter update
        Vector3 gain = ElementWiseDivide(kalmanVelocityError, kalmanVelocityError + Vector3.one * kalmanMeasurementNoise);
        kalmanVelocityEstimate += Vector3.Scale(gain, droneVelocity - kalmanVelocityEstimate);
        kalmanVelocityError = Vector3.Scale(Vector3.one - gain, kalmanVelocityError + Vector3.one * kalmanProcessNoise);

        // Convert from drone frame to Unity frame
        Vector3 unityVelocity = ConvertNEDToUnityFrame(kalmanVelocityEstimate);

        // Apply orientation to get world velocity
        Quaternion droneRotation = Quaternion.Euler(
            (float) flightData.aircraft_orientation.pitch,
            (float) flightData.aircraft_orientation.yaw,
            (float) flightData.aircraft_orientation.roll
        );
        Vector3 worldVelocity = droneRotation * unityVelocity;

        // Integrate position
        estimatedPosition += worldVelocity * deltaTime;

        // Apply position to drone model
        transform.localPosition = estimatedPosition;

        Debug.DrawLine(DroneModel.localPosition, DroneModel.localPosition + unityVelocity, Color.green, 0.1f);
    }

    // Convert velocity from NED (North-East-Down) to Unity (Right-Up-Forward)
    private Vector3 ConvertNEDToUnityFrame(Vector3 nedVelocity) {
        return new Vector3(
            nedVelocity.y,        // East → Unity X
            -nedVelocity.z,       // -Down → Unity Y
            nedVelocity.x         // North → Unity Z
        );
    }

    private Vector3 ElementWiseDivide(Vector3 a, Vector3 b) {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
}

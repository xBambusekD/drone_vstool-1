using System.Collections;
using System.Collections.Generic;
using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CesiumGlobeAnchor))]
public class Drone2DCesium : Drone2D {


    private CesiumGlobeAnchor GPSLocation;
    private LayerMask layerMask;

    public override void InitDrone() {
        base.InitDrone();

        layerMask = ~LayerMask.GetMask("Mission", "DroneScreen");

        GPSLocation = GetComponent<CesiumGlobeAnchor>();
        GPSLocation.enabled = true;
    }

    public override void UpdateFlightData(DroneFlightData flightData) {
        GPSLocation.longitudeLatitudeHeight = new double3(flightData.gps.longitude, flightData.gps.latitude, GetGroundHeight());
        droneImage.transform.localRotation = Quaternion.Euler(new Vector3(90f, (float)flightData.aircraft_orientation.yaw, 0));
    }

    public double GetGroundHeight() {
        if (Physics.Raycast(transform.position + new Vector3(0, 1000, 0), transform.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity, layerMask)) {
            if (hit.collider != null && hit.distance < 50000) {
                return GPSLocation.longitudeLatitudeHeight.z - (double) (hit.distance - 1000);
            }
        }
        return 500;
    }
}

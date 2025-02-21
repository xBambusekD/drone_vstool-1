using System.Collections;
using System.Collections.Generic;
using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CesiumGlobeAnchor))]
public class DroneCesium : Drone {

    public CesiumGlobeAnchor GPSLocation {
        get; private set;
    }

    public double3 GPSLocationNoOffset => new double3(GPSLocation.longitudeLatitudeHeight.x - gpsOffset.longitude, GPSLocation.longitudeLatitudeHeight.y - gpsOffset.latitude, GPSLocation.longitudeLatitudeHeight.z);

    private GPS gpsOffset = new GPS() { latitude = 0, longitude = 0 };

    private LayerMask layerMask;

    public override void InitDrone(DroneStaticData staticData) {
        base.InitDrone(staticData);

        layerMask = ~LayerMask.GetMask("Mission", "DroneScreen");

        GPSLocation = GetComponent<CesiumGlobeAnchor>();
        GPSLocation.enabled = true;
    }

    protected override void UpdateDroneLocation(DroneFlightData flightData) {
        GPSLocation.longitudeLatitudeHeight = new double3(flightData.gps.longitude + gpsOffset.longitude, flightData.gps.latitude + gpsOffset.latitude, flightData.altitude + 45f);
        //GPSLocation.longitudeLatitudeHeight = new double3(flightData.gps.longitude + gpsOffset.longitude, flightData.gps.latitude + gpsOffset.latitude, flightData.relative_altitude + GetGroundAltitude());
        DroneModel.localRotation = Quaternion.Euler(-(float) flightData.aircraft_orientation.pitch, (float) flightData.aircraft_orientation.yaw, -(float) flightData.aircraft_orientation.roll);
    }

    private double GetGroundAltitude() {
        if (Physics.Raycast(transform.position + new Vector3(0, 1000, 0), transform.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity, layerMask)) {
            if (hit.collider != null) {
                return GPSLocation.longitudeLatitudeHeight.z - (double) (hit.distance - 1000);
            }
        }
        return 0;
    }

    public override void SetGPSOffset(GPS offset) {
        base.SetGPSOffset(offset);

        gpsOffset = offset;
    }

    public override GPS GetDroneLocation() {
        return new GPS() { longitude = GPSLocationNoOffset.x, latitude = GPSLocationNoOffset.y };
    }
}

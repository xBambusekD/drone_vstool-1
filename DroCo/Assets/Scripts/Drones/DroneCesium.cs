using System.Collections;
using System.Collections.Generic;
using CesiumForUnity;
using UnityEngine;

[RequireComponent(typeof(CesiumGlobeAnchor))]
public class DroneCesium : Drone {

    public CesiumGlobeAnchor GPSLocation {
        get; private set;
    }

    public override void InitDrone(DroneStaticData staticData) {
        base.InitDrone(staticData);

        GPSLocation = GetComponent<CesiumGlobeAnchor>();
        GPSLocation.enabled = true;
    }

    protected override void UpdateDroneLocation(DroneFlightData flightData) {
        GPSLocation.longitudeLatitudeHeight = new Unity.Mathematics.double3(flightData.gps.longitude, flightData.gps.latitude, flightData.altitude + 45f);
        DroneModel.localRotation = Quaternion.Euler(-(float) flightData.aircraft_orientation.pitch, (float) flightData.aircraft_orientation.yaw, -(float) flightData.aircraft_orientation.roll);
    }

    public override void SetGPSOffset(GPS offset) {
        throw new System.NotImplementedException();
    }

    public override GPS GetDroneLocation() {
        return new GPS() { longitude = GPSLocation.longitudeLatitudeHeight.x, latitude = GPSLocation.longitudeLatitudeHeight.y };
    }
}

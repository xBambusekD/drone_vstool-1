using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using UnityEngine;

[RequireComponent(typeof(ArcGISLocationComponent))]
public class Drone2DArcGIS : Drone2D {

    private ArcGISLocationComponent GPSLocation;

    public override void InitDrone() {
        base.InitDrone();

        GPSLocation = GetComponent<ArcGISLocationComponent>();
        GPSLocation.enabled = true;
    }

    public override void UpdateFlightData(DroneFlightData flightData) {
        GPSLocation.Position = new ArcGISPoint(flightData.gps.longitude, flightData.gps.latitude, 0, new ArcGISSpatialReference(4326));
        GPSLocation.Rotation = new ArcGISRotation(flightData.aircraft_orientation.yaw, 0, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone {

    public GameObject DroneGameObject {
        get; set;
    }

    public DroneFlightData FlightData {
        get; set;
    }

    public Drone(GameObject droneGameObject, DroneFlightData flightData) {
        DroneGameObject = droneGameObject;
        FlightData = flightData;
    }

    public void UpdateDroneFlightData(DroneFlightData flightData) {
        FlightData = flightData;
        if (double.IsNaN(FlightData.Latitude) || double.IsNaN(FlightData.Longitude)) {
            return;
        }

        Mapbox.Utils.Vector2d mapboxPosition = new Mapbox.Utils.Vector2d(FlightData.Latitude, FlightData.Longitude);
        Vector3 position3d = MapController.Instance.Map.GeoToWorldPosition(mapboxPosition, false);
        if (FlightData.DroneId == "DJI-Mavic2") {
            float groundAltitude = MapController.Instance.Map.QueryElevationInUnityUnitsAt(MapController.Instance.Map.WorldToGeoPosition(position3d));
            position3d.y = groundAltitude + (float) FlightData.Altitude;
        } else {
            position3d.y = (float) FlightData.Altitude;
        }
        DroneGameObject.transform.position = position3d;
        DroneGameObject.transform.eulerAngles = new Vector3((float)FlightData.Pitch, (float)FlightData.Yaw + (float)FlightData.Compass, (float)FlightData.Roll);
    }

}

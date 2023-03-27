using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DroneFlightData {
    public string ClientID;
    public double Altitude;
    public double Latitude;
    public double Longitude;
    public double Pitch;
    public double Roll;
    public double Yaw;
    public double Compass;

    public DroneFlightData() {
        ClientID = "unset";
        Altitude = 0;
        Latitude = 0;
        Longitude = 0;
        Pitch = 0;
        Roll = 0;
        Yaw = 0;
        Compass = 0;
    }

    public void SetData(double height, double latitude, double longitute, double pitch, double roll, double yaw, double compass) {
        Altitude = height;
        Latitude = latitude;
        Longitude = longitute;
        Pitch = pitch;
        Roll = roll;
        Yaw = yaw;
        Compass = compass;
    }
}

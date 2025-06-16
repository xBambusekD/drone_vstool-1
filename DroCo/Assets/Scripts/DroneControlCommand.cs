using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DroneControlCommand {

    public double pitch;
    public double roll;
    public double yaw;
    public double throttle;


    public DroneControlCommand() {
        pitch = 0;
        roll = 0;
        yaw = 0;
        throttle = 0;
    }

    public override string ToString() {
        return $"{{\"pitch\":{pitch}, \"roll\":{roll}, \"yaw\":{yaw}, \"throttle\":{throttle}}}";
    }
}


[Serializable]
public class EnableDroneControl {

    public bool enable;

    public EnableDroneControl() {
        enable = false;
    }

    public override string ToString() {
        return $"{{\"enable\":{enable}}}";
    }
}

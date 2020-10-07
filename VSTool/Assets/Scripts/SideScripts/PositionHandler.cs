using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.Messages;
public class PositionHandler : MonoBehaviour
{
    public DroneController droneController;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if(RosSharp.RosBridgeClient.GlobalPositionSubscriber.MessageRecieved)
        {
            droneController.setAltitudeOffset();
            Invoke("NewMethod",0.1f);
            RosSharp.RosBridgeClient.GlobalPositionSubscriber.MessageRecieved = false;
        }
    }

    private void NewMethod()
    {
        transform.position = Drones.drones[MissionHandler.activeDrone].transform.position;
        transform.rotation = Drones.drones[MissionHandler.activeDrone].transform.rotation;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y-90,transform.eulerAngles.z);
    }
}

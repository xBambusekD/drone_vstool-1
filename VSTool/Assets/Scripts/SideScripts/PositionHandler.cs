using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.Messages;
public class PositionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if(RosSharp.RosBridgeClient.GlobalPositionSubscriber.MessageRecieved)
        {
            Invoke("NewMethod",1);
            RosSharp.RosBridgeClient.GlobalPositionSubscriber.MessageRecieved = false;
        }
        // if (Input.GetKeyUp("o"))
        // {
        //     Invoke("NewMethod1",1);
        // }
    }

    private void NewMethod1()
    {
        transform.position = new Vector3(transform.position.x, Drones.drones[MissionHandler.activeDrone].DroneGameObject.transform.position.y, transform.position.z);
    }

    private void NewMethod()
    {
        transform.position = Drones.drones[MissionHandler.activeDrone].DroneGameObject.transform.position;
        transform.rotation = Drones.drones[MissionHandler.activeDrone].DroneGameObject.transform.rotation;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y-90,transform.eulerAngles.z);
    }
}

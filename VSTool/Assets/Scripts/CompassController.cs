using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassController : MonoBehaviour
{
    public Transform DroneModel;


    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, DroneModel.eulerAngles.x);
    }

}

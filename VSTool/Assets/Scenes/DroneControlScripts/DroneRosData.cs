using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Messages;

class DroneRosData: AbstractDroneData
{
    private GameObject rosConnector;
    private GlobalPositionSubscriber positionSubs;
    private CompassSubscriber compassSubs;
    private ImuSubscriber imuSubs;
    public bool offset = false;
    private RosSharp.RosBridgeClient.Messages.Imu imuMes;
    private Quaternion pitchRoll;


    public DroneRosData(AbstractMap map, Vector3 defPos) : base(map, defPos)
    {

    }

    public void setRosConnector(GameObject rosCon)
    {
        rosConnector = rosCon;
        positionSubs = rosCon.GetComponent("GlobalPositionSubscriber") as GlobalPositionSubscriber;
        compassSubs = rosCon.GetComponent("CompassSubscriber") as CompassSubscriber;
        imuSubs = rosCon.GetComponent("ImuSubscriber") as ImuSubscriber;
    }

    public override void update()
    {
        RosSharp.RosBridgeClient.Messages.NavSatFix positionMes = positionSubs.position;
        rotation = compassSubs.orientation;
        imuMes = imuSubs.imuData;

        if (imuMes != null && positionMes != null) //tady by to chtelo spis overovat,zda je pripojeno
        {
            RosSharp.RosBridgeClient.Messages.Geometry.Quaternion imuQuat = imuMes.orientation;

            //pitchRoll =  RosSharp.TransformExtensions.Ros2Unity(imuQuat); z nejakho duvodu nefunguje, tak vezmu primo kod metody
            pitchRoll = new Quaternion(imuQuat.y, -imuQuat.z, -imuQuat.x, imuQuat.w);

            if (positionMes.latitude == 0 && positionMes.longitude == 0 && positionMes.altitude == 0) // no GPS fix
            {
                //position = new Vector3(0, 0, 0);
                Debug.Log("no GPS fix");
            }
            else
            {
                position = Map.GeoToWorldPosition(new Mapbox.Utils.Vector2d(positionMes.latitude, positionMes.longitude), false);

                //set new AltitudeOffset
                if (offset)
                {
                    float altitude = (float)positionMes.altitude;
                    PlayerPrefs.SetFloat("AltitudeOffset", (getGroundAltitude() - altitude));
                    offset = false;
                }

                float altitudeOffset = PlayerPrefs.GetFloat("AltitudeOffset");
                // position.y = (float)positionMes.altitude + altitudeOffset; //doplnim vysku
                position.y = (float)positionMes.altitude +altitudeOffset;
            }

           // Debug.Log("DroneRosData: lat: " + positionMes.latitude + ", lon: " + positionMes.longitude + ", alt: "+ positionMes.altitude + ", compass: " + rotation+ ", UMU eulerAngles: " + pitchRoll.eulerAngles);
        }
    }


    public override void reset(Vector3 pos, Vector3 rot)
    {

    }
    

    public override Vector3 GetPosition()
    {

        return position;
    }

    public override Vector3 GetRotation()
    {
        return new Vector3(0, rotation, 0);
    }

    public override Vector3 GetPitchRoll()
    {
        Vector3 angles = pitchRoll.eulerAngles;
        angles.y = 0;

        return Quaternion.AngleAxis(-90, Vector3.up) *  angles; //nesedí osy imu a UNITy, proto je to potřeba otočit
    }

    public override Vector3 GetCameraRotation()
    {
        return new Vector3(0, 0, 0);
    }
}

/*
This message class is generated automatically with 'SimpleMessageGenerator' of ROS#
*/

using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Navigation;
using RosSharp.RosBridgeClient.Messages.Sensor;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.Messages.Actionlib;

namespace RosSharp.RosBridgeClient.Messages
{
    public class NavSatFix : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "sensor_msgs/NavSatFix";

        public NavSatStatus status;
        public double latitude;
        public double longitude;
        public double altitude;
        public uint position_covariance_type;

        public NavSatFix()
        {
            status = new NavSatStatus();
            latitude = new double();
            longitude = new double();
            altitude = new double();
            position_covariance_type = 0;
        }
    }
}


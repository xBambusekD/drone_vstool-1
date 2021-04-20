/*
This message class is generated automatically with 'SimpleMessageGenerator' of ROS#
*/ 

using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Navigation;
using RosSharp.RosBridgeClient.Messages.Sensor;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.Messages.Actionlib;
//using UnityEngine;



namespace RosSharp.RosBridgeClient.Messages
{
    public class Imu : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "sensor_msgs/Imu";

        public Header header;
        public Quaternion orientation;
        public float[] orientation_covariance;
        public Vector3 angular_velocity;
        public float[] angular_velocity_covariance;
        public Vector3 linear_acceleration;
        public float[] linear_acceleration_covariance;

        public Imu()
        {
            header = new Header();
            orientation = new Quaternion();
            orientation_covariance = new float[9];
            angular_velocity = new Vector3();
            angular_velocity_covariance = new float[9];
            linear_acceleration = new Vector3();
            linear_acceleration_covariance = new float[9];
        }
    }
}


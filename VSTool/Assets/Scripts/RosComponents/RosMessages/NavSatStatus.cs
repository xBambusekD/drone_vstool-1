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
public class NavSatStatus : Message
{
[JsonIgnore]
public const string RosMessageName = "sensor_msgs/NavSatStatus";

public int status;
public int service;

public NavSatStatus()
{
status = new int();
service = new int();
}
}
}


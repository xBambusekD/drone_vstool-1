using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    interface IDroneImageSubscriber
    {
        bool OptimalizeForProjector { get; set; }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.Messages;

namespace RosSharp.RosBridgeClient
{
    public class CompassSubscriber : Subscriber<Messages.Float64>
    {
        public float orientation;

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(Messages.Float64 position)
        {
            orientation = position.data;
            //Debug.Log("compass: "+position.data);
        }
    }
}

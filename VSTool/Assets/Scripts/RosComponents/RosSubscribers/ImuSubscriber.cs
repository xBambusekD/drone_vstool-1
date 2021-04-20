using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.Messages;

namespace RosSharp.RosBridgeClient
{
    public class ImuSubscriber : Subscriber<Messages.Imu>
    {
        public Messages.Imu imuData;

        private bool isMessageReceived;
        private MyRosConnector droneRosConnector;

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(Messages.Imu data)
        {
            imuData = data;
            isMessageReceived = false;
        }

        private void Update()
        {
            if (isMessageReceived)
            {
                isMessageReceived=false;
                if (droneRosConnector != null) droneRosConnector.RecievedMessageSetTime(); //musi se volat z updatu, jinak neni definovany cas
            }
        }
    }
}

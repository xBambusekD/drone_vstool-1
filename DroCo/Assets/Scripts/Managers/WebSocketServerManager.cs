using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketServer;

namespace WebSocketServer {
    public class WebSocketServerManager : WebSocketServer {

        [Serializable]
        private class Message<T> {
            public string type;
            public T data;
        }

        [Serializable]
        private class Hello {
            public string ctype;
            public string drone_name;
            public string serial;
        }

        [Serializable]
        private class HelloResponse {
            public string client_id;
            public string rtmp_port;
        }

        private bool handshake_done = false;

        public override void OnOpen(WebSocketConnection connection) {
            Debug.Log("New connection opened");
            
        }

        public override void OnClose(WebSocketConnection connection) {
            Debug.Log("Connection close");
        }

        public override void OnMessage(WebSocketMessage message) {

            Debug.Log(message.data);
            Message<string> msg = JsonUtility.FromJson<Message<string>>(message.data);
            if (msg.type == "hello") {
                DoHandshake(message.connection.id, JsonUtility.FromJson<Message<Hello>>(message.data));
            } else if (handshake_done && msg.type == "data_broadcast") {

                 Message<DroneFlightData> dsfdr = JsonUtility.FromJson<Message<DroneFlightData>>(message.data);

                //DroneManager.Instance.HandleReceivedDroneData(dsfdr.data);
            } else {
                Debug.LogError("Unknown data received! " + message.data);
            }
            
        }

        public override void OnError(WebSocketConnection connection) {
            Debug.Log("Connection error");
        }

        private void DoHandshake(string clientID, Message<Hello> droneData) {
            handshake_done = true;

            Message<HelloResponse> helloResponse = new Message<HelloResponse>
                { data = new HelloResponse() };
            helloResponse.type = "hello_resp";
            helloResponse.data.client_id = "abc";
            helloResponse.data.rtmp_port = "1935";

            string msg = JsonUtility.ToJson(helloResponse);
            SendMessageToClient(msg);

            DroneStaticData newDrone = new DroneStaticData {
                client_id = "abc",
                drone_name = droneData.data.drone_name,
                serial = droneData.data.serial
            };
            DroneManager.Instance.AddDrone(newDrone);

        }

    }
}

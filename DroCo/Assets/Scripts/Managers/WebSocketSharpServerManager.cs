using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class TestBehavior : WebSocketBehavior {
    protected override void OnOpen() {
        base.OnOpen();
        Debug.Log("Connection open");
    }

    protected override void OnClose(CloseEventArgs e) {
        base.OnClose(e);
        Debug.Log("Connection close: " + e.Reason);
    }

    protected override void OnError(ErrorEventArgs e) {
        base.OnError(e);
        Debug.Log("Connection error: " + e.Message + " ..exception: " + e.Exception);
    }

    protected override void OnMessage(MessageEventArgs e) {
        base.OnMessage(e);
        Debug.Log(e.Data);

        try {
            Send("hello response");
        } catch (Exception ex) {
            Debug.LogError(ex.Message);
        }
    }
}

public class WebSocketServerBehavior : WebSocketBehavior {

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

    protected override void OnOpen() {
        base.OnOpen();
        Debug.Log("Connection open");
    }

    protected override void OnMessage(MessageEventArgs e) {
        base.OnMessage(e);

        Debug.Log(e.Data);
        Message<string> msg = JsonUtility.FromJson<Message<string>>(e.Data);
        if (msg.type == "hello") {
            DoHandshake(ID, JsonUtility.FromJson<Message<Hello>>(e.Data));
        } else if (handshake_done && msg.type == "data_broadcast") {

            Message<DroneFlightData> dfd = JsonUtility.FromJson<Message<DroneFlightData>>(e.Data);
            
            UnityMainThreadDispatcher.Instance().Enqueue(UpdateDroneFlightData(dfd.data));
        } else {
            Debug.LogError("Unknown data received! " + e.Data);
        }
    }

    protected override void OnClose(CloseEventArgs e) {
        base.OnClose(e);
        Debug.Log("Connection close: " + e.Reason);
    }

    protected override void OnError(ErrorEventArgs e) {
        base.OnError(e);
        Debug.Log("Connection error: " + e.Message + " ..exception: " + e.Exception);
    }

    private void DoHandshake(string clientID, Message<Hello> droneData) {
        Message<HelloResponse> helloResponse = new Message<HelloResponse> {
            data = new HelloResponse()
        };
        helloResponse.type = "hello_resp";
        helloResponse.data.client_id = clientID;
        helloResponse.data.rtmp_port = "1935";

        string msg = JsonUtility.ToJson(helloResponse);
        Debug.Log("Sending:" + msg);

        Send(msg);

        handshake_done = true;

        DroneStaticData newDrone = new DroneStaticData {
            client_id = clientID,
            drone_name = droneData.data.drone_name,
            serial = droneData.data.serial
        };

        UnityMainThreadDispatcher.Instance().Enqueue(AddDrone(newDrone));
    }

    private IEnumerator AddDrone(DroneStaticData newDrone) {
        DroneManager.Instance.AddDrone(newDrone);
        yield return null;
    }

    private IEnumerator UpdateDroneFlightData(DroneFlightData flightData) {
        DroneManager.Instance.HandleReceivedDroneData(flightData);
        yield return null;
    }
}

public class WebSocketSharpServerManager : Singleton<WebSocketSharpServerManager> {

    public string Address;
    public string Port;

    private WebSocketSharp.Server.WebSocketServer Server;

    private string clientID;

    private void Start() {
        Debug.Log("Starting server");
        try {
            Server = new WebSocketSharp.Server.WebSocketServer("ws://" + Address + ":" + Port);
            Server.AddWebSocketService<WebSocketServerBehavior>("/");

            //Server.AddWebSocketService<TestBehavior>("/test");

            Server.Start();

            Debug.Log("Server started on " + Address + " and port " + Port);

        } catch (Exception ex) {
            Debug.LogError(ex.Message);
            Port = (int.Parse(Port) + 1).ToString();
            Start();
            return;
        }

        //InvokeRepeating("SendMessageToClient", 5f, 5f);
    }

    private void SendMessageToClient() {
        clientID = Server.WebSocketServices["/test"].Sessions.IDs.First();
        Debug.Log("Sending test hello message to client " + clientID);
        try {
            Server.WebSocketServices["/test"].Sessions.SendTo("test hello message", clientID);
        } catch (Exception e) {
            Debug.LogError(e);
        }

    }

    private void OnApplicationQuit() {
        if (Server != null) {
            Server.Stop();
            Server = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DroCo;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using WebSocketSharp;

public class WebSocketManager : Singleton<WebSocketManager> {

    public string APIDomainWS = "";
    private WebSocket websocket;

    public delegate void DroneFlightDataEventHandler(object sender, DroneFlightDataEventArgs args);
    public class DroneFlightDataEventArgs {
        public string Data {
            get; set;
        }

        public DroneFlightDataEventArgs(string data) {
            Data = data;
        }
    }
    public DroneFlightDataEventHandler OnDroneDataReceived;

    /// <summary>
    /// Requset id pool
    /// </summary>
    private int requestID = 1;

    /// <summary>
    /// Dictionary of unprocessed responses
    /// </summary>
    private Dictionary<int, string> responses = new Dictionary<int, string>();


    private void Start() {
        ConnectToServer(PlayerPrefs.GetString("DrocoServerURL"));
    }


    /// <summary>
    /// Create websocket URI from domain name and port
    /// </summary>
    /// <param name="domain">Domain name or IP address</param>
    /// <param name="port">Server port</param>
    /// <returns></returns>
    public string GetWSURI(string domain, int port) {
        return "ws://" + domain + ":" + port.ToString();
    }

    public void ReconnectToServer(string domain, int port) {
        if (websocket.IsAlive) {
            websocket.Close();
        }
        ConnectToServer(domain, port);
    }

    public void ReconnectToServer(string wsURI) {
        if (websocket.IsAlive) {
            websocket.Close();
        }
        ConnectToServer(wsURI);
    }

    public void ConnectToServer(string domain, int port) {
        try {
            APIDomainWS = GetWSURI(domain, port);
            ConnectToServer(APIDomainWS);
        } catch (UriFormatException ex) {
            Debug.LogError(ex);
        }
    }

    public void ConnectToServer(string wsURI) {
        try {
            APIDomainWS = wsURI;
            websocket = new WebSocket(APIDomainWS);

            websocket.OnOpen += OnConnectedWS;
            websocket.OnError += OnErrorWS;
            websocket.OnClose += OnCloseWS;
            websocket.OnMessage += HandleReceivedDataWS;

            websocket.Connect();
        } catch (UriFormatException ex) {
            Debug.LogError(ex);
        }
    }

    private void HandleReceivedDataWS(object sender, MessageEventArgs e) {
        //string data = Encoding.Default.GetString(e.Data);
        UnityMainThreadDispatcher.Instance().Enqueue(UpdateDroneData(e.Data));
    }

    public IEnumerator UpdateDroneData(string data) {
        DroneManager.Instance.HandleReceivedDroneData(data);
        yield return null;
    }

    private void OnCloseWS(object sender, CloseEventArgs e) {
        Debug.Log("Connection closed!");
    }

    private void OnErrorWS(object sender, ErrorEventArgs e) {
        Debug.LogError(e.Message + " : " + e.Exception);
    }

    private void OnConnectedWS(object sender, EventArgs e) {
        Debug.Log("On connected");
    }
    /// <summary>
    /// Universal method for sending data to server
    /// </summary>
    /// <param name="data">String to send</param>
    /// <param name="key">ID of request (used to obtain result)</param>
    /// <param name="storeResult">Flag whether or not store result</param>
    /// <param name="logInfo">Flag whether or not log sended message</param>
    public void SendDataToServer(string data, int key = -1, bool storeResult = false, bool logInfo = false) {
        if (key < 0) {
            key = Interlocked.Increment(ref requestID);
        }
        if (logInfo)
            Debug.Log("Sending data to server: " + data);

        if (storeResult) {
            responses[key] = null;
        }
        SendWebSocketMessage(data);
    }

    /// <summary>
    /// Sends data to server
    /// </summary>
    /// <param name="data"></param>
    private async void SendWebSocketMessage(string data) {
        //if (websocket.State == WebSocketState.Open) {
        if (websocket.IsAlive == true) {
            websocket.Send(data);
        }
    }

    private void OnDestroy() {
        websocket.Close();
    }

}

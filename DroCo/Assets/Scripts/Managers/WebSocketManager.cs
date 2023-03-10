using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System;
using System.Text;

public class WebSocketManager : Singleton<WebSocketManager> {
    /// <summary>
    /// Drone Server URI
    /// </summary>
    private string APIDomainWS = "";
    /// <summary>
    /// Websocket context
    /// </summary>
    private WebSocket websocket;

    private void Update() {
        if (websocket != null && websocket.State == WebSocketState.Open)
            websocket.DispatchMessageQueue();
    }

    public async void ConnectToServer(string domain, int port) {
        try {
            APIDomainWS = GetWSURI(domain, port);
            websocket = new WebSocket(APIDomainWS);
            
            websocket.OnOpen += OnConnected;
            websocket.OnError += OnError;
            websocket.OnClose += OnClose;
            websocket.OnMessage += HandleReceivedData;

            await websocket.Connect();
        } catch (UriFormatException ex) {
            Debug.LogError(ex);
        }
    }

    private void HandleReceivedData(byte[] message) {
        string data = Encoding.Default.GetString(message);
        DroneManager.Instance.HandleReceivedDroneData(data);
    }

    private void OnClose(WebSocketCloseCode closeCode) {
        Debug.Log("Connection closed!");
    }

    private void OnError(string errorMsg) {
        Debug.LogError(errorMsg);
    }

    private void OnConnected() {
        Debug.Log("On connected");
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

    private async void OnApplicationQuit() {
        await websocket.Close();
    }

}

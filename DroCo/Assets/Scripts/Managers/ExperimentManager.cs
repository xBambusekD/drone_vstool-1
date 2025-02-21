using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

[RequireComponent(typeof(ExperimentSettings))]
public class ExperimentManager : Singleton<ExperimentManager> {

    public enum AppMode {
        DesktopUgCS,
        TabletARView,
        MobileTopdownView
    }

    public ExperimentSettings ExperimentSettings;

    public GameObject PlaceholderBackground;
    public UnityTransport UnityTransport;
    public NetworkRPCSync NetworkRPC;

    public GameObject ConnectionScreen, LoadingScreen;

    public VideoPlayerControls VideoPlayerControls;

    public event Action OnClientConnectedToServer;

    public TopPanel TopPanel;

    //public WebSocketServerExperiment WebSocketServer;
    //public WebSocketClientExperiment WebSocketClient;

    private Coroutine connectionTimeoutCoroutine;

    private bool fpvSet = false;

    private void Start() {
        if (ExperimentSettings == null) {
            ExperimentSettings = GetComponent<ExperimentSettings>();
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;


        if (ExperimentSettings.CurrentAppMode == AppMode.DesktopUgCS) {
            StartHost();
        }
        FlightLogPlayerManager.Instance.LoadDefaultFlightLog();
    }

    //public void StartHost() {
    //    WebSocketServerExperiment.Instance.StartServer();
    //}

    //public void StartClient(string ipAddress) {
    //    WebSocketClientExperiment.Instance.ConnectToServer(ipAddress, 5558);
    //}

    //public void HandleReceivedLogData(int sequenceNumber, DroneFlightData data) {
    //    OnPlayFlightLog();
    //    DroneManager.Instance.HandleReceivedDroneData(data);
    //}

    //public void HandleReceivedDroneData(DroneStaticData data) {
    //    DroneManager.Instance.AddDrone(data);
    //}

    //public DroneStaticData GetExperimentDroneData() {
    //    string logLine = FlightLogPlayerManager.Instance.GetLogMessage(0);
    //    DroneFlightData flightData = JsonUtility.FromJson<DroneFlightData>(logLine);

    //    DroneStaticData drone = new DroneStaticData {
    //        client_id = flightData.client_id,
    //        drone_name = "experiment_test_drone",
    //        serial = "experiment_serial"
    //    };

    //    return drone;
    //}

    //public void HandleClientDisconnected() {

    //}

    //public void OnClientConnected() {
    //    Debug.Log("Client successfully connected to the server!");
    //    ConnectionScreen.SetActive(false);
    //}





    public void StartHost() {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient(string ipAddress) {
        UnityTransport.ConnectionData.Address = ipAddress;
        LoadingScreen.SetActive(true);
        NetworkManager.Singleton.StartClient();
        connectionTimeoutCoroutine = StartCoroutine(ConnectionTimeout(5f));
    }

    private IEnumerator ConnectionTimeout(float timeoutDuration) {
        float timer = 0f;

        while (timer < timeoutDuration) {
            // If the client is connected, cancel the timeout
            if (NetworkManager.Singleton.IsConnectedClient) {

                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Timeout has expired, stop the client
        Debug.LogWarning("Timeout! Failed to connect the client to the server " + UnityTransport.ConnectionData.Address);
        //NetworkManager.Singleton.Shutdown();
        LoadingScreen.SetActive(false);
        // Clear the coroutine reference
        connectionTimeoutCoroutine = null;
    }

    private void OnClientConnected(ulong clientId) {
        if (clientId == NetworkManager.Singleton.LocalClientId) {
            Debug.Log("Client successfully connected to the server!");
            // If the timeout coroutine is running, stop it
            if (connectionTimeoutCoroutine != null) {
                StopCoroutine(connectionTimeoutCoroutine);
                connectionTimeoutCoroutine = null;
            }
            ConnectionScreen.SetActive(false);
            NetworkRPC.RequestVideoPlayerStatusRpc();
            OnClientConnectedToServer?.Invoke();
        } else {
            // Init drone on clients
            //NetworkRPC.SendDroneConnectionMessage(FlightLogPlayerManager.Instance.GetLogMessage(0));
            Debug.Log("Server registered new client connection.");
        }
    }

    // Called on clients only.
    public void UpdateVideoPlayerStatus(bool isPlaying) {
        VideoPlayerControls.UpdateStatus(isPlaying);
    }

    private void OnClientDisconnected(ulong clientId) {
        if (clientId == NetworkManager.Singleton.LocalClientId) {
            Debug.Log("Client disconnected from the server.");
            ConnectionScreen.SetActive(true);
            LoadingScreen.SetActive(false);
        } else {
            Debug.Log($"A client with ID {clientId} disconnected.");
        }
    }

    // Called on clients from server.
    public void OnFrameNumberResponse(int frameNumber) {
        // Do the setting of FPV
        OnPlayFlightLog();

        FlightLogPlayerManager.Instance.PlayLogMessage(frameNumber);
        VideoPlayerControls.UpdateProgressBar(frameNumber);
    }

    private void OnPlayFlightLog() {
        if (ExperimentSettings.CurrentAppMode == AppMode.TabletARView && !fpvSet) {
            PlaceholderBackground.SetActive(false);
            CameraManager.Instance.SetCameraFPV(true);
            fpvSet = true;
        }
    }

    public void OnPlayButtonPressed() {
        OnPlayFlightLog();
        NetworkRPC.SendPlayRpc();
    }

    public void OnPauseButtonPressed() {
        NetworkRPC.SendPauseRpc();
    }

    // Called on clients from server.
    public void OnPlayRpcReceived() {
        VideoPlayerControls.OnPlayButton();
    }

    // Called on clients from server.
    public void OnPauseRpcReceived() {
        VideoPlayerControls.OnPauseButton();
    }

    public void SyncVideoPlayerControls(int frameNumber) {
        NetworkRPC.SendFrameNumberRpc(frameNumber);
    }

    public void RequestPlayRpc() {
        NetworkRPC.RequestPlayRpc();
    }

    public void RequestPauseRpc() {
        NetworkRPC.RequestPauseRpc();
    }

    public void RequestSeekRpc(int frameNumber) {
        NetworkRPC.RequestSeekRpc(frameNumber);
    }

    public void OnReceiveRequestPlay() {
        VideoPlayerControls.OnPlayButton();
    }

    public void OnReceiveRequestPause() {
        VideoPlayerControls.OnPauseButton();
    }

    public void OnReceiveRequestSeek(int frame) {
        VideoPlayerControls.UpdateProgressBar(frame);
    }

    //public void SendFlightLogMessage(string message, int frameNumber) {
    //    NetworkRPC.SendFlightLogMessage(message);

    //    //WebSocketServerExperiment.Instance.SendMessageToAllClients("{\"type\":\"data_broadcast\", \"sequence_number\":" + frameNumber + ", \"data\":" + message + "}");
    //}


    public bool IsLogPlaying() {
        return VideoPlayerControls.IsPlaying;
    }

    private void OnDisable() {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}

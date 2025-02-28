using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ExperimentSettings))]
public class ExperimentManager : Singleton<ExperimentManager> {

    public enum StickConfiguration {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        CENTER,
        NONE
    }


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

    public Image RemoteControllerConnectionImage;
    public TMP_Text LeftStickText;
    public TMP_Text RightStickText;
    public TMP_Text LeftStickCommandText;
    public TMP_Text RightStickCommandText;

    private StickConfiguration leftStickConfiguration;
    private StickConfiguration rightStickConfiguration;
    private RemoteController lastRemoteControllerData;

    private void Start() {
        if (ExperimentSettings == null) {
            ExperimentSettings = GetComponent<ExperimentSettings>();
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;


        if (ExperimentSettings.CurrentAppMode == AppMode.DesktopUgCS) {
            StartHost();
            WebSocketServerExperiment.Instance.StartServer();
            SetNewStickConfiguration();
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
            CameraManager.Instance.DisplayVRSceneLit(false);
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

    public void SetNewStickConfiguration() {
        leftStickConfiguration = (StickConfiguration) UnityEngine.Random.Range(0, 4);
        rightStickConfiguration = (StickConfiguration) UnityEngine.Random.Range(0, 4);
        LeftStickCommandText.text = leftStickConfiguration.ToString();
        RightStickCommandText.text = rightStickConfiguration.ToString();

        // Invalidate current stick configuration with zero data
        HandleReceivedRemoteControllerData(new RemoteController() {
            client_id = "", left_stick = new Stick() { x = 0, y = 0 }, right_stick = new Stick() { x = 0, y = 0 }
        });
    }

    public StickConfiguration ComputeLeftStickConfiguration(DroneFlightData data) {
        // velocity Z  -Z (nahoru), 0 (stoji), Z (dolu)
        // yaw         -X (tocim se doleva), 0 (stoji), X (tocim se doprava)
        if (data.aircraft_velocity.velocity_z < 0) {
            return StickConfiguration.UP;
        } else if (data.aircraft_velocity.velocity_z > 0) {
            return StickConfiguration.DOWN;
        } else {
            return StickConfiguration.CENTER;
        }
    }

    public StickConfiguration ComputeRightStickConfiguration(DroneFlightData data) {
        // velocity X  -X (dopredu), 0 (stoji), X (dozadu)
        // velocity Y  -Y (doprava), 0 (stoji), Y (doleva)

        if (data.aircraft_velocity.velocity_x > data.aircraft_velocity.velocity_y) {
            if (data.aircraft_velocity.velocity_x < 0) {
                return StickConfiguration.UP;
            } else if (data.aircraft_velocity.velocity_x > 0) {
                return StickConfiguration.DOWN;
            } else {
                return StickConfiguration.CENTER;
            }
        } else {
            if (data.aircraft_velocity.velocity_y < 0) {
                return StickConfiguration.RIGHT;
            } else if (data.aircraft_velocity.velocity_y > 0) {
                return StickConfiguration.LEFT;
            } else {
                return StickConfiguration.CENTER;
            }
        }
    }

    public void SetNewStickConfiguration(DroneFlightData currentLog, DroneFlightData[] nextLogs = null) {
        //StickConfiguration left = ComputeLeftStickConfiguration(currentLog);
        //StickConfiguration right = ComputeRightStickConfiguration(currentLog);

        // LEFT STICK
        leftStickConfiguration = ComputeLeftStickConfiguration(currentLog);

        // RIGHT STICK
        rightStickConfiguration = ComputeRightStickConfiguration(currentLog);



        //leftStickConfiguration = (StickConfiguration) UnityEngine.Random.Range(0, 4);
        //rightStickConfiguration = (StickConfiguration) UnityEngine.Random.Range(0, 4);
        LeftStickCommandText.text = leftStickConfiguration.ToString();
        RightStickCommandText.text = rightStickConfiguration.ToString();

        // Reevaluate current controller data
        HandleReceivedRemoteControllerData(lastRemoteControllerData);
    }

    public void HandleReceivedRemoteControllerData(RemoteController data) {
        LeftStickText.text = data.left_stick.ToString();
        RightStickText.text = data.right_stick.ToString();

        if (GetStickConfiguration(data.left_stick) == leftStickConfiguration && GetStickConfiguration(data.right_stick) == rightStickConfiguration) {
            if (!VideoPlayerControls.IsPlaying) {
                VideoPlayerControls.OnPlayButton();
            }
        } else {
            VideoPlayerControls.OnPauseButton();
        }

        lastRemoteControllerData = data;

        //if (data.left_stick.y > 500 && data.right_stick.y > 500) {
        //    if (!VideoPlayerControls.IsPlaying) {
        //        VideoPlayerControls.OnPlayButton();
        //    }
        //} else {
        //    VideoPlayerControls.OnPauseButton();
        //}
    }

    private StickConfiguration GetStickConfiguration(Stick stick) {
        if (stick.x > 500) {
            return StickConfiguration.RIGHT;
        } else if (stick.x < -500) {
            return StickConfiguration.LEFT;
        } else if (stick.y > 500) {
            return StickConfiguration.UP;
        } else if (stick.y < -500) {
            return StickConfiguration.DOWN;
        } else if (stick.x < 50 && stick.x > -50 && stick.y < 50 && stick.y > -50) {
            return StickConfiguration.CENTER;
        }
        return StickConfiguration.NONE;
    }

    public void OnRemoteControllerConnected() {
        RemoteControllerConnectionImage.color = Color.green;
    }

    public void OnRemoteControllerDisconnected() {
        RemoteControllerConnectionImage.color = Color.red;
    }
}

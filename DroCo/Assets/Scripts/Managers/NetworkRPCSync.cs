using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkRPCSync : NetworkBehaviour {

    [Rpc(SendTo.NotServer)]
    public void SendFrameNumberRpc(int frameNumber) {
        ExperimentManager.Instance.OnFrameNumberResponse(frameNumber);
    }

    [Rpc(SendTo.NotServer)]
    public void SendPlayRpc() {
        ExperimentManager.Instance.OnPlayRpcReceived();
    }

    [Rpc(SendTo.NotServer)]
    public void SendPauseRpc() {
        ExperimentManager.Instance.OnPauseRpcReceived();
    }

    [Rpc(SendTo.Server)]
    public void RequestPlayRpc() {
        ExperimentManager.Instance.OnReceiveRequestPlay();
    }

    [Rpc(SendTo.Server)]
    public void RequestPauseRpc() {
        ExperimentManager.Instance.OnReceiveRequestPause();
    }

    [Rpc(SendTo.Server)]
    public void RequestSeekRpc(int frame) {
        ExperimentManager.Instance.OnReceiveRequestSeek(frame);
    }

    //[Rpc(SendTo.NotMe)]
    //private void SendFlightLogMessageRpc(string message, ulong sourceNetworkObjectId) {
    //    FlightLogPlayerManager.Instance.PlayLogMessage(message);
    //}

    //[Rpc(SendTo.NotMe)]
    //private void SendDroneConnectionMessageRpc(string message, ulong sourceNetworkObjectId) {
    //    DroneFlightData flightData = JsonUtility.FromJson<DroneFlightData>(message);

    //    FlightLogPlayerManager.Instance.ConnectDrone(flightData.client_id, "experiment_test_drone", "experiment_serial");
    //}

    [Rpc(SendTo.Server)]
    public void RequestVideoPlayerStatusRpc() {
        SendVideoPlayerStatusRpc(ExperimentManager.Instance.IsLogPlaying());
    }

    [Rpc(SendTo.NotServer)]
    public void SendVideoPlayerStatusRpc(bool isPlaying) {
        ExperimentManager.Instance.UpdateVideoPlayerStatus(isPlaying);
    }

    //public void SendFrameNumber(int frameNumber) {
    //    SendFrameNumRpc(frameNumber, NetworkObjectId);
    //}

    //public void SendFlightLogMessage(string message) {
    //    Debug.Log("Number of characters " + message.Length);
    //    Debug.Log("Bytes: " + message.Length * sizeof(char));
    //    SendFlightLogMessageRpc(message, NetworkObjectId);
    //}

    //public void SendDroneConnectionMessage(string message) {
    //    SendDroneConnectionMessageRpc(message, NetworkObjectId);
    //}


}

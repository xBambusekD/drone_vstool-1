using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public string ServerIP = "butcluster.ddns.net";
    public int ServerPort = 5555;
    public int RTMPPort = 1935;

    private void Start() {
        WebSocketManager.Instance.ConnectToServer(ServerIP, ServerPort);
    }

}


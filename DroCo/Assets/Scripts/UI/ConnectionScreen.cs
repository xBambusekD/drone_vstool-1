using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectionScreen : MonoBehaviour {

    public TMP_InputField IPInputField;

    private void Start() {
        IPInputField.text = PlayerPrefs.GetString("server_ip");
    }

    public void OnIPConnect() {
        ExperimentManager.Instance.StartClient(IPInputField.text);
    }

    public void SaveIPAddress(string ip) {
        PlayerPrefs.SetString("server_ip", ip);
    }
}

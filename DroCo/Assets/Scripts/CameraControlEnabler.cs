using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CameraControlEnabler : MonoBehaviour {

    [SerializeField]
    private FreeCamera CameraMoverScript;

    private void Start() {
        CameraMoverScript.enabled = false;
    }

    private void Update() {
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            CameraMoverScript.enabled = true;
        }
        if (Mouse.current.rightButton.wasReleasedThisFrame) {
            CameraMoverScript.enabled= false;
        }
    }
}

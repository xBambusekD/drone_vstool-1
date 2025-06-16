using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class DroneVirtualStickController : MonoBehaviour {


    private DroneVirtualStickInputs controls;

    public DroneControlCommand CurrentCommand = new DroneControlCommand();

    public float MAX_PITCH_ROLL = 0.3f;     // m/s
    public float MAX_YAW_SPEED = 15.0f;    // deg/s
    public float MAX_THROTTLE = 0.2f;     // m/s


    private void Awake() {
        controls = new DroneVirtualStickInputs();

        controls.DroneInputs.Pitch.started += PitchStarted;
        controls.DroneInputs.Pitch.performed += PitchPerformed;
        controls.DroneInputs.Pitch.canceled += PitchCanceled;

        controls.DroneInputs.Roll.started += RollStarted;
        controls.DroneInputs.Roll.performed += RollPerformed;
        controls.DroneInputs.Roll.canceled += RollCanceled;

        controls.DroneInputs.Yaw.started += YawStarted;
        controls.DroneInputs.Yaw.performed += YawPerformed;
        controls.DroneInputs.Yaw.canceled += YawCanceled;

        controls.DroneInputs.Vertical.started += VerticalStarted;
        controls.DroneInputs.Vertical.performed += VerticalPerformed;
        controls.DroneInputs.Vertical.canceled += VerticalCanceled;
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }


    private void PitchStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float pitch = obj.ReadValue<float>();
        CurrentCommand.pitch = pitch * MAX_PITCH_ROLL;
    }

    private void PitchPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float pitch = obj.ReadValue<float>();
    }

    private void PitchCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float pitch = 0f;
        CurrentCommand.pitch = pitch;
    }

    private void RollStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float roll = obj.ReadValue<float>();
        CurrentCommand.roll = roll * MAX_PITCH_ROLL;
    }

    private void RollPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float roll = obj.ReadValue<float>();
    }

    private void RollCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float roll = 0f;
        CurrentCommand.roll = roll;
    }

    private void YawStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float yaw = obj.ReadValue<float>();
        CurrentCommand.yaw = yaw * MAX_YAW_SPEED;            
    }

    private void YawPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float yaw = obj.ReadValue<float>();
    }

    private void YawCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float yaw = 0f;
        CurrentCommand.yaw = yaw;
    }


    private void VerticalStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float vertical = obj.ReadValue<float>();
        CurrentCommand.throttle = vertical * MAX_THROTTLE;
    }

    private void VerticalPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float vertical = obj.ReadValue<float>();
    }

    private void VerticalCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float vertical = 0f;
        CurrentCommand.throttle = vertical;
    }


}

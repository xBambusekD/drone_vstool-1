using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : Singleton<CameraManager> {
    public enum CameraView {
        FirstPerson,
        ThirdPerson,
        FreeLook
    }

    [SerializeField]
    private Camera MainCamera;

    private ArcGISCameraControllerTouch cameraControllerTouch;
    private CameraFollowSimple cameraFollow;

    public bool FollowingTarget { get; private set; }

    private float pinchSpeed = 0.03f;

    //public Texture2D VideoTexture;

    //public Camera DroneViewCamera;
    //public Camera DroneFPVCamera;
    //public GameObject DroneModel;
    private void Start() {
        cameraControllerTouch = MainCamera.GetComponent<ArcGISCameraControllerTouch>();
        cameraFollow = MainCamera.GetComponent<CameraFollowSimple>();
        FollowingTarget = false;
    }


    private void Update() {
        if (GetMouseScollValue() != 0f && FollowingTarget) {
            if (GameManager.Instance.CurrentDisplayState == GameManager.DisplayState.Scene3DView) {
                StopFollowingTarget();
            }
        }

        //if (Keyboard.current[Key.Numpad1].wasPressedThisFrame) {
        //    SetCameraView(CameraView.FirstPerson);
        //} else if (Keyboard.current[Key.Numpad2].wasPressedThisFrame) {
        //    SetCameraView(CameraView.ThirdPerson);
        //} else if (Keyboard.current[Key.Numpad3].wasPressedThisFrame) {
        //    SetCameraView(CameraView.FreeLook);
        //}
    }

    private float GetMouseScollValue() {
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 2) {
            UnityEngine.InputSystem.EnhancedTouch.Touch touch1 = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];
            UnityEngine.InputSystem.EnhancedTouch.Touch touch2 = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[1];

            Vector2 touch1Previous = touch1.screenPosition - touch1.delta;
            Vector2 touch2Previous = touch2.screenPosition - touch2.delta;

            return -pinchSpeed * (Vector2.Distance(touch1Previous, touch2Previous) - Vector2.Distance(touch1.screenPosition, touch2.screenPosition));
        } else if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 0) {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current.scroll.ReadValue().normalized.y;
#else
            return Input.mouseScrollDelta.y;
#endif
        } else {
            return 0f;
        }
    }

    public void StartFollowingTarget(Transform transformToFollow) {
        cameraControllerTouch.enabled = false;
        cameraFollow.enabled = true;
        cameraFollow.StartFollowing(transformToFollow);
        FollowingTarget = true;
    }

    public void StopFollowingTarget() {
        cameraControllerTouch.enabled = true;
        cameraFollow.enabled = false;
        cameraFollow.StopFollowing();
        FollowingTarget = false;
    }

    //public void SetCameraView(CameraView view) {
    //    switch (view) {
    //        case CameraView.FirstPerson:
    //            MainCamera.gameObject.SetActive(false);
    //            DroneViewCamera.gameObject.SetActive(false);
    //            DroneFPVCamera.gameObject.SetActive(true);
    //            DroneModel.gameObject.SetActive(false);
    //            break;
    //        case CameraView.ThirdPerson:
    //            MainCamera.gameObject.SetActive(true);
    //            DroneViewCamera.gameObject.SetActive(true);
    //            DroneFPVCamera.gameObject.SetActive(false);
    //            DroneModel.gameObject.SetActive(true);
    //            break;
    //        case CameraView.FreeLook:
    //            MainCamera.gameObject.SetActive(true);
    //            DroneViewCamera.gameObject.SetActive(false);
    //            DroneFPVCamera.gameObject.SetActive(false);
    //            DroneModel.gameObject.SetActive(true);
    //            break;
    //    }
    //    Debug.Log("Setting camera view to: " + view);
    //}

}

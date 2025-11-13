using System.Collections;
using System.Collections.Generic;
using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;

public class MouseCameraController : MonoBehaviour {
    [Header("Camera Movement Settings")]
    public float PanSpeed = 0.15f;
    public float RotateSpeed = 10;
    public float ZoomSpeed = 0.1f;
    public float MinZoom = 5f;
    public float MaxZoom = 10000f;
    public bool IsCamera2D = false;

    [Header("UI Elements")]
    public GameObject PivotMarker;
    public float PivotScale = 0.008f;

    private CameraControls controls;
    private Vector2 panDelta;
    private Vector2 rotateDelta;
    private float zoomDelta;

    private Vector3 pivotPoint = Vector3.one;
    private Vector2 prevTouchDistance;

    private void Awake() {
        EnhancedTouchSupport.Enable();

        controls = new CameraControls();

        controls.Camera.Pan.performed += callbackContext => panDelta = callbackContext.ReadValue<Vector2>();
        controls.Camera.Pan.canceled += callbackContext => panDelta = Vector2.zero;

        controls.Camera.Rotate.performed += callbackContext => rotateDelta = callbackContext.ReadValue<Vector2>();
        controls.Camera.Rotate.canceled += callbackContext => rotateDelta = Vector2.zero;

        controls.Camera.Zoom.performed += callbackContext => zoomDelta = callbackContext.ReadValue<Vector2>().y;
    }

    private void Start() {
        if (IsCamera2D) {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }

    private void Update() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (Application.isMobilePlatform) {
                HandleTouchInput();
            } else {
                HandleMouseInput();
            }
        }
    }

    private void HandleMouseInput() {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        HandlePan(mouseScreenPosition);
        HandleRotate();
        HandleZoom(mouseScreenPosition);

        if (Mouse.current.rightButton.wasPressedThisFrame) {
            pivotPoint = RaycastPivotPoint(mouseScreenPosition);
            DisplayMarkerOnPivot(true);
        } else if (Mouse.current.leftButton.wasPressedThisFrame) {
            pivotPoint = RaycastPivotPoint(mouseScreenPosition);
        } else if (Mouse.current.rightButton.wasReleasedThisFrame) {
            DisplayMarkerOnPivot(false);
        }
    }

    private void HandleTouchInput() {
        UnityEngine.InputSystem.Utilities.ReadOnlyArray<UnityEngine.InputSystem.EnhancedTouch.Touch> activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;

        if (activeTouches.Count == 0) {
            // reset touch distance
            prevTouchDistance = Vector2.zero;
            return;
        }

        if (activeTouches.Count == 1) { // Pan
            panDelta = activeTouches[0].delta;
            HandlePan(activeTouches[0].screenPosition);
        } else if (activeTouches.Count == 2) { // Rotate and Zoom
            TouchControl(activeTouches[0].screenPosition, activeTouches[1].screenPosition);
        }
    }

    private void HandlePan(Vector2 screenPosition) {
        if (panDelta != Vector2.zero) {

            // Calculate movement in world space parallel to the map's plane
            Vector3 right = transform.right;
            Vector3 forward = Vector3.Cross(transform.right, Vector3.up);

            // Adjust pan speed based on camera distance from the pivot point
            if (Vector3.Distance(pivotPoint, Vector3.one) <= 0.1f) {
                pivotPoint = RaycastPivotPoint(screenPosition);
            }
            float distance = Vector3.Distance(transform.position, pivotPoint);
            float adjustedPanSpeed = PanSpeed * Mathf.Max(distance, 1f); // Ensure minimum scaling factor

            Vector3 translation = (-panDelta.x * right - panDelta.y * forward) * adjustedPanSpeed * Time.deltaTime;
            transform.position += translation;
        }
    }

    private void HandleRotate() {
        if (rotateDelta != Vector2.zero) {
            Vector3 rotation = new Vector3(-rotateDelta.y, rotateDelta.x, 0) * RotateSpeed * Time.deltaTime;
            transform.RotateAround(pivotPoint, Vector3.up, rotation.y);
            if (!IsCamera2D) {
                transform.RotateAround(pivotPoint, transform.right, rotation.x);
            }
        }
    }

    private void HandleZoom(Vector2 screenPosition) {
        if (zoomDelta != 0) {
            pivotPoint = RaycastPivotPoint(screenPosition);

            // Calculate the direction towards the pivot point
            Vector3 direction = (pivotPoint - transform.position).normalized;

            float distanceToPivot = Vector3.Distance(transform.position, pivotPoint);

            // Apply zoom
            float zoomAmount = zoomDelta * ZoomSpeed * Time.deltaTime * Mathf.Max(distanceToPivot, 1f);            
            Vector3 newPosition = transform.position + direction * zoomAmount;

            // Ensure the camera stays within the zoom limits
            float distance = Vector3.Distance(newPosition, pivotPoint);
            if (distance >= MinZoom && distance <= MaxZoom) {
                transform.position = newPosition;
            }

            // Reset zoomDelta
            zoomDelta = 0;
        }
    }


    private void TouchControl(Vector2 touch1Pos, Vector2 touch2Pos) {

        Vector2 currentTouchDistance = touch1Pos - touch2Pos;

        if (prevTouchDistance != Vector2.zero) {
            float pinchDelta = currentTouchDistance.magnitude - prevTouchDistance.magnitude;
            zoomDelta = pinchDelta; // Adjust sensitivity

            Vector2 midpointCurrent = (touch1Pos + touch2Pos) / 2;

            HandleZoom(midpointCurrent);
        }
        prevTouchDistance = currentTouchDistance;
    }

    private Vector3 RaycastPivotPoint(Vector2 screenPosition) {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            return hit.point;
        }
        return Vector3.zero;
    }

    private void DisplayMarkerOnPivot(bool active) {
        PivotMarker.transform.position = pivotPoint;
        PivotMarker.transform.localScale = Vector3.one * Vector3.Distance(transform.position, pivotPoint) * PivotScale;
        PivotMarker.SetActive(active);
    }
}

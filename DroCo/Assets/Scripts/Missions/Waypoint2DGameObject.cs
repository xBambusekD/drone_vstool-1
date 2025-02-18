using System.Collections;
using System.Collections.Generic;
using CesiumForUnity;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Waypoint2DGameObject : MonoBehaviour {

    [SerializeField]
    private RectTransform connectionBinder;
    public RectTransform ConnectionBinder => connectionBinder;

    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private RectTransform model; 

    private LayerMask layerMask;
    private CesiumGlobeAnchor location;


    private void Start() {
        layerMask = ~LayerMask.GetMask("Mission", "DroneScreen");
    }

    private void SetText(string txt) {
        text.text = txt;
    }

    public void InitWaypoint(string waypointName) {
        SetText(waypointName);

        location = GetComponent<CesiumGlobeAnchor>();
        ExperimentManager.Instance.OnClientConnectedToServer += OnConnected;
    }

    private void OnConnected() {
        GroundWaypoint();
    }

    private void GroundWaypoint() {
        if (location != null) {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity, layerMask)) {
                if (hit.collider != null && hit.distance < 50000) {
                    double3 originalPosition = location.longitudeLatitudeHeight;
                    location.longitudeLatitudeHeight = new double3(originalPosition.x, originalPosition.y, originalPosition.z - (double) hit.distance);
                    Debug.Log("Moving waypoint about " + hit.distance + " down. " + "Hit to " + hit.collider.transform.parent.name);
                }
            }
        }
    }
}

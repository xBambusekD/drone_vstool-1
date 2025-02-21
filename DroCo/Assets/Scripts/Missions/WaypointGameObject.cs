using System.Collections;
using System.Collections.Generic;
using CesiumForUnity;
using Highlighters;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class WaypointGameObject : MonoBehaviour {

    [SerializeField]
    private RectTransform connectionBinder;
    public RectTransform ConnectionBinder => connectionBinder;

    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private MeshRenderer model;

    [SerializeField]
    private GameObject shadow;

    [SerializeField]
    private Material apMaterial;

    [SerializeField]
    private Material apErrorMaterial;

    private LayerMask layerMask;
    private LayerMask mapLayerMask;

    public float SphereRadius = 1f;

    private HighlighterRenderer highlighter;
    public bool highlighted = false;

    public CesiumGlobeAnchor location;

    public Waypoint WaypointRef;

    private float AGL = 0f;


    private void Start() {
        layerMask =~ LayerMask.GetMask("Mission", "DroneScreen");
        mapLayerMask = LayerMask.GetMask("Map");
        highlighter = new HighlighterRenderer(model, 1);
    }

    private void Update() {
        GroundShadow();
        CheckCollisions();
    }

    private void GroundShadow() {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity, layerMask)) {
            if (hit.collider != null) {
                shadow.transform.localScale = new Vector3(shadow.transform.localScale.x, hit.distance / 2, shadow.transform.localScale.z);
            }
        }
    }

    private void CheckCollisions() {
        if (Physics.CheckSphere(transform.position, SphereRadius, layerMask)) {
            //model.material = apErrorMaterial;
            if (!highlighted) {
                MissionManager.Instance.HighlightWaypointOccluded(highlighter);
                highlighted = true;
            }
        } else {
            //model.material = apMaterial;
            if (highlighted) {
                MissionManager.Instance.UnHighlightWaypoint(highlighter);
                highlighted = false;
            }
        }
    }

    public void SetText(string txt) {
        text.text = txt;
    }

    public void SetAltitudeCoroutine(float altitude) {
        if (isActiveAndEnabled) {
            AGL = altitude;
            StartCoroutine(SetAltitude(altitude));
        }
    }

    public void SetAltitudeAGL(float altitudeAGL) {
        if (Physics.Raycast(transform.position + new Vector3(0, 1000, 0), transform.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity, mapLayerMask)) {
            if (hit.collider != null) {
                double3 originalPosition = location.longitudeLatitudeHeight;
                location.longitudeLatitudeHeight = new double3(originalPosition.x, originalPosition.y, originalPosition.z - (double)(hit.distance - 1000) + altitudeAGL);
            }
        }
    }

    private IEnumerator SetAltitude(float altitude) {
        yield return new WaitForSeconds(0.5f);

        SetAltitudeAGL(altitude);
    }

    public void SetLocation(CesiumGlobeAnchor loc) {
        location = loc;
    }

    public void SetAsStartingPoint() {
        model.transform.localScale = Vector3.one;
        model.material.color = Color.green;
    }

    public void SetAsLastPoint() {
        model.transform.localScale = Vector3.one;
        model.material.color = Color.red;
    }

    public string GetName() {
        return WaypointRef.Name;
    }

    public float GetAMSL() {
        return Mathf.Round((float) (location.longitudeLatitudeHeight.z) * 10.0f) * 0.1f;
    }

    public float GetAGL() {
        return AGL;
    }
}

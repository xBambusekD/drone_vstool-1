using System.Collections;
using System.Collections.Generic;
using Highlighters;
using TMPro;
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

    public float SphereRadius = 1f;

    private HighlighterRenderer highlighter;
    private bool highlighted = false;

    private void Start() {
        layerMask =~ LayerMask.GetMask("Mission", "DroneScreen");
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
}

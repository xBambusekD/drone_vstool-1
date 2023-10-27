using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHeightCorrector : MonoBehaviour {

    private int invokeCounter = 0;

    private void Start() {
        InvokeRepeating("CorrectBuildingHeightInChildren", 0f, 10f);
    }

    private void CorrectBuildingHeightInChildren() {
        // Slow down building height corrector after five initial tries
        invokeCounter++;
        if (invokeCounter == 5) {
            Debug.Log("Slowing down the invokes");
            CancelInvoke();
            InvokeRepeating("CorrectBuildingHeightInChildren", 0f, 120f);
        }

        CorrectBuildingHeight(transform);
    }

    private void CorrectBuildingHeight(Transform parent) {
        
        float scale = GameManager.Instance.BuildingScale;
        float altitudeOffset = GameManager.Instance.BuildingAltitudeOffset;

        //Debug.Log("Correcting building heights for the " + invokeCounter);
        foreach (Transform go in parent) {
            if (go.name.StartsWith("ArcGISGameObject")) {
                if (go.GetComponent<MeshRenderer>().material.name.StartsWith("SceneNodeSurface")) {
                    if (go.TryGetComponent(out Building building)) {

                    } else {
                        building = go.gameObject.AddComponent<Building>();
                        building.SetScale(scale);
                    }
                    if (!building.BuildingCorrected) {
                        building.CorrectBuilding(scale, altitudeOffset);
                    }
                } else if (go.TryGetComponent(out Building building)) {
                    building.NotABuilding();
                }
            }

            CorrectBuildingHeight(go);
        }
    }
}

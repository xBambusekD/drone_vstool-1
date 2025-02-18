using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class ExperimentSettings : MonoBehaviour {


    [Header("Select App Mode Loadout")]
    public ExperimentManager.AppMode CurrentAppMode;

    [Header("GameObjects by App Mode")]
    public List<GameObject> DesktopUgCSObjects = new List<GameObject>();
    public List<GameObject> TabletARViewObjects = new List<GameObject>();
    public List<GameObject> MobileTopdownViewObjects = new List<GameObject>();

    [Header("Additional Managers to be Set")]
    public MapManager MapManager;
    public ExperimentManager ExperimentManager;

    [Header("Connection Materials")]
    public Material ConnectionMaterial;
    public Material ConnectionMaterial2D;
    public LineRenderer ConnectionPrefab;

    private ExperimentManager.AppMode previousAppMode;

    // Automatically called when a value is changed in the Inspector
    private void OnValidate() {
        // Check if the value of selectedOption has actually changed
        if (CurrentAppMode != previousAppMode) {
            previousAppMode = CurrentAppMode; // Update the previous option
            ApplyActivation();
        }
    }

    // Method to activate/deactivate GameObjects based on the enum
    public void ApplyActivation() {
        switch (CurrentAppMode) {
            case ExperimentManager.AppMode.DesktopUgCS:
                ActivateObjects(TabletARViewObjects, false);
                ActivateObjects(MobileTopdownViewObjects, false);
                ActivateObjects(DesktopUgCSObjects, true);
                ExperimentManager.VideoPlayerControls = DesktopUgCSObjects.First().GetComponent<VideoPlayerControls>();
                ConnectionPrefab.materials = new Material[] { ConnectionMaterial };
                break;
            case ExperimentManager.AppMode.TabletARView:
                ActivateObjects(MobileTopdownViewObjects, false);
                ActivateObjects(DesktopUgCSObjects, false);
                ActivateObjects(TabletARViewObjects, true);
                ExperimentManager.VideoPlayerControls = TabletARViewObjects.First().GetComponent<VideoPlayerControls>();
                ConnectionPrefab.materials = new Material[] { ConnectionMaterial };
                break;
            case ExperimentManager.AppMode.MobileTopdownView:
                ActivateObjects(TabletARViewObjects, false);
                ActivateObjects(DesktopUgCSObjects, false);
                ActivateObjects(MobileTopdownViewObjects, true);
                ExperimentManager.VideoPlayerControls = MobileTopdownViewObjects.First().GetComponent<VideoPlayerControls>();
                ConnectionPrefab.materials = new Material[] { ConnectionMaterial2D };
                break;
        }

        // Only ARView uses ArcGIS, rest uses Cesium Tilesets
        MapManager.CurrentMapType = CurrentAppMode == ExperimentManager.AppMode.TabletARView ? MapManager.MapType.ArcGIS : MapManager.MapType.Cesium;        
    }

    private void ActivateObjects(List<GameObject> objects, bool activate) {
        foreach (GameObject obj in objects) {
            if (obj != null) {
                obj.SetActive(activate);
            }
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotMarker : MonoBehaviour {

    private void Update() {
        transform.LookAt(Camera.main.transform.position);
    }
}

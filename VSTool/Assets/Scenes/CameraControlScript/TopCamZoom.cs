using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopCamZoom : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Vector3 zoom = new Vector3(0, scroll * 0.8f, 0);
        if ((transform.localPosition.y > 0.5 || scroll > 0) && (transform.localPosition.y < 30f || scroll < 0))
            transform.localPosition = transform.localPosition + zoom;
    }
}

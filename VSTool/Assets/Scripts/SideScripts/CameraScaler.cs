using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        Rect camRect = cam.rect;
        camRect.yMax = 0.96f;
        camRect.xMax = 0.8f; // 80% of viewport
        cam.rect = camRect;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

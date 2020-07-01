using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform CameraPosition;
    void Start()
    {
        transform.position = CameraPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = CameraPosition.position;
    }
}

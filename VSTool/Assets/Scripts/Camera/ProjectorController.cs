using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorController : MonoBehaviour
{
    public int FOV;
    public int width;
    public int height;
    public Projector projector;

    // Start is called before the first frame update
    void Start()
    {
        //FOV = PlayerPrefs.GetInt("CameraFOV");
        width = PlayerPrefs.GetInt("CameraResWidth");
        height = PlayerPrefs.GetInt("CameraResHeight");

        projector = gameObject.GetComponent<Projector>();
        projector.fieldOfView = 80;
        projector.aspectRatio = (1.0f + width) / height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

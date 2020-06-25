/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoScreenResizer : MonoBehaviour
{
    public int FOV;
    public int width;
    public int height;
    public float distance;
    public Texture noVideoTexture;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("CameraResWidth") && PlayerPrefs.HasKey("CameraResHeight") && PlayerPrefs.HasKey("CameraFOV"))
        {
            FOV = PlayerPrefs.GetInt("CameraFOV");
            width = PlayerPrefs.GetInt("CameraResWidth");
            height = PlayerPrefs.GetInt("CameraResHeight");
            distance = PlayerPrefs.GetFloat("CameraScreenDistance");

            resize();
        }

        Material material = gameObject.GetComponent<Renderer>().sharedMaterial;
        material.SetTexture("_MainTex", noVideoTexture);
        material.SetTextureScale("_MainTex", new Vector2(1, 1));
    }

    // Update is called once per frame
    void Update()
    {
        //resize();
    }

    void resize()
    {
        float videoSizeWhidth = Mathf.Tan((FOV / 2) * Mathf.Deg2Rad) * 2 * distance;
        float videoSizeHeight = videoSizeWhidth / width * height;

        transform.localPosition = new Vector3(-1*distance, 0, 0);
        transform.localScale = new Vector3(videoSizeWhidth, videoSizeHeight, 0.01f);

    }

    public void resizeSlider(float newFOV){
        float videoSizeWhidth = Mathf.Tan((newFOV / 2) * Mathf.Deg2Rad) * 2 * distance;
        float videoSizeHeight = videoSizeWhidth / width * height;

        transform.localPosition = new Vector3(-1*distance, 0, 0);
        transform.localScale = new Vector3(videoSizeWhidth, videoSizeHeight, 0.01f);
    }
}

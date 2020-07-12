using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
public class SetupPlayerPrefs : MonoBehaviour
{
    public TMP_InputField RosBridgeURL;
    public TMP_InputField MapCenter;

    public Slider CameraFOV;

    public Slider CameraResWidth;
    public Slider CameraResHeight;
    public Slider CameraScreenDistance;

    public TMP_InputField Topic;

    public Slider MapSize;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("CameraResWidth") ) PlayerPrefs.SetInt("CameraResWidth", 672);
        if (!PlayerPrefs.HasKey("CameraResHeight") ) PlayerPrefs.SetInt("CameraResHeight", 376);
        if (!PlayerPrefs.HasKey("CameraFOV") ) PlayerPrefs.SetInt("CameraFOV", 130);
        if (!PlayerPrefs.HasKey("CameraScreenDistance") ) PlayerPrefs.SetFloat("CameraScreenDistance", 20f);
        if (!PlayerPrefs.HasKey("VideoCompression") ) PlayerPrefs.SetInt("VideoCompression", 1);

        if (!PlayerPrefs.HasKey("RosBridgeURL") ) PlayerPrefs.SetString("RosBridgeURL", "ws://10.42.0.1:9090");

        if (!PlayerPrefs.HasKey("MapDefaultLayer") ) PlayerPrefs.SetString("MapDefaultLayer", "mapbox.satellite");//mapbox://styles/mapbox/streets-v10
        if (!PlayerPrefs.HasKey("MapCenter") ) PlayerPrefs.SetString("MapCenter", "49.226564, 16.596639");
        if (!PlayerPrefs.HasKey("MapSize") ) PlayerPrefs.SetInt("MapSize", 4);
        if (!PlayerPrefs.HasKey("VideoTimeStep") ) PlayerPrefs.SetFloat("VideoTimeStep", 0);
        if (!PlayerPrefs.HasKey("VideoTopic") ) PlayerPrefs.SetString("VideoTopic", "/zed/right/image_rect_color/compressed/optimized");
        if (!PlayerPrefs.HasKey("AltitudeOffset") ) PlayerPrefs.SetFloat("AltitudeOffset", 0);

        RosBridgeURL.text = PlayerPrefs.GetString("RosBridgeURL");
        MapCenter.text = PlayerPrefs.GetString("MapCenter");
        MapSize.value = PlayerPrefs.GetInt("MapSize");
        CameraFOV.value = PlayerPrefs.GetInt("CameraFOV");
        CameraResHeight.value = PlayerPrefs.GetInt("CameraResHeight");
        CameraResWidth.value = PlayerPrefs.GetInt("CameraResWidth");
        CameraScreenDistance.value = PlayerPrefs.GetFloat("CameraScreenDistance");
        Topic.text = PlayerPrefs.GetString("VideoTopic");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class StartUpManager : MonoBehaviour
{
    public InputField RosBridgeUrlInputField;
    public InputField MapCenterInputField;
    public Slider MapSizeSlider;
    public InputField CamResWidthInputField;
    public InputField CamResHeightInputField;
    public InputField CamFovInputField;
    public InputField CameraScreenDistInputField;
    public InputField TopicVideoField;
    public InputField VideoFPSField;
    public InputField AltitudeOffsetInputField;
    public Toggle SatelliteMapToggle;
    public Toggle VideoCompressionToggle;

    void Awake()
    {
        SetDefaults(false);
    }

    void SetDefaults(bool force)
    {
        if (!PlayerPrefs.HasKey("CameraResWidth") || force) PlayerPrefs.SetInt("CameraResWidth", 672);
        if (!PlayerPrefs.HasKey("CameraResHeight") || force) PlayerPrefs.SetInt("CameraResHeight", 376);
        if (!PlayerPrefs.HasKey("CameraFOV") || force) PlayerPrefs.SetInt("CameraFOV", 130);
        if (!PlayerPrefs.HasKey("CameraScreenDistance") || force) PlayerPrefs.SetFloat("CameraScreenDistance", 20f);
        if (!PlayerPrefs.HasKey("VideoCompression") || force) PlayerPrefs.SetInt("VideoCompression", 1);

        if (!PlayerPrefs.HasKey("RosBridgeURL") || force) PlayerPrefs.SetString("RosBridgeURL", "ws://10.42.0.1:9090");

        if (!PlayerPrefs.HasKey("MapDefaultLayer") || force) PlayerPrefs.SetString("MapDefaultLayer", "mapbox.satellite");//mapbox://styles/mapbox/streets-v10
        if (!PlayerPrefs.HasKey("MapCenter") || force) PlayerPrefs.SetString("MapCenter", "49.226564, 16.596639");
        if (!PlayerPrefs.HasKey("MapSize") || force) PlayerPrefs.SetInt("MapSize", 4);
        if (!PlayerPrefs.HasKey("VideoTimeStep") || force) PlayerPrefs.SetFloat("VideoTimeStep", 0);
        if (!PlayerPrefs.HasKey("VideoTopic") || force) PlayerPrefs.SetString("VideoTopic", "/zed/right/image_rect_color/compressed/optimized");
        if (!PlayerPrefs.HasKey("AltitudeOffset") || force) PlayerPrefs.SetFloat("AltitudeOffset", 0);
    }


    public void VideoTimeStepChanged(string value)
    {
        int fps = int.Parse(VideoFPSField.text);
        float timeStep = 0;
        if (fps > 0)
            timeStep = 1.0f / fps;
        PlayerPrefs.SetFloat("VideoTimeStep", timeStep);
    }

    public void VideoTopicChanged(string value)
    {
        PlayerPrefs.SetString("VideoTopic", TopicVideoField.text);
    }

    public void RosBridgeUrlChanged(string value)
    {
        PlayerPrefs.SetString("RosBridgeURL", value);
    }

    public void MapCenterChanged(string value)
    {
        PlayerPrefs.SetString("MapCenter", value);
    }

    public void MapSizeChanged(float value)
    {
        //Debug.Log("MapSizeChanged " + MapSizeSlider.value);
        PlayerPrefs.SetInt("MapSize", Mathf.RoundToInt(MapSizeSlider.value));
    }

    public void AltitudeOffsetChanged(float value)
    {
        PlayerPrefs.SetFloat("AltitudeOffset", float.Parse(AltitudeOffsetInputField.text.Replace('.', ',')));
    }


    public void CamResWidthChanged(string value)
    {
        PlayerPrefs.SetInt("CameraResWidth", int.Parse(CamResWidthInputField.text));
    }

    public void CamResHeightChanged(string value)
    {
        PlayerPrefs.SetInt("CameraResHeight", int.Parse(CamResHeightInputField.text));
    }

    public void CamFovChanged(string value)
    {
        PlayerPrefs.SetInt("CameraFOV", int.Parse(CamFovInputField.text));
    }

    public void CameraScreenDistChanged(string value)
    {
        PlayerPrefs.SetFloat("CameraScreenDistance", float.Parse(CameraScreenDistInputField.text.Replace('.', ',')));
    }

    public void SatalliteMapChanged(bool val)
    {
        if(SatelliteMapToggle.isOn)
            PlayerPrefs.SetString("MapDefaultLayer", "mapbox.satellite");
        else
            PlayerPrefs.SetString("MapDefaultLayer", "mapbox://styles/mapbox/streets-v10");
    }

    public void VideoCompressionChanged(bool val)
    {
        if (VideoCompressionToggle.isOn)
            PlayerPrefs.SetInt("VideoCompression", 1);
        else
            PlayerPrefs.SetInt("VideoCompression", 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        RosBridgeUrlInputField.text = PlayerPrefs.GetString("RosBridgeURL");
        MapCenterInputField.text = PlayerPrefs.GetString("MapCenter");
        MapSizeSlider.value = PlayerPrefs.GetInt("MapSize");
        CamResWidthInputField.text = PlayerPrefs.GetInt("CameraResWidth").ToString();
        CamResHeightInputField.text = PlayerPrefs.GetInt("CameraResHeight").ToString();
        CamFovInputField.text = PlayerPrefs.GetInt("CameraFOV").ToString();
        CameraScreenDistInputField.text = PlayerPrefs.GetFloat("CameraScreenDistance").ToString().Replace(',','.');
        TopicVideoField.text = PlayerPrefs.GetString("VideoTopic");

        AltitudeOffsetInputField.text = PlayerPrefs.GetFloat("AltitudeOffset").ToString().Replace(',', '.');

        float VideoTimeStep = PlayerPrefs.GetFloat("VideoTimeStep");
        if (VideoTimeStep == 0) VideoFPSField.text = "0";
        else VideoFPSField.text =  Mathf.RoundToInt(1.0f / VideoTimeStep).ToString();

        VideoCompressionToggle.isOn = PlayerPrefs.GetInt("VideoCompression") >0;
        SatelliteMapToggle.isOn = PlayerPrefs.GetString("MapDefaultLayer").Equals("mapbox.satellite");
    }

    public void StartFlightOnclick()
    {
        SceneManager.LoadScene("DroneScene", LoadSceneMode.Single);
    }

    public void SetDefaultsOnclick()
    {
        //if (EditorUtility.DisplayDialog("Set default values?","Are you sure?", "Yes, set defaults", "No, keep values"))
      
            SetDefaults(true);
            Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

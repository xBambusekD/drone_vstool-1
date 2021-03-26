using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.Unity.Map;
using TMPro;
using DroCo;

public class MapController : Singleton<MapController> {
    public AbstractMap Map;

    public TMP_InputField MapCenter;

    public Slider MapSizeSlider;
    private bool useSatelliteMap=true;

    // Use this for initialization
    void Start () {
        string mapLayer = PlayerPrefs.GetString("MapDefaultLayer");
        useSatelliteMap = (mapLayer == "mapbox.satellite");
        Map.ImageLayer.SetLayerSource(mapLayer);

        Map.Options.locationOptions.latitudeLongitude = PlayerPrefs.GetString("MapCenter");
        RangeTileProviderOptions options = Map.Options.extentOptions.GetTileProviderOptions() as RangeTileProviderOptions;
        options.west = options.south = options.north = options.east = 5;
        // options.west = options.south = options.north = options.east = PlayerPrefs.GetInt("MapSize");
        Invoke("AddMeshCollider",1);
        
    }
	void AddMeshCollider(){
        foreach(Transform child in transform){
           foreach(Transform building in child){
               building.gameObject.AddComponent<MeshCollider>();
           }
        } 
    }
	// Update is called once per frame
	void Update () {   
        if (Input.GetKeyUp("v"))
        {
            changeSource();
        }
    }

    public void changeSource(){
            if(useSatelliteMap) Map.ImageLayer.SetLayerSource("mapbox://styles/mapbox/streets-v10");
            else Map.ImageLayer.SetLayerSource("mapbox.satellite");
            useSatelliteMap = !useSatelliteMap;
    }
    public void changeMapCenter(){
        Map.Options.locationOptions.latitudeLongitude = MapCenter.text;
        PlayerPrefs.SetString("MapCenter",MapCenter.text);
        Map.UpdateMap();
    }

    public void changeMapSize(){
        Debug.Log(MapSizeSlider.value);
        Map.Options.locationOptions.latitudeLongitude = PlayerPrefs.GetString("MapCenter");
        RangeTileProviderOptions options = Map.Options.extentOptions.GetTileProviderOptions() as RangeTileProviderOptions;
        options.west = options.south = options.north = options.east = (int)MapSizeSlider.value;
        PlayerPrefs.SetInt("MapSize", (int)MapSizeSlider.value);
        Map.UpdateMap();
    }
}

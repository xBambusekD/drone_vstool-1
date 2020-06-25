using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;

public class MapController : MonoBehaviour {
    public AbstractMap Map;
    private bool useSatelliteMap=true;

    // Use this for initialization
    void Start () {
        string mapLayer = PlayerPrefs.GetString("MapDefaultLayer");
        useSatelliteMap = (mapLayer == "mapbox.satellite");
        Map.ImageLayer.SetLayerSource(mapLayer);

        Map.Options.locationOptions.latitudeLongitude = PlayerPrefs.GetString("MapCenter");
        RangeTileProviderOptions options = Map.Options.extentOptions.GetTileProviderOptions() as RangeTileProviderOptions;
        options.west = options.south = options.north = options.east = PlayerPrefs.GetInt("MapSize");
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
            if(useSatelliteMap) Map.ImageLayer.SetLayerSource("mapbox://styles/mapbox/streets-v10");
            else Map.ImageLayer.SetLayerSource("mapbox.satellite");
            useSatelliteMap = !useSatelliteMap;
        }
    }
}

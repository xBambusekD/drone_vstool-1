using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Map;
using UnityEngine;

public class MapManager : Singleton<MapManager> {

    public ArcGISMap Map;
    public ArcGISMapComponent MapComponent;

    private void Start() {
        
    }

}

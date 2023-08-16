using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Map;
using UnityEngine;

public class MapManager : Singleton<MapManager> {

    public enum Location {
        PRAGUE,
        BRNO,
        PILSEN,
        UNKNOWN
    }
        
    public ArcGISMapComponent ArcGISMap;
    public Location DefaultLocation;

    private void Start() {
        
    }

    public Location GetKnownLocation(GPS gpsPosition) {
        return Location.UNKNOWN;
    }

}

public class PragueData {

}

public class BrnoData {
    public static string BrnoElevation = "https://gis.brno.cz/ags1/rest/services/OMI/omi_dtm_2019_wgs_1m_e/ImageServer";
    public static string BrnoLOD1 = "https://gis.brno.cz/ags1/rest/services/Hosted/Brno_3D_LOD1_SJTSK/SceneServer";
    public static string BrnoLOD2 = "https://gis.brno.cz/ags1/rest/services/Hosted/3D_budovy_LOD2_WM/SceneServer";

    public PolygonCollider2D BrnoArea;

    public BrnoData() {
        BrnoArea = new PolygonCollider2D();
        BrnoArea.points = new Vector2[] { new Vector2(49.2635479f, 16.4276505f),
            new Vector2(49.3201989f, 16.6624832f),
            new Vector2(49.1345533f, 16.7644501f),
            new Vector2(49.0819618f, 16.5268707f)};
    }
}

public class PilsenData {
    public static string Pilsen1 = "https://tiles.arcgis.com/tiles/NUPWFrOotHlwVelG/arcgis/rest/services/3D_budovy_etapa_I_1/SceneServer";
    public static string Pilsen2 = "https://tiles.arcgis.com/tiles/NUPWFrOotHlwVelG/arcgis/rest/services/3D_budovy_etapa_I_2/SceneServer";
    public static string Pilsen3 = "https://tiles.arcgis.com/tiles/NUPWFrOotHlwVelG/arcgis/rest/services/3D_budovy_etapa_II/SceneServer";

    public PolygonCollider2D PilsenArea;

    public PilsenData() {
        PilsenArea = new PolygonCollider2D();
        PilsenArea.points = new Vector2[] { };
    }
}

public class OpenStreetMap {
    public static string OSM3D = "https://basemaps3d.arcgis.com/arcgis/rest/services/OpenStreetMap3D_Buildings_v1/SceneServer";
}

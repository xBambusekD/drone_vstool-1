using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mapbox.Unity.Map;
using System;
using DroCo;

public class MissionHandler : Singleton<MissionHandler>
{

    public AbstractMap Map;
    private string path;
    private string jsonContent;
    public GameObject newDrone;
    public Transform ourDrone;
    private int droneNumber = 1;
    public Transform dronesPanelGrid;
    public GameObject dronesPrefab;

    public Transform iconTransform;
    public GameObject icon;
    public GameObject WayPointPointerPrefab;
    public GameObject PopUp;
    public RenderTexture PopUpRenderTexture;
    private float groundAltitude;

    public float speed;
    public GameObject arrow;

    public GameObject currentObjectiveGO;
    public Mission mission;
    public GameObject confirmButton;
    public List<int> droneIndexes;

    public TextMeshProUGUI description;

    public Transform selectorParent;
    public GameObject defaultDrone;
    private bool checkedPoint = false;
    private bool simulationEnabled = true;

    public InsideZoneChecker InsideZoneChecker;

    Mesh mesh;
    Vector3[] vertices;
    int [] triangles;

    private bool parsePass = true;



    // Defaultly the first drone is 
    public static int activeDrone = 0;
    public void AddDrone(Vector3 position){ 
        GameObject Clone = Instantiate(newDrone, position, ourDrone.rotation);
        Clone.name = "DroneObject" + droneNumber.ToString();
        Drones.drones.Add(new Drone(Clone, new DroneFlightData()));
        Drones.DroneAdded(dronesPanelGrid,dronesPrefab,iconTransform,icon,PopUp, PopUpRenderTexture);
        droneNumber++;
        Clone.transform.SetParent(transform);
    }

    private void FillIndexes(){
        for(int f = 0 ; f < mission.drones.Count; f++){
            droneIndexes.Add(f);
        }
    }
    
    private void LoadJsonData(){
        path = Application.streamingAssetsPath + "/mission.json";
        jsonContent =File.ReadAllText(path);
        try
        {
            mission = JsonUtility.FromJson<Mission>(jsonContent);
        }
        catch (System.Exception)
        {
            parsePass = false;
        }
       
    }

    void SetupDrones(){
        int i = 0;
        foreach(var item in mission.drones){
            Vector3 position3d;
            Mapbox.Utils.Vector2d mapboxPosition = new Mapbox.Utils.Vector2d(item.latitude,item.longitude);
            position3d = Map.GeoToWorldPosition(mapboxPosition,false);
            groundAltitude = Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(position3d));
            position3d.y = groundAltitude;
            // Defaultne je prvy dron ten co uz je v scene
            if(i != activeDrone)
                AddDrone(position3d);
            i++;
        }        
    }

    private void GeneratePoint(Checkpoint item){
        GameObject WayPointPointerPrefab;
        WayPointPointerPrefab = Resources.Load<GameObject>("Zones/WayPointPointer");
        GameObject WayPointPointer = Instantiate(WayPointPointerPrefab);
        WayPointPointer.transform.SetParent(transform);
                    // Vytvor vektor z gps
        Mapbox.Utils.Vector2d p = new Mapbox.Utils.Vector2d(item.points[0].latitude,item.points[0].longitude);
                    // // Ziskaj poziciu
        Vector3 position = Map.GeoToWorldPosition(p,false);
                    // Ziskaj vysku
        groundAltitude = Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(position));
        position.y = groundAltitude + (float)item.points[0].height;
        WayPointPointer.transform.position = position;
        WayPointPointer.name = item.name;
        item.points[0].pointGameObject = WayPointPointer;
    }


    private void GenerateZone(Checkpoint checkpoint){

        GameObject ZoneGameObject = new GameObject(checkpoint.name);
        ZoneGameObject.transform.position = new Vector3(0,0,0);
        ZoneGameObject.transform.SetParent(transform);
        ZoneGameObject.layer =13;
        Color yellow = new Color(1.0F, 0.9333333F, 0.0F, 0.25F);
        Color red = new Color(1.0F, 0.0F, 0.0F, 0.25F);
        ZoneGameObject.AddComponent<MeshFilter>();
        ZoneGameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Zones/WayPointMaterial");
        if(checkpoint.drones.Count > 0)
            ZoneGameObject.GetComponent<MeshRenderer>().material.SetColor("_Color",yellow);
        else
            ZoneGameObject.GetComponent<MeshRenderer>().material.SetColor("_Color",red);
        List <Vector3> points = new List<Vector3>();
         

        // Minimalna groundAltitude aby zona nelevitovala
        float minAltitude = float.MaxValue;

        // Ziskam list bodov zony
        int j = 0;
        foreach(var point in checkpoint.points){
            GameObject WayPointPointer = Instantiate(Resources.Load<GameObject>("Zones/ZoneWall")); 
            WayPointPointer.transform.SetParent(ZoneGameObject.transform);

            // Vytvor vektor z gps
            Mapbox.Utils.Vector2d position2d = new Mapbox.Utils.Vector2d(point.latitude,point.longitude);
            // // Ziskaj poziciu
            Vector3 position = Map.GeoToWorldPosition(position2d,false);
            // Ziskaj vysku
            groundAltitude = Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(position));
            if(groundAltitude < minAltitude)
                minAltitude= groundAltitude;
            position.y = groundAltitude + (float)point.height;
            points.Add(position);
            WayPointPointer.transform.position = position;
            checkpoint.points[j].pointGameObject = WayPointPointer;
            j++;
        }

        mesh = new Mesh();
        ZoneGameObject.GetComponent<MeshFilter>().mesh = mesh;
        // Pridaj samostatny prvok^^ TODO
        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();
        int multiplier;

        // Ziskam pointy a trojuholniky z nich
        for(int i = 0; i < points.Count-1; i++){
            multiplier = i*4;
            
            verticesList.AddRange(new List<Vector3>(){
                points[i],
                points[i+1],
                new Vector3(points[i].x,minAltitude,points[i].z),
                new Vector3(points[i+1].x,minAltitude,points[i+1].z)
            });


            trianglesList.AddRange(new int[]{
                0+multiplier ,1+multiplier ,2+multiplier,
                0+multiplier ,2+multiplier ,1+multiplier,
                1+multiplier ,2+multiplier ,3+multiplier,
                1+multiplier ,3+multiplier ,2+multiplier
            });
        } 

        // Poslednu stenu treba spojit s prvou
        multiplier = (points.Count-1)*4;
        verticesList.AddRange(new List<Vector3>(){
            points[points.Count-1],
            points[0],
            new Vector3(points[points.Count-1].x,minAltitude,points[points.Count-1].z),
            new Vector3(points[0].x,minAltitude,points[0].z)
        });


        trianglesList.AddRange(new int[]{
            0+multiplier ,1+multiplier ,2+multiplier,
            0+multiplier ,2+multiplier ,1+multiplier,
            1+multiplier ,2+multiplier ,3+multiplier,
            1+multiplier ,3+multiplier ,2+multiplier
        });

        vertices = verticesList.ToArray();
        triangles = trianglesList.ToArray();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;  
        
        GenerateMiddlePoint(checkpoint).transform.SetParent(ZoneGameObject.transform);
        checkpoint.ZoneGameObject = ZoneGameObject;
    }

    GameObject GenerateMiddlePoint(Checkpoint checkpoint){
        GameObject Middle = Instantiate(Resources.Load<GameObject>("Zones/ZoneWall"));
        Vector3 center = new Vector3(0, 0, 0);
        float count = 0;
        foreach (var pointOfZone in checkpoint.points){
            center += pointOfZone.pointGameObject.transform.position;
            count++;
        }
        var theCenter = center / count;
        Middle.transform.position = theCenter;
        Middle.name = checkpoint.name;
        
        Point centerPoint = new Point();
        centerPoint.pointGameObject = Middle;
        checkpoint.points.Add(centerPoint);
        return Middle;
    }

    void ManageSidePanelInfo(){
        int i = 0;
        foreach (Transform child in dronesPanelGrid.transform)
        {
            TextMeshProUGUI height = child.Find("Height").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI objective = child.Find("Objective").GetComponent<TextMeshProUGUI>();
            float droneAltitude = 0.0f;
            droneAltitude = Drones.drones[i].DroneGameObject.transform.localPosition.y - Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(Drones.drones[i].DroneGameObject.transform.position));
            height.text = "Height:" + Mathf.Round(droneAltitude) + "m";
            objective.text = "Objective: " + mission.drones[i].checkpoints[0].name;
            i++;
        }

       
    }

    void Start()
    {
        LoadJsonData();
        if(parsePass){
            FillIndexes();
            // Nastavime drony
            SetupDrones();

            foreach(var checkpoint in mission.checkpoints){
                checkpoint.droneCount = checkpoint.drones.Count;
                // Drony dostanu misiu
                if(checkpoint.type == "regular" || checkpoint.type == "confirm"){
                    GeneratePoint(checkpoint);
                } else if(checkpoint.type == "zone"){
                    GenerateZone(checkpoint);
                    Debug.Log(checkpoint.name);
                }

                // Priradim kompetentnym dronom missiu
                foreach(var name in checkpoint.drones){
                    foreach(var drone in mission.drones){
                        if(drone.name == name){
                            drone.checkpoints.Add(checkpoint);
                        }
                    }
                }
            }
        }

    }

    public DroneFlightData jsonData;
    public float offset = 1.0F;
    public float zoneOffset = 100.0F;
    void Update()
    {
        if(parsePass){
        AssignCurrentObjective();
        ManageSidePanelInfo();
        bool insideZone = false;
        int i = 0;
        foreach (var drone in mission.drones)
        {
            //Check if mission isnt over
            if(drone.checkpoints.Count > 0){
                Vector3 dronePosition = Drones.drones[i].DroneGameObject.transform.position;
                int last_index = drone.checkpoints[0].points.Count - 1;
                Vector3 objectivePosition = drone.checkpoints[0].points[last_index].pointGameObject.transform.position;
                // SETTING
                if(drone.checkpoints[0].type == "zone"){
                    List<InsideZoneChecker.Point2d> ZonePoints = new List<InsideZoneChecker.Point2d>();
                    foreach(var point in drone.checkpoints[0].points){
                        InsideZoneChecker.Point2d tmpPoint = new InsideZoneChecker.Point2d((int)point.pointGameObject.transform.position.x,(int)point.pointGameObject.transform.position.z);
                        ZonePoints.Add(tmpPoint);
                    }
                    InsideZoneChecker.Point2d []polygon1 = ZonePoints.ToArray();
                    InsideZoneChecker.Point2d point1 = new InsideZoneChecker.Point2d((int)dronePosition.x,(int)dronePosition.z);
                    int n = polygon1.Length;


                    if (InsideZoneChecker.isInside(polygon1, n, point1))
                        insideZone = true;
                    else
                        insideZone = false;

                    if(i == activeDrone){
                        var zone = mission.checkpoints.Find(Point => Point.name == drone.checkpoints[0].name);
                        zone.ZoneGameObject.SetActive(true);
                    }

                }
                // Regular Point in Space
                    // Drone is away from point
                    // if ((drone.checkpoints[0].type == "regular" && Vector3.Distance(dronePosition, objectivePosition) > offset  )||(drone.checkpoints[0].type == "zone")){
                    if ((drone.checkpoints[0].type == "regular" && Vector3.Distance(dronePosition, objectivePosition) > offset  )||(drone.checkpoints[0].type == "confirm" && Vector3.Distance(dronePosition, objectivePosition) > offset  )||(drone.checkpoints[0].type == "zone" && !insideZone)){
                        if(i == activeDrone){
                            foreach (Transform child in selectorParent){
                                child.GetComponent<Image>().color = new Color(0.3301887F, 0.3301887F, 0.3301887F, 0.6666667F);
                            }
                            if(drone.checkpoints[0].type == "regular")
                                description.text = "Reach " + drone.checkpoints[0].name;
                            else if(drone.checkpoints[0].description != null)
                                description.text = drone.checkpoints[0].description;
                            else
                                description.text = "Reach " + drone.checkpoints[0].name;
                        } else {
                            // Chod k pointu
                            if(mission.drones[i].url != null){
                                StartCoroutine(getData(mission.drones[i].url));
                                Vector3 position3d;
                                Mapbox.Utils.Vector2d mapboxPosition = new Mapbox.Utils.Vector2d(jsonData.Latitude,jsonData.Longitude);
                                position3d = Map.GeoToWorldPosition(mapboxPosition,false);
                                groundAltitude = Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(position3d));
                                position3d.y = groundAltitude + (float)jsonData.Altitude;
                                Drones.drones[i].DroneGameObject.transform.position = position3d;
                            }
                            else 
                            if(simulationEnabled){
                                Drones.drones[i].DroneGameObject.transform.position = Vector3.MoveTowards(dronePosition, objectivePosition, Time.deltaTime * speed);
                                Drones.drones[i].DroneGameObject.transform.LookAt(objectivePosition);
                            }
                        }
                    // Drone is at the point
                    } else {
                        if (droneIndexes.Contains(i)){
                            if(i == activeDrone && drone.checkpoints[0].type == "confirm"){
                                confirmButton.SetActive(true);
                                if(checkedPoint){
                                    droneIndexes.Remove(i);
                            // Odcitam ten index
                                mission.checkpoints.Find(Point => Point.name == drone.checkpoints[0].name).droneCount--;
                                Debug.Log(mission.checkpoints.Find(Point => Point.name == drone.checkpoints[0].name).droneCount);

                                checkedPoint =false;
                                confirmButton.SetActive(false);
                                }
                            }else{
                                 droneIndexes.Remove(i);
                            // Odcitam ten index
                                mission.checkpoints.Find(Point => Point.name == drone.checkpoints[0].name).droneCount--;
                                Debug.Log(mission.checkpoints.Find(Point => Point.name == drone.checkpoints[0].name).droneCount);
                            }
                           
                        }

                        if(i == activeDrone){
                            if(drone.checkpoints[0].type == "confirm" && checkedPoint == true)
                                description.text = "Wait for other drones";
                            else if(drone.checkpoints[0].type == "confirm" && checkedPoint == false)
                                description.text = drone.checkpoints[0].description + " AND CONFIRM";
                            int j = 0;
                            foreach (Transform child in selectorParent){
                                if (droneIndexes.Contains(j) && drone.checkpoints[0].drones.Contains(mission.drones[j].name)){
                                    child.GetComponent<Image>().color = new Color(0.5660378F, 0.06674973F, 0.06674973F, 6666667F);
                                }
                                else{
                                    child.GetComponent<Image>().color = new Color(0.3301887F, 0.3301887F, 0.3301887F, 0.6666667F);
                                }
                                j++;
                            }
                        }
                        
                        if (mission.checkpoints.Find(Point => Point.name == drone.checkpoints[0].name).droneCount == 0){
                            droneIndexes.Add(i);
                            if(drone.checkpoints[0].type == "zone"){
                                var zone = mission.checkpoints.Find(Point => Point.name == drone.checkpoints[0].name);
                                Color green = new Color(0.03921568F, 0.9333333F, 0.1818267F, 0.25F);
                                zone.ZoneGameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", green);
                            }
                           
                            drone.checkpoints.RemoveAt(0);   
                        }
                    }
            }
            i++;
        }
        }
        
    }
    public void checkedPointConfirm()
    {
        checkedPoint =true;
    }
    IEnumerator getData(string url){
        WWW _www = new WWW(url);
        yield return _www;
        if(_www.error == null){
            processJsonData(_www.text);
        } else {
            // Debug.Log("error");
        }
    }

    private void processJsonData(string _url){
        jsonData = JsonUtility.FromJson<DroneFlightData>(_url);

    }
    private void AssignCurrentObjective()
    {
        if (mission.drones[activeDrone].checkpoints.Count > 0)
            if (mission.drones[activeDrone].checkpoints[0].type == "regular" || mission.drones[activeDrone].checkpoints[0].type == "confirm")
                currentObjectiveGO.transform.position = mission.drones[activeDrone].checkpoints[0].points[0].pointGameObject.transform.position;
            else if (mission.drones[activeDrone].checkpoints[0].type == "zone")
                currentObjectiveGO.transform.position = mission.drones[activeDrone].checkpoints[0].points[mission.drones[0].checkpoints[0].points.Count - 1].pointGameObject.transform.position; // Last is Middle of Zone
    }

    public void destroyMission(){
        foreach(Transform child in transform){
            Destroy(child.gameObject);
        }
    }

}

[System.Serializable]
public class Mission{
    public List<DroneM> drones;
    public List<Checkpoint> checkpoints;
}
[System.Serializable]
public class Checkpoint
{
    public GameObject ZoneGameObject;
    public string type;
    public string name;
    public List <Point> points;
    
    public List <string> drones;

    public int droneCount;

    public string description;
}

[System.Serializable]
public class Point{
    public GameObject pointGameObject;
    public double height;
    public double latitude;
    public double longitude;    
}

[System.Serializable]
public class DroneM{
    public string name;
    public double latitude;
    public double longitude;

    public string url;
    public List <Checkpoint> checkpoints;
}

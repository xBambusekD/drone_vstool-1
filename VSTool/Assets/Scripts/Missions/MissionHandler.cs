using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mapbox.Unity.Map;
using System; 
  



public class MissionHandler : MonoBehaviour
{
 // Define Infinite (Using INT_MAX  
    // caused overflow problems) 
    static int INF = 10000; 
  
    public class Point2d  
    { 
        public int x; 
        public int y; 
  
        public Point2d(int x, int y) 
        { 
            this.x = x; 
            this.y = y; 
        } 
    }; 
  
    // Given three colinear Point2ds p, q, r,  
    // the function checks if Point2d q lies 
    // on line segment 'pr' 
    static bool onSegment(Point2d p, Point2d q, Point2d r)  
    { 
        if (q.x <= Math.Max(p.x, r.x) && 
            q.x >= Math.Min(p.x, r.x) && 
            q.y <= Math.Max(p.y, r.y) && 
            q.y >= Math.Min(p.y, r.y)) 
        { 
            return true; 
        } 
        return false; 
    } 
  
    // To find orientation of ordered triplet (p, q, r). 
    // The function returns following values 
    // 0 --> p, q and r are colinear 
    // 1 --> Clockwise 
    // 2 --> Counterclockwise 
    static int orientation(Point2d p, Point2d q, Point2d r)  
    { 
        int val = (q.y - p.y) * (r.x - q.x) -  
                  (q.x - p.x) * (r.y - q.y); 
  
        if (val == 0)  
        { 
            return 0; // colinear 
        } 
        return (val > 0) ? 1 : 2; // clock or counterclock wise 
    } 
  
    // The function that returns true if  
    // line segment 'p1q1' and 'p2q2' intersect. 
    static bool doIntersect(Point2d p1, Point2d q1,  
                            Point2d p2, Point2d q2)  
    { 
        // Find the four orientations needed for  
        // general and special cases 
        int o1 = orientation(p1, q1, p2); 
        int o2 = orientation(p1, q1, q2); 
        int o3 = orientation(p2, q2, p1); 
        int o4 = orientation(p2, q2, q1); 
  
        // General case 
        if (o1 != o2 && o3 != o4) 
        { 
            return true; 
        } 
  
        // Special Cases 
        // p1, q1 and p2 are colinear and 
        // p2 lies on segment p1q1 
        if (o1 == 0 && onSegment(p1, p2, q1))  
        { 
            return true; 
        } 
  
        // p1, q1 and p2 are colinear and 
        // q2 lies on segment p1q1 
        if (o2 == 0 && onSegment(p1, q2, q1))  
        { 
            return true; 
        } 
  
        // p2, q2 and p1 are colinear and 
        // p1 lies on segment p2q2 
        if (o3 == 0 && onSegment(p2, p1, q2)) 
        { 
            return true; 
        } 
  
        // p2, q2 and q1 are colinear and 
        // q1 lies on segment p2q2 
        if (o4 == 0 && onSegment(p2, q1, q2)) 
        { 
            return true; 
        } 
  
        // Doesn't fall in any of the above cases 
        return false;  
    } 
  
    // Returns true if the Point2d p lies  
    // inside the polygon[] with n vertices 
    static bool isInside(Point2d []polygon, int n, Point2d p) 
    { 
        // There must be at least 3 vertices in polygon[] 
        if (n < 3)  
        { 
            return false; 
        } 
  
        // Create a Point2d for line segment from p to infinite 
        Point2d extreme = new Point2d(INF, p.y); 
  
        // Count intersections of the above line  
        // with sides of polygon 
        int count = 0, i = 0; 
        do
        { 
            int next = (i + 1) % n; 
  
            // Check if the line segment from 'p' to  
            // 'extreme' intersects with the line  
            // segment from 'polygon[i]' to 'polygon[next]' 
            if (doIntersect(polygon[i],  
                            polygon[next], p, extreme))  
            { 
                // If the Point2d 'p' is colinear with line  
                // segment 'i-next', then check if it lies  
                // on segment. If it lies, return true, otherwise false 
                if (orientation(polygon[i], p, polygon[next]) == 0) 
                { 
                    return onSegment(polygon[i], p, 
                                     polygon[next]); 
                } 
                count++; 
            } 
            i = next; 
        } while (i != 0); 
  
        // Return true if count is odd, false otherwise 
        return (count % 2 == 1); // Same as (count%2 == 1) 
    } 


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

    public List<int> droneIndexes;

    public TextMeshProUGUI description;

    public Transform selectorParent;
    public GameObject defaultDrone;

    private bool simulationEnabled = false;

    private InsideZoneChecker InsideZoneChecker;

    Mesh mesh;
    Vector3[] vertices;
    int [] triangles;

    private bool parsePass = true;



    // Defaultly the first drone is 
    public static int activeDrone = 0;
    public void AddDrone(Vector3 position){ 
        GameObject Clone = Instantiate(newDrone, position, ourDrone.rotation);
        Clone.name = "DroneObject" + droneNumber.ToString();
        Drones.drones.Add(Clone);
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
        ZoneGameObject.AddComponent<MeshFilter>();
        ZoneGameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Zones/WayPointMaterial");
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
        checkpoint.ZoneGameObject.SetActive(false);
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
            droneAltitude = Drones.drones[i].transform.localPosition.y - Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(Drones.drones[i].transform.position));
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
                if(checkpoint.type == "regular"){
                    GeneratePoint(checkpoint);
                } else if(checkpoint.type == "zone"){
                    GenerateZone(checkpoint);
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

    public jsonUrlDataClass jsonData;
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
                Vector3 dronePosition = Drones.drones[i].transform.position;
                int last_index = drone.checkpoints[0].points.Count - 1;
                Vector3 objectivePosition = drone.checkpoints[0].points[last_index].pointGameObject.transform.position;
                // SETTING
                if(drone.checkpoints[0].type == "zone"){
                    List<Point2d> ZonePoints = new List<Point2d>();
                    foreach(var point in drone.checkpoints[0].points){
                        Point2d tmpPoint = new Point2d((int)point.pointGameObject.transform.position.x,(int)point.pointGameObject.transform.position.z);
                        ZonePoints.Add(tmpPoint);
                        
                    }
                    Point2d []polygon1 = ZonePoints.ToArray();
                    Point2d point1 = new Point2d((int)dronePosition.x,(int)dronePosition.z);
                    int n = polygon1.Length;


                    if (isInside(polygon1, n, point1))
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
                    if ((drone.checkpoints[0].type == "regular" && Vector3.Distance(dronePosition, objectivePosition) > offset  )||(drone.checkpoints[0].type == "zone" && !insideZone)){
                        if(i == activeDrone){
                            foreach (Transform child in selectorParent){
                                child.GetComponent<Image>().color = new Color(0.3301887F, 0.3301887F, 0.3301887F, 0.6666667F);
                            }
                            description.text = "Reach " + drone.checkpoints[0].name;
                        } else {
                            // Chod k pointu
                            if(mission.drones[i].url != null){
                                StartCoroutine(getData(mission.drones[i].url));
                                Vector3 position3d;
                                Mapbox.Utils.Vector2d mapboxPosition = new Mapbox.Utils.Vector2d(jsonData.latitude,jsonData.longitude);
                                position3d = Map.GeoToWorldPosition(mapboxPosition,false);
                                groundAltitude = Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(position3d));
                                position3d.y = groundAltitude + (float)jsonData.height;
                                Drones.drones[i].transform.position = position3d;
                            }
                            else 
                            if(simulationEnabled){
                                Drones.drones[i].transform.position = Vector3.MoveTowards(dronePosition, objectivePosition, Time.deltaTime * speed);
                                Drones.drones[i].transform.LookAt(objectivePosition);
                            }
                        }
                    // Drone is at the point
                    } else {
                        if (droneIndexes.Contains(i)){
                            droneIndexes.Remove(i);
                            // Odcitam ten index
                            mission.checkpoints.Find(Point => Point.name == drone.checkpoints[0].name).droneCount--;
                            Debug.Log(mission.checkpoints.Find(Point => Point.name == drone.checkpoints[0].name).droneCount);
                        }

                        if(i == activeDrone){
                            description.text = "Wait for other drones";
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
        jsonData = JsonUtility.FromJson<jsonUrlDataClass>(_url);

    }
    private void AssignCurrentObjective()
    {
        if (mission.drones[activeDrone].checkpoints.Count > 0)
            if (mission.drones[activeDrone].checkpoints[0].type == "regular")
                currentObjectiveGO.transform.position = mission.drones[activeDrone].checkpoints[0].points[0].pointGameObject.transform.position;
            else if (mission.drones[activeDrone].checkpoints[0].type == "zone")
                currentObjectiveGO.transform.position = mission.drones[activeDrone].checkpoints[0].points[mission.drones[0].checkpoints[0].points.Count - 1].pointGameObject.transform.position; // Last is Middle of Zone
    }
}
[Serializable]
public class jsonUrlDataClass{
    public double height;
    public double latitude;
    public double longitude;
}

[System.Serializable]
public class Mission{
    public List<Drone> drones;
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
}

[System.Serializable]
public class Point{
    public GameObject pointGameObject;
    public double height;
    public double latitude;
    public double longitude;    
}

[System.Serializable]
public class Drone{
    public string name;
    public double latitude;
    public double longitude;

    public string url;
    public List <Checkpoint> checkpoints;
}

using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Modifiers;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using System.Collections;

public class MapClickController : MonoBehaviour {

    public enum MapClickMode
    {
        Off,
        WayPoints,
        Zones
    }

    public MapClickMode mode = MapClickMode.Off;

    Ray ray;
    RaycastHit hit;
    LayerMask tarrainMask;
    LayerMask zonePointMask;

    private float clicktime;

    private GameObject DraggedPoint;

    private GuiController guiController;
    public GameObject guiCanvas;

    public GameObject zoneObject;
    private ZoneController zoneController;

    public Camera cam;

    Material _material;

    // Use this for initialization
    void Start () {
        tarrainMask = LayerMask.GetMask("terrain");
        zonePointMask = LayerMask.GetMask("zonePoint");

        guiController = guiCanvas.GetComponent<GuiController>();
        zoneController = zoneObject.GetComponent<ZoneController>();
        DraggedPoint = null;
    }

    void updateRay() //volat před každým rayTracingem
    {
        
        //if (ChangeCamera.currentCam != null) cam = ChangeCamera.currentCam;
        //else
        //cam = Camera.main;

        ray = cam.ScreenPointToRay(Input.mousePosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (mode != MapClickMode.Off)
        {
            bool click = false;

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("GetMouseButtonDown"); 
                clicktime = Time.time; //možný začátek kliknutí

                updateRay();
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, zonePointMask))
                {
                    Debug.Log("kliknuto na point" + hit.transform.gameObject);
                    DraggedPoint = hit.transform.gameObject;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("GetMouseButtonUp");

                if (Time.time - clicktime < 0.15f && DraggedPoint == null)
                {
                    click = true;
                }

                DraggedPoint = null;
            }

            if (click || DraggedPoint != null)
            {
                updateRay();
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, tarrainMask))
                {
                    /*
                    Debug.Log(hit.point);
                    Debug.Log("hit distance: " + hit.distance);
                    Debug.Log(hit.collider);
                    */
                    if (click)
                    {
                        Debug.Log("Click detected");
                        if (mode == MapClickMode.Zones) OnClickInZoneMode(hit.point, hit.distance);
                        if (mode == MapClickMode.WayPoints) OnClickInWaypointMode(hit.point, hit.distance);
                    }

                    if (DraggedPoint != null)
                    {
                        DraggedPoint.transform.position = hit.point;
                        //ZonePointData pointScript = DraggedPoint.GetComponent("ZonePointData") as ZonePointData;
                        //pointScript.point = hit.point;
                        zoneController.OnPointDrag();
                    }
                }
            }
        }
    }

    //kliknuto v režimu přídávání WayPoint bodů 
    private void OnClickInWaypointMode(Vector3 position, float distance)
    {
        Debug.Log("OnClickInWaypointMode");
        guiController.OnMapClickInWaypointMode(position, distance);
    }

    //kliknuto v režimu přídávání bodů zón
    private void OnClickInZoneMode(Vector3 position, float distance)
    {
        Debug.Log("OnClickInZoneMode");
        zoneController.OnClickInZoneMode(position, distance);
        /*
        GameObject point = Instantiate(pointer, hit.point, Quaternion.identity);
        point.transform.parent = Zones.transform; 
        ZonePointData pointScript = point.GetComponent("ZonePointData") as ZonePointData;

        polygonPoints.Add(point);
        DrawZone(polygonPoints);
        */
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class MissionManager : Singleton<MissionManager> {

    private List<DistanceBillboard> distancesList = new List<DistanceBillboard>();

    [SerializeField]
    private Transform ExperimentScene;
    [SerializeField]
    private GameObject ActionPointPrefab;
    [SerializeField]
    private Material ConnectionMaterial;
    [SerializeField]
    private GameObject WaypointPrefab;

    private GameObject APInstance;
    private Collider apCollider;

    private List<Waypoint> missionWaypoints = new List<Waypoint>();



    private void Start() {
        APInstance = Instantiate(ActionPointPrefab, ExperimentScene);
        APInstance.SetActive(false);
        apCollider = APInstance.GetComponentInChildren<Collider>();
        apCollider.enabled = false;
    }

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (hit.collider != null) {
                if (hit.collider.tag.Equals("Connection")) {
                    APInstance.SetActive(true);
                    APInstance.transform.position = hit.point;
                    if (Mouse.current.leftButton.wasPressedThisFrame) {
                        CreateAP(hit.collider.gameObject.GetComponent<Connection>());
                    }
                } else {
                    APInstance.SetActive(false);
                }
            }
        }
    }

    private void CreateAP(Connection connection) {
        GameObject newAP = Instantiate(ActionPointPrefab, ExperimentScene);
        newAP.transform.position = APInstance.transform.position;
        ActionPoint ap = newAP.GetComponent<ActionPoint>();
        RectTransform[] targets = connection.target;

        ConnectionManager.CreateConnection(targets[1], ap.ConnectionBinder);
        Connection newConnection = ConnectionManager.FindConnection(targets[1], ap.ConnectionBinder);
        newConnection.points[0].direction = ConnectionPoint.ConnectionDirection.East;
        newConnection.points[1].direction = ConnectionPoint.ConnectionDirection.West;
        LineRenderer lineRend = newConnection.GetComponent<LineRenderer>();
        lineRend.material = ConnectionMaterial;
        lineRend.startWidth = 0.3f;
        lineRend.endWidth = 0.3f;
        newConnection.gameObject.layer = 12;

        connection.SetTargets(ap.ConnectionBinder, targets[0]);
        connection.GetComponent<BoxCollider>().enabled = false;

    }

    public void AddToDistanceList(DistanceBillboard billboard) {
        distancesList.Add(billboard);
    }

    public void ChangeTargetFaceCamera(Transform target) {
        foreach (DistanceBillboard billboard in distancesList) {
            billboard.ObjectToFace = target;
        }
    }

    public void ChangeTarget(Transform target) {
        foreach (DistanceBillboard billboard in distancesList) {
            billboard.ObjectToComputeDistance = target;
        }
    }

    public void LoadMission() {
        StartCoroutine(DownloadMission());
    }

    public IEnumerator DownloadMission(string url = "https://nextcloud.fit.vutbr.cz/s/wA7FzncKQJWRBGw/download/AR_test.kml") {
        UnityWebRequest www;

        Debug.Log("DOWNLOADING MISSION");

        try {
            www = UnityWebRequest.Get(url);
        } catch (WebException ex) {
            Debug.LogException(ex);
            yield break;
        }

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error + " (" + url + ")");
        } else {
            OnMissionDownloaded(www.downloadHandler.text);
        }
    }

    private void OnMissionDownloaded(string kml) {
        //Destroy previous mission
        DestroyMission();

        string startKeyword = "<coordinates>";
        string endKeyword = "</coordinates>";
        int start = kml.IndexOf(startKeyword);
        int end = kml.IndexOf(endKeyword);
        string missionStr = kml.Substring(start + startKeyword.Length, end - start - startKeyword.Length);

        string[] points = missionStr.Split(" ");

        foreach (string point in points) {
            string[] coords = point.Split(",");
            missionWaypoints.Add(new Waypoint(new GPS() { longitude = double.Parse(coords[0]), latitude = double.Parse(coords[1]) }, double.Parse(coords[2])));
        }

        SpawnMission();
    }

    private void SpawnMission() {
        WaypointGameObject previousWaypoint = null;
        int i = 0;
        foreach(Waypoint waypoint in missionWaypoints) {
            GameObject point = Instantiate(WaypointPrefab, GameManager.Instance.Scene3DView.transform);
            ArcGISLocationComponent pointLocation = point.AddComponent<ArcGISLocationComponent>();
            pointLocation.Position = new ArcGISPoint(waypoint.Coordinates.longitude, waypoint.Coordinates.latitude, waypoint.Altitude, new ArcGISSpatialReference(4326));
            WaypointGameObject waypointGO = point.GetComponent<WaypointGameObject>();
            waypointGO.SetText(i.ToString());
            i++;

            // Make connection between waypoints
            if (previousWaypoint != null) {
                ConnectionManager.CreateConnection(previousWaypoint.ConnectionBinder, waypointGO.ConnectionBinder);
            }

            previousWaypoint = waypointGO;

            waypoint.SetVisual(point);
        }
    }

    private void DestroyMission() {
        foreach (Waypoint waypoint in missionWaypoints) {
            waypoint.DestroyVisual();
        }

        missionWaypoints.Clear();
    }

}

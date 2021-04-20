/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    private List<GameObject> polygonPoints;

    private GameObject lineGeneratorPrefab;

    private GameObject pointer;
    private GameObject actZone;
    public GameObject Zones;

    // Start is called before the first frame update
    void Start()
    {
        polygonPoints = new List<GameObject>();
        actZone = new GameObject();
        actZone.name = "zones";
        actZone.transform.parent = Zones.transform;

        lineGeneratorPrefab = Resources.Load<GameObject>("Prefabs/ZoneLine");
        pointer = Resources.Load<GameObject>("Prefabs/Capsule");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickInZoneMode(Vector3 position, float distance)
    {
        Debug.Log("ZoneController.OnClickInZoneMode");
        GameObject point = Instantiate(pointer, position, Quaternion.identity);
        point.transform.parent = Zones.transform;
        ZonePointData pointScript = point.GetComponent("ZonePointData") as ZonePointData;

        polygonPoints.Add(point);
        DrawZone(polygonPoints);
    }

    public void OnPointDrag()
    {
        Debug.Log("ZoneController.OnPointDrag");
        DrawZone(polygonPoints);
    }

    private void DrawZone(List<GameObject> polygonGroundPoints)
    {
        Destroy(actZone);
        actZone = new GameObject();
        actZone.name = "zones";
        actZone.transform.parent = Zones.transform;

        Material newMat = Resources.Load("Materials/ZoneMaterialGreen", typeof(Material)) as Material;

        if (polygonPoints.Count > 1)
            for (int i = 0; i < polygonPoints.Count; i++)
            {
                DrawZoneWall(polygonPoints[i].transform.position, polygonPoints[(i + 1) % polygonPoints.Count].transform.position, newMat, 100.0f);
            }
    }


    private void DrawZoneWall(Vector3 pointA, Vector3 pointB, Material material, float height)
    {
        int lineDistance = 1;

        Vector3 direction = pointB - pointA;
        direction.Normalize();
        direction = direction * lineDistance;

        Vector3 addHeight = new Vector3(0.1f, height, 0.1f);

        //svisle hranice
        for (Vector3 point = pointA; Vector3.Distance(point, pointB) > lineDistance; point += direction)
        {
            SpawnLineGenerator(new Vector3[2] { point, point + addHeight }, material);
        }

        //vodorovne    
        for (Vector3 lineHeight = addHeight; lineHeight.y > 0; lineHeight -= new Vector3(0, lineDistance, 0))
        {
            SpawnLineGenerator(new Vector3[2] { (pointA + lineHeight), pointB + lineHeight }, material);
        }
    }


    private void SpawnLineGenerator(Vector3[] linePoints, Material material)
    {
        // Create new LineHolder object.
        GameObject newLineGen = Instantiate(lineGeneratorPrefab);
        // Get reference to newLineGen's LineRenderer.
        LineRenderer lRend = newLineGen.GetComponent<LineRenderer>();

        // Set amount of LineRenderer positions = amount of line point positions.
        lRend.positionCount = linePoints.Length;
        // Set positions of LineRenderer using linePoints array.
        lRend.SetPositions(linePoints);
        lRend.transform.parent = actZone.transform;
        lRend.name = "zoneBarrier";
        lRend.material = material;
    }
}

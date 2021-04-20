using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mapbox.Unity.Map;
using System;
using System.Linq;

public class LoadManager : MonoBehaviour
{
    public Slider transparencySlider;
    int MaxVerts = 3000;
    public GameObject LoaderGameObject;

    public void OpenLoader()
    {
        LoaderGameObject.SetActive(!LoaderGameObject.activeSelf);
    }
    public void loadMesh()
    {
        float startTime = Time.realtimeSinceStartup;
        GameObject MapGO = GameObject.Find("Map");
        AbstractMap Map = MapGO.GetComponent<AbstractMap>();
        string path = Application.streamingAssetsPath +"/Saves/"+ transform.GetComponentInChildren<TextMeshProUGUI>().text;
        string jsonContent = File.ReadAllText(path);
        MeshData rosMessege = JsonUtility.FromJson<MeshData>(jsonContent);
        int CloudsCount = rosMessege.Messages.Count;
        int NumberOfGO = (CloudsCount / MaxVerts) + 1;
        Debug.Log(CloudsCount);
        List<GameObject> MeshGOList = new List<GameObject>();
        GameObject load = new GameObject("Loader");
        load.transform.SetParent(GameObject.Find("LoadedObjects").transform);
        load.layer = 14;
        Vector3 position3d = Map.GeoToWorldPosition(new Mapbox.Utils.Vector2d(rosMessege.latitude, rosMessege.longitude));
        position3d.y = Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(position3d));
        load.transform.position = position3d;
        load.transform.eulerAngles = new Vector3(rosMessege.rotX, rosMessege.rotY, rosMessege.rotZ);
        // Generate GameObjects
        for (int i = 0; i < NumberOfGO; i++)
        {
            GameObject newMeshGameObject = new GameObject("MeshObject");
            newMeshGameObject.AddComponent<MeshFilter>();
            newMeshGameObject.AddComponent<MeshRenderer>();
            newMeshGameObject.AddComponent<MeshCollider>();
            
            // newMeshGameObject
            newMeshGameObject.transform.SetParent(load.transform);
            newMeshGameObject.transform.localPosition = new Vector3(0, 0, 0);
            newMeshGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            newMeshGameObject.layer = 14;
            newMeshGameObject.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
            MeshGOList.Add(newMeshGameObject);
        }

        List<MessegeInfo> Points = rosMessege.Messages;
        long NumberOfClouds = Points.Count;


        float inc = 0.3f;
        int tris = 0;
        int vert = 0;
        long arraySize;
        for (int l = 0; l < NumberOfGO; l++)
        {
            if (l + 1 == NumberOfGO)
            {
                arraySize = -(NumberOfGO - 1) * MaxVerts + CloudsCount;
            }
            else
            {
                arraySize = MaxVerts;
            }
            Vector3[] vertices = new Vector3[arraySize * 8];
            int[] triangles = new int[arraySize * 36];
            Color[] colors = new Color[arraySize * 8];
            for (int i = 0; i < arraySize; i++)
            {
                vertices[i * 8 + 0] = new Vector3(Points[i + MaxVerts * l].x - inc, Points[i + MaxVerts * l].z - inc, Points[i + MaxVerts * l].y + inc);
                vertices[i * 8 + 1] = new Vector3(Points[i + MaxVerts * l].x - inc, Points[i + MaxVerts * l].z - inc, Points[i + MaxVerts * l].y - inc);
                vertices[i * 8 + 2] = new Vector3(Points[i + MaxVerts * l].x - inc, Points[i + MaxVerts * l].z + inc, Points[i + MaxVerts * l].y - inc);
                vertices[i * 8 + 3] = new Vector3(Points[i + MaxVerts * l].x - inc, Points[i + MaxVerts * l].z + inc, Points[i + MaxVerts * l].y + inc);
                vertices[i * 8 + 4] = new Vector3(Points[i + MaxVerts * l].x + inc, Points[i + MaxVerts * l].z + inc, Points[i + MaxVerts * l].y + inc);
                vertices[i * 8 + 5] = new Vector3(Points[i + MaxVerts * l].x + inc, Points[i + MaxVerts * l].z + inc, Points[i + MaxVerts * l].y - inc);
                vertices[i * 8 + 6] = new Vector3(Points[i + MaxVerts * l].x + inc, Points[i + MaxVerts * l].z - inc, Points[i + MaxVerts * l].y - inc);
                vertices[i * 8 + 7] = new Vector3(Points[i + MaxVerts * l].x + inc, Points[i + MaxVerts * l].z - inc, Points[i + MaxVerts * l].y + inc);
                triangles[i * 36 + 0] = 0 + 8 * i;
                triangles[i * 36 + 1] = 2 + 8 * i;
                triangles[i * 36 + 2] = 1 + 8 * i;
                triangles[i * 36 + 3] = 0 + 8 * i;
                triangles[i * 36 + 4] = 3 + 8 * i;
                triangles[i * 36 + 5] = 2 + 8 * i;
                triangles[i * 36 + 6] = 2 + 8 * i;
                triangles[i * 36 + 7] = 3 + 8 * i;
                triangles[i * 36 + 8] = 4 + 8 * i;
                triangles[i * 36 + 9] = 2 + 8 * i;
                triangles[i * 36 + 10] = 4 + 8 * i;
                triangles[i * 36 + 11] = 5 + 8 * i;
                triangles[i * 36 + 12] = 1 + 8 * i;
                triangles[i * 36 + 13] = 2 + 8 * i;
                triangles[i * 36 + 14] = 5 + 8 * i;
                triangles[i * 36 + 15] = 1 + 8 * i;
                triangles[i * 36 + 16] = 5 + 8 * i;
                triangles[i * 36 + 17] = 6 + 8 * i;
                triangles[i * 36 + 18] = 0 + 8 * i;
                triangles[i * 36 + 19] = 7 + 8 * i;
                triangles[i * 36 + 20] = 4 + 8 * i;
                triangles[i * 36 + 21] = 0 + 8 * i;
                triangles[i * 36 + 22] = 4 + 8 * i;
                triangles[i * 36 + 23] = 3 + 8 * i;
                triangles[i * 36 + 24] = 5 + 8 * i;
                triangles[i * 36 + 25] = 4 + 8 * i;
                triangles[i * 36 + 26] = 7 + 8 * i;
                triangles[i * 36 + 27] = 5 + 8 * i;
                triangles[i * 36 + 28] = 7 + 8 * i;
                triangles[i * 36 + 29] = 6 + 8 * i;
                triangles[i * 36 + 30] = 0 + 8 * i;
                triangles[i * 36 + 31] = 6 + 8 * i;
                triangles[i * 36 + 32] = 7 + 8 * i;
                triangles[i * 36 + 33] = 0 + 8 * i;
                triangles[i * 36 + 34] = 1 + 8 * i;
                triangles[i * 36 + 35] = 6 + 8 * i;
                colors[8 * i + 0] = new Color32((byte)Points[i + MaxVerts * l].r, (byte)Points[i + MaxVerts * l].g, (byte)Points[i + MaxVerts * l].b, 255);
                colors[8 * i + 1] = new Color32((byte)Points[i + MaxVerts * l].r, (byte)Points[i + MaxVerts * l].g, (byte)Points[i + MaxVerts * l].b, 255);
                colors[8 * i + 2] = new Color32((byte)Points[i + MaxVerts * l].r, (byte)Points[i + MaxVerts * l].g, (byte)Points[i + MaxVerts * l].b, 255);
                colors[8 * i + 3] = new Color32((byte)Points[i + MaxVerts * l].r, (byte)Points[i + MaxVerts * l].g, (byte)Points[i + MaxVerts * l].b, 255);
                colors[8 * i + 4] = new Color32((byte)Points[i + MaxVerts * l].r, (byte)Points[i + MaxVerts * l].g, (byte)Points[i + MaxVerts * l].b, 255);
                colors[8 * i + 5] = new Color32((byte)Points[i + MaxVerts * l].r, (byte)Points[i + MaxVerts * l].g, (byte)Points[i + MaxVerts * l].b, 255);
                colors[8 * i + 6] = new Color32((byte)Points[i + MaxVerts * l].r, (byte)Points[i + MaxVerts * l].g, (byte)Points[i + MaxVerts * l].b, 255);
                colors[8 * i + 7] = new Color32((byte)Points[i + MaxVerts * l].r, (byte)Points[i + MaxVerts * l].g, (byte)Points[i + MaxVerts * l].b, 255);
            }
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.colors = colors;
            MeshGOList[l].GetComponent<MeshFilter>().mesh = mesh;
            MeshGOList[l].GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
    }

    public void ChangeTransparency()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform renderObject in child)
            {
                renderObject.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("Transparency", transparencySlider.value);
            }
        }
    }


    [System.Serializable]
    public class MessegeInfo
    {
        public float x, y, z;
        public int r, g, b;
        public MessegeInfo(float xx, float yy, float zz, int rr, int gg, int bb)
        {
            x = xx;
            y = yy;
            z = zz;
            r = rr;
            g = gg;
            b = bb;
        }
    }
    [System.Serializable]
    public class MeshData
    {
        public double latitude;
        public double longitude;
        public float rotX;
        public float rotY;
        public float rotZ;
        public List<MessegeInfo> Messages;
        public MeshData(List<MessegeInfo> list)
        {
            Messages = list;
        }
    }
}

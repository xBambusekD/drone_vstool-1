using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mapbox.Unity.Map;
using System;

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

        Vector3[][] vertices = new Vector3[NumberOfGO][];
        int[][] triangles = new int[NumberOfGO][];
        Color[][] colors = new Color[NumberOfGO][];

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

            float inc = 0.3f;
            int vinc = 0;
            int tris = 0;
            int vert = 0;
            int first = l * MaxVerts;
            int last = l * (int)MaxVerts + (int)arraySize;

            vertices[l] = new Vector3[arraySize * 8];
            triangles[l] = new int[36 * (last - first)];
            colors[l] = new Color[arraySize * 8];



            for (int k = first; k < last; k++)
            {
                for (int i = 0; i < (last - first) * 8; i++)
                {
                    if (i == 0 + vinc)
                    {
                        vertices[l][i] = new Vector3(Points[k].x - inc, Points[k].z - inc, Points[k].y + inc);
                    }
                    else if (i == 1 + vinc)
                    {
                        vertices[l][i] = new Vector3(Points[k].x - inc, Points[k].z - inc, Points[k].y - inc);
                    }
                    else if (i == 2 + vinc)
                    {
                        vertices[l][i] = new Vector3(Points[k].x - inc, Points[k].z + inc, Points[k].y - inc);
                    }
                    else if (i == 3 + vinc)
                    {
                        vertices[l][i] = new Vector3(Points[k].x - inc, Points[k].z + inc, Points[k].y + inc);
                    }
                    else if (i == 4 + vinc)
                    {
                        vertices[l][i] = new Vector3(Points[k].x + inc, Points[k].z + inc, Points[k].y + inc);
                    }
                    else if (i == 5 + vinc)
                    {
                        vertices[l][i] = new Vector3(Points[k].x + inc, Points[k].z + inc, Points[k].y - inc);
                    }
                    else if (i == 6 + vinc)
                    {
                        vertices[l][i] = new Vector3(Points[k].x + inc, Points[k].z - inc, Points[k].y - inc);
                    }
                    else if (i == 7 + vinc)
                    {
                        vertices[l][i] = new Vector3(Points[k].x + inc, Points[k].z - inc, Points[k].y + inc);
                    }
                }
                vinc = vinc + 8;
                for (int i = 0; i < 36; i++)
                {
                    if (i == 0)
                    {
                        triangles[l][i + tris] = 0 + vert;
                    }
                    else if (i == 1)
                    {
                        triangles[l][i + tris] = 2 + vert;
                    }
                    else if (i == 2)
                    {
                        triangles[l][i + tris] = 1 + vert;
                    }
                    else if (i == 3)
                    {
                        triangles[l][i + tris] = 0 + vert;
                    }
                    else if (i == 4)
                    {
                        triangles[l][i + tris] = 3 + vert;
                    }
                    else if (i == 5)
                    {
                        triangles[l][i + tris] = 2 + vert;
                    }
                    else if (i == 6)
                    {
                        triangles[l][i + tris] = 2 + vert;
                    }
                    else if (i == 7)
                    {
                        triangles[l][i + tris] = 3 + vert;
                    }
                    else if (i == 8)
                    {
                        triangles[l][i + tris] = 4 + vert;
                    }
                    else if (i == 9)
                    {
                        triangles[l][i + tris] = 2 + vert;
                    }
                    else if (i == 10)
                    {
                        triangles[l][i + tris] = 4 + vert;
                    }
                    else if (i == 11)
                    {
                        triangles[l][i + tris] = 5 + vert;
                    }
                    else if (i == 12)
                    {
                        triangles[l][i + tris] = 1 + vert;
                    }
                    else if (i == 13)
                    {
                        triangles[l][i + tris] = 2 + vert;
                    }
                    else if (i == 14)
                    {
                        triangles[l][i + tris] = 5 + vert;
                    }
                    else if (i == 15)
                    {
                        triangles[l][i + tris] = 1 + vert;
                    }
                    else if (i == 16)
                    {
                        triangles[l][i + tris] = 5 + vert;
                    }
                    else if (i == 17)
                    {
                        triangles[l][i + tris] = 6 + vert;
                    }
                    else if (i == 18)
                    {
                        triangles[l][i + tris] = 0 + vert;
                    }
                    else if (i == 19)
                    {
                        triangles[l][i + tris] = 7 + vert;
                    }
                    else if (i == 20)
                    {
                        triangles[l][i + tris] = 4 + vert;
                    }
                    else if (i == 21)
                    {
                        triangles[l][i + tris] = 0 + vert;
                    }
                    else if (i == 22)
                    {
                        triangles[l][i + tris] = 4 + vert;
                    }
                    else if (i == 23)
                    {
                        triangles[l][i + tris] = 3 + vert;
                    }
                    else if (i == 24)
                    {
                        triangles[l][i + tris] = 5 + vert;
                    }
                    else if (i == 25)
                    {
                        triangles[l][i + tris] = 4 + vert;
                    }
                    else if (i == 26)
                    {
                        triangles[l][i + tris] = 7 + vert;
                    }
                    else if (i == 27)
                    {
                        triangles[l][i + tris] = 5 + vert;
                    }
                    else if (i == 28)
                    {
                        triangles[l][i + tris] = 7 + vert;
                    }
                    else if (i == 29)
                    {
                        triangles[l][i + tris] = 6 + vert;
                    }
                    else if (i == 30)
                    {
                        triangles[l][i + tris] = 0 + vert;
                    }
                    else if (i == 31)
                    {
                        triangles[l][i + tris] = 6 + vert;
                    }
                    else if (i == 32)
                    {
                        triangles[l][i + tris] = 7 + vert;
                    }
                    else if (i == 33)
                    {
                        triangles[l][i + tris] = 0 + vert;
                    }
                    else if (i == 34)
                    {
                        triangles[l][i + tris] = 1 + vert;
                    }
                    else if (i == 35)
                    {
                        triangles[l][i + tris] = 6 + vert;
                    }
                }
                vert += 8;
                tris += 36;
                for (int i = 0; i < 8; i++)
                {
                    int ci = i + (8 * (k - first));
                    colors[l][ci] = new Color32((byte)Points[k].r, (byte)Points[k].g, (byte)Points[k].b, 255);
                }
            }
        }
        for (int i = 0; i < NumberOfGO; i++)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices[i];
            mesh.triangles = triangles[i];
            mesh.colors = colors[i];
            MeshGOList[i].GetComponent<MeshFilter>().mesh = mesh;
            MeshGOList[i].GetComponent<MeshCollider>().sharedMesh = mesh;
        }


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

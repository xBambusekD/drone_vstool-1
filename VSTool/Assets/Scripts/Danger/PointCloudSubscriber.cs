using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RosSharp.RosBridgeClient;
using Random = UnityEngine.Random;
using sensor_msgs = RosSharp.RosBridgeClient.Messages.Sensor;
using System.IO;
using Object = UnityEngine.Object;
using System.Linq;
using Unity.Jobs;
using Unity.Collections;
using Mapbox.Unity.Map;
using TMPro;

public class PointCloudSubscriber : MonoBehaviour
{
    private RosSocket rosSocket;
    string subscriptionId = "";
    string subscription2Id = "";
    public AbstractMap Map;

    public RgbPoint3[] Points;
    Vector3[] newverts;


    int ci;
    int MaxVerts = 7000;
    int objcount = 0;

    private MeshFilter meshFilter1;
    private Vector3[] vertices;
    private int[] triangles;

    private Color[] colors;
    private long ProcessedClouds = 0;

    private long LastNumberOfClouds = 0;

    public TMP_InputField saveName;

    List<GameObject> MeshGOList = new List<GameObject>();

    private long layer = 0;

    private long NumberToGenerate = 0;
    private bool createGO = false;
    private bool updateMesh = false;
    private Vector3[][] vertices2d;
    private int[][] triangles2d;
    private Color[][] colors2d;
    private RgbPoint3[] lastMessage;
    long NumberOfClouds;


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

    void Start()
    {
        SetupConnection();
    }

    public void SetupConnection()
    {
        string uri = PlayerPrefs.GetString("RosBridgeURL");
        rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));
        Subscribe("/zed/rtabmap/octomap_occupied_space");
    }

    void Update()
    {
        if (createGO)
        {
            createGO = false;
            for (int i = 0; i < NumberToGenerate; i++)
            {
                GameObject newMeshGameObject = new GameObject("MeshObject");
                newMeshGameObject.AddComponent<MeshFilter>();
                newMeshGameObject.AddComponent<MeshRenderer>();
                newMeshGameObject.AddComponent<MeshCollider>();
                // newMeshGameObject
                newMeshGameObject.layer = 14;
                newMeshGameObject.transform.SetParent(transform);
                newMeshGameObject.transform.localPosition = new Vector3(0, 0, 0);
                newMeshGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                newMeshGameObject.transform.gameObject.layer = 14;
                newMeshGameObject.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
                MeshGOList.Add(newMeshGameObject);
            }

            NumberToGenerate = 0;
        }

        if (updateMesh)
        {
            updateMesh = false;
            for (int i = 0; i < layer; i++)
            {
                Mesh mesh = new Mesh();
                mesh.vertices = vertices2d[i];
                mesh.triangles = triangles2d[i];
                mesh.colors = colors2d[i];
                MeshGOList[i].GetComponent<MeshFilter>().mesh = mesh;
                MeshGOList[i].GetComponent<MeshCollider>().sharedMesh = mesh;
            }

        }
    }

    public void Subscribe(string id)
    {
        subscriptionId = rosSocket.Subscribe<sensor_msgs.PointCloud2>(id, SubscriptionHandler);
    }


    public void saveMesh()
    {
        List<MessegeInfo> messageInfos = new List<MessegeInfo>();
        for (int i = 0; i < NumberOfClouds; i++)
        {
            messageInfos.Add(new MessegeInfo(lastMessage[i].x, lastMessage[i].y, lastMessage[i].z,
                lastMessage[i].rgb[0], lastMessage[i].rgb[1], lastMessage[i].rgb[2]));
        }

        MeshData message = new MeshData(messageInfos);
        Mapbox.Utils.Vector2d renderPosition = Map.WorldToGeoPosition(transform.position);
        message.latitude = renderPosition.x;
        message.longitude = renderPosition.y;
        Debug.Log(message.latitude);
        Debug.Log(message.longitude);
        message.rotX = transform.rotation.eulerAngles.x;
        message.rotY = transform.rotation.eulerAngles.y;
        message.rotZ = transform.rotation.eulerAngles.z;
        if (saveName.text.Length != 0)
        {
            Debug.Log(saveName.text);
            string path = Application.streamingAssetsPath + "/Saves/" + saveName.text + ".json";
            File.WriteAllText(path, JsonUtility.ToJson(message));
            Debug.Log("saved");
        }
        else
        {
            Debug.Log("Zadaj heslo");
        }


    }


    public void SubscriptionHandler(sensor_msgs.PointCloud2 message)
    {
        NumberOfClouds = message.data.Length / message.point_step; // Cut the pointcloud to points

        if (NumberOfClouds > layer * MaxVerts)
        {
            NumberToGenerate = (NumberOfClouds - layer * MaxVerts) / MaxVerts + 1;
            layer += NumberToGenerate;
            createGO = true;
        }

        RgbPoint3[] Points = new RgbPoint3[NumberOfClouds];
        byte[] byteSlice = new byte[message.point_step];

        for (long i = 0; i < NumberOfClouds; i++)
        {
            Array.Copy(message.data, i * message.point_step, byteSlice, 0, message.point_step);
            Points[i] = new RgbPoint3(byteSlice, message.fields);
        }

        lastMessage = Points;

        vertices2d = new Vector3[layer][];
        triangles2d = new int[layer][];
        colors2d = new Color[layer][];

        SetupMeshArrays(Points);
        updateMesh = true;
    }

    private void SetupMeshArrays(RgbPoint3[] Points)
    {
        long arraySize;
        for (int l = 0; l < layer; l++)
        {
            if (l + 1 == layer)
            {
                arraySize = -(layer - 1) * MaxVerts + NumberOfClouds;
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
            int last = l * (int) MaxVerts + (int) arraySize;

            vertices2d[l] = new Vector3[arraySize * 8];
            triangles2d[l] = new int[36 * (last - first)];
            colors2d[l] = new Color[arraySize * 8];



            for (int i = 0; i < arraySize; i++)
            {
                vertices2d[l][i * 8 + 0] = new Vector3(Points[i + MaxVerts * l].x - inc,
                    Points[i + MaxVerts * l].z - inc, Points[i + MaxVerts * l].y + inc);
                vertices2d[l][i * 8 + 1] = new Vector3(Points[i + MaxVerts * l].x - inc,
                    Points[i + MaxVerts * l].z - inc, Points[i + MaxVerts * l].y - inc);
                vertices2d[l][i * 8 + 2] = new Vector3(Points[i + MaxVerts * l].x - inc,
                    Points[i + MaxVerts * l].z + inc, Points[i + MaxVerts * l].y - inc);
                vertices2d[l][i * 8 + 3] = new Vector3(Points[i + MaxVerts * l].x - inc,
                    Points[i + MaxVerts * l].z + inc, Points[i + MaxVerts * l].y + inc);
                vertices2d[l][i * 8 + 4] = new Vector3(Points[i + MaxVerts * l].x + inc,
                    Points[i + MaxVerts * l].z + inc, Points[i + MaxVerts * l].y + inc);
                vertices2d[l][i * 8 + 5] = new Vector3(Points[i + MaxVerts * l].x + inc,
                    Points[i + MaxVerts * l].z + inc, Points[i + MaxVerts * l].y - inc);
                vertices2d[l][i * 8 + 6] = new Vector3(Points[i + MaxVerts * l].x + inc,
                    Points[i + MaxVerts * l].z - inc, Points[i + MaxVerts * l].y - inc);
                vertices2d[l][i * 8 + 7] = new Vector3(Points[i + MaxVerts * l].x + inc,
                    Points[i + MaxVerts * l].z - inc, Points[i + MaxVerts * l].y + inc);
                triangles2d[l][i * 36 + 0] = 0 + 8 * i;
                triangles2d[l][i * 36 + 1] = 2 + 8 * i;
                triangles2d[l][i * 36 + 2] = 1 + 8 * i;
                triangles2d[l][i * 36 + 3] = 0 + 8 * i;
                triangles2d[l][i * 36 + 4] = 3 + 8 * i;
                triangles2d[l][i * 36 + 5] = 2 + 8 * i;
                triangles2d[l][i * 36 + 6] = 2 + 8 * i;
                triangles2d[l][i * 36 + 7] = 3 + 8 * i;
                triangles2d[l][i * 36 + 8] = 4 + 8 * i;
                triangles2d[l][i * 36 + 9] = 2 + 8 * i;
                triangles2d[l][i * 36 + 10] = 4 + 8 * i;
                triangles2d[l][i * 36 + 11] = 5 + 8 * i;
                triangles2d[l][i * 36 + 12] = 1 + 8 * i;
                triangles2d[l][i * 36 + 13] = 2 + 8 * i;
                triangles2d[l][i * 36 + 14] = 5 + 8 * i;
                triangles2d[l][i * 36 + 15] = 1 + 8 * i;
                triangles2d[l][i * 36 + 16] = 5 + 8 * i;
                triangles2d[l][i * 36 + 17] = 6 + 8 * i;
                triangles2d[l][i * 36 + 18] = 0 + 8 * i;
                triangles2d[l][i * 36 + 19] = 7 + 8 * i;
                triangles2d[l][i * 36 + 20] = 4 + 8 * i;
                triangles2d[l][i * 36 + 21] = 0 + 8 * i;
                triangles2d[l][i * 36 + 22] = 4 + 8 * i;
                triangles2d[l][i * 36 + 23] = 3 + 8 * i;
                triangles2d[l][i * 36 + 24] = 5 + 8 * i;
                triangles2d[l][i * 36 + 25] = 4 + 8 * i;
                triangles2d[l][i * 36 + 26] = 7 + 8 * i;
                triangles2d[l][i * 36 + 27] = 5 + 8 * i;
                triangles2d[l][i * 36 + 28] = 7 + 8 * i;
                triangles2d[l][i * 36 + 29] = 6 + 8 * i;
                triangles2d[l][i * 36 + 30] = 0 + 8 * i;
                triangles2d[l][i * 36 + 31] = 6 + 8 * i;
                triangles2d[l][i * 36 + 32] = 7 + 8 * i;
                triangles2d[l][i * 36 + 33] = 0 + 8 * i;
                triangles2d[l][i * 36 + 34] = 1 + 8 * i;
                triangles2d[l][i * 36 + 35] = 6 + 8 * i;
                colors2d[l][8 * i + 0] = new Color32((byte) Points[i + MaxVerts * l].rgb[0],
                    (byte) Points[i + MaxVerts * l].rgb[1], (byte) Points[i + MaxVerts * l].rgb[2], 255);
                colors2d[l][8 * i + 1] = new Color32((byte) Points[i + MaxVerts * l].rgb[0],
                    (byte) Points[i + MaxVerts * l].rgb[1], (byte) Points[i + MaxVerts * l].rgb[2], 255);
                colors2d[l][8 * i + 2] = new Color32((byte) Points[i + MaxVerts * l].rgb[0],
                    (byte) Points[i + MaxVerts * l].rgb[1], (byte) Points[i + MaxVerts * l].rgb[2], 255);
                colors2d[l][8 * i + 3] = new Color32((byte) Points[i + MaxVerts * l].rgb[0],
                    (byte) Points[i + MaxVerts * l].rgb[1], (byte) Points[i + MaxVerts * l].rgb[2], 255);
                colors2d[l][8 * i + 4] = new Color32((byte) Points[i + MaxVerts * l].rgb[0],
                    (byte) Points[i + MaxVerts * l].rgb[1], (byte) Points[i + MaxVerts * l].rgb[2], 255);
                colors2d[l][8 * i + 5] = new Color32((byte) Points[i + MaxVerts * l].rgb[0],
                    (byte) Points[i + MaxVerts * l].rgb[1], (byte) Points[i + MaxVerts * l].rgb[2], 255);
                colors2d[l][8 * i + 6] = new Color32((byte) Points[i + MaxVerts * l].rgb[0],
                    (byte) Points[i + MaxVerts * l].rgb[1], (byte) Points[i + MaxVerts * l].rgb[2], 255);
                colors2d[l][8 * i + 7] = new Color32((byte) Points[i + MaxVerts * l].rgb[0],
                    (byte) Points[i + MaxVerts * l].rgb[1], (byte) Points[i + MaxVerts * l].rgb[2], 255);

            }
        }
    }
}

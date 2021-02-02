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

public class PointCloudSubscriber : MonoBehaviour
{
	private RosSocket rosSocket;
	string subscriptionId = "";
	string subscription2Id = "";


	public RgbPoint3[] Points;
	Vector3[] newverts;


	int ci;
	int MaxVerts = 3000;
	int objcount = 0;

	private MeshFilter meshFilter1;
	private Vector3[] vertices;
	private int[] triangles;

	private Color[] colors;
	private long ProcessedClouds = 0;

	private long LastNumberOfClouds = 0;

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
	public class MessegeInfo{
		public float x,y,z;
		public int r,g,b;
		public MessegeInfo(float xx, float yy, float zz,int rr, int gg, int bb){
			x = xx;
			y = yy;
			z = zz;
			r = rr;
			g = gg;
			b = bb;
		}
	}
	[System.Serializable]
	public class MeshData{
		public List<MessegeInfo> Messages;	
		public MeshData(List<MessegeInfo> list){
			Messages = list;
		}
	}

    void Start()
	{
		SetupConnection();
	}

    public void SetupConnection(){
        string uri = PlayerPrefs.GetString("RosBridgeURL");
        rosSocket = new RosSocket(new  RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));
		Subscribe("/zed/rtabmap/octomap_occupied_space");
    }
	void Update(){
		if(createGO){
			createGO = false;
			for(int i = 0; i < NumberToGenerate;i++){
				GameObject newMeshGameObject = new GameObject("MeshObject");
				newMeshGameObject.AddComponent<MeshFilter>();
				newMeshGameObject.AddComponent<MeshRenderer>();
				newMeshGameObject.AddComponent<MeshCollider>();
				// newMeshGameObject
				newMeshGameObject.transform.SetParent(transform);
				newMeshGameObject.transform.localPosition =new Vector3(0,0,0);	
				newMeshGameObject.transform.localEulerAngles = new Vector3(0,0,0);
                newMeshGameObject.transform.gameObject.layer = 11;
				newMeshGameObject.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
				MeshGOList.Add(newMeshGameObject);		
			}
			NumberToGenerate = 0;
		}

		if(updateMesh){
			updateMesh = false;
			for(int i = 0; i < layer; i++){
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
		subscriptionId  = rosSocket.Subscribe<sensor_msgs.PointCloud2>(id, SubscriptionHandler);
	}


	public void saveMesh(){
		List<MessegeInfo> messageInfos = new List<MessegeInfo>();
		for(int i = 0; i < NumberOfClouds; i++){
			messageInfos.Add(new MessegeInfo(lastMessage[i].x,lastMessage[i].y,lastMessage[i].z,lastMessage[i].rgb[0],lastMessage[i].rgb[1],lastMessage[i].rgb[2]));
		}
		MeshData message = new MeshData(messageInfos);
		string path = Application.dataPath + "/save.json";
		if(!File.Exists(path)){
			File.WriteAllText(path,JsonUtility.ToJson(message));
		}
	}

	// TODO
	public void loadMesh(){
		string path = Application.dataPath + "/save.json";
		string jsonContent =File.ReadAllText(path);
		MeshData rosMessege = JsonUtility.FromJson<MeshData>(jsonContent);
        int CloudsCount = rosMessege.Messages.Count();
        int NumberOfGO = (CloudsCount / MaxVerts) +1;
        Debug.Log(CloudsCount);
        List<GameObject> MeshGOList = new List<GameObject>();

        // Generate GameObjects
        for (int i = 0; i < NumberOfGO; i++)
        {
            GameObject newMeshGameObject = new GameObject("MeshObject");
            newMeshGameObject.AddComponent<MeshFilter>();
            newMeshGameObject.AddComponent<MeshRenderer>();
            newMeshGameObject.AddComponent<MeshCollider>();
            // newMeshGameObject
            newMeshGameObject.transform.SetParent(transform);
            newMeshGameObject.transform.localPosition = new Vector3(0, 0, 0);
            newMeshGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            newMeshGameObject.transform.gameObject.layer = 11;
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
                    ci = i + (8 * (k - first));
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
	public void SubscriptionHandler(sensor_msgs.PointCloud2 message)
    {
        NumberOfClouds = message.data.Length / message.point_step;  // Cut the pointcloud to points

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
            int last = l * (int)MaxVerts + (int)arraySize;

            vertices2d[l] = new Vector3[arraySize * 8];
            triangles2d[l] = new int[36 * (last - first)];
            colors2d[l] = new Color[arraySize * 8];



            for (int k = first; k < last; k++)
            {
                for (int i = 0; i < (last - first) * 8; i++)
                {
                    if (i == 0 + vinc)
                    {
                        vertices2d[l][i] = new Vector3(Points[k].x - inc, Points[k].z - inc, Points[k].y + inc);
                    }
                    else if (i == 1 + vinc)
                    {
                        vertices2d[l][i] = new Vector3(Points[k].x - inc, Points[k].z - inc, Points[k].y - inc);
                    }
                    else if (i == 2 + vinc)
                    {
                        vertices2d[l][i] = new Vector3(Points[k].x - inc, Points[k].z + inc, Points[k].y - inc);
                    }
                    else if (i == 3 + vinc)
                    {
                        vertices2d[l][i] = new Vector3(Points[k].x - inc, Points[k].z + inc, Points[k].y + inc);
                    }
                    else if (i == 4 + vinc)
                    {
                        vertices2d[l][i] = new Vector3(Points[k].x + inc, Points[k].z + inc, Points[k].y + inc);
                    }
                    else if (i == 5 + vinc)
                    {
                        vertices2d[l][i] = new Vector3(Points[k].x + inc, Points[k].z + inc, Points[k].y - inc);
                    }
                    else if (i == 6 + vinc)
                    {
                        vertices2d[l][i] = new Vector3(Points[k].x + inc, Points[k].z - inc, Points[k].y - inc);
                    }
                    else if (i == 7 + vinc)
                    {
                        vertices2d[l][i] = new Vector3(Points[k].x + inc, Points[k].z - inc, Points[k].y + inc);
                    }
                }
                vinc = vinc + 8;
                for (int i = 0; i < 36; i++)
                {
                    if (i == 0)
                    {
                        triangles2d[l][i + tris] = 0 + vert;
                    }
                    else if (i == 1)
                    {
                        triangles2d[l][i + tris] = 2 + vert;
                    }
                    else if (i == 2)
                    {
                        triangles2d[l][i + tris] = 1 + vert;
                    }
                    else if (i == 3)
                    {
                        triangles2d[l][i + tris] = 0 + vert;
                    }
                    else if (i == 4)
                    {
                        triangles2d[l][i + tris] = 3 + vert;
                    }
                    else if (i == 5)
                    {
                        triangles2d[l][i + tris] = 2 + vert;
                    }
                    else if (i == 6)
                    {
                        triangles2d[l][i + tris] = 2 + vert;
                    }
                    else if (i == 7)
                    {
                        triangles2d[l][i + tris] = 3 + vert;
                    }
                    else if (i == 8)
                    {
                        triangles2d[l][i + tris] = 4 + vert;
                    }
                    else if (i == 9)
                    {
                        triangles2d[l][i + tris] = 2 + vert;
                    }
                    else if (i == 10)
                    {
                        triangles2d[l][i + tris] = 4 + vert;
                    }
                    else if (i == 11)
                    {
                        triangles2d[l][i + tris] = 5 + vert;
                    }
                    else if (i == 12)
                    {
                        triangles2d[l][i + tris] = 1 + vert;
                    }
                    else if (i == 13)
                    {
                        triangles2d[l][i + tris] = 2 + vert;
                    }
                    else if (i == 14)
                    {
                        triangles2d[l][i + tris] = 5 + vert;
                    }
                    else if (i == 15)
                    {
                        triangles2d[l][i + tris] = 1 + vert;
                    }
                    else if (i == 16)
                    {
                        triangles2d[l][i + tris] = 5 + vert;
                    }
                    else if (i == 17)
                    {
                        triangles2d[l][i + tris] = 6 + vert;
                    }
                    else if (i == 18)
                    {
                        triangles2d[l][i + tris] = 0 + vert;
                    }
                    else if (i == 19)
                    {
                        triangles2d[l][i + tris] = 7 + vert;
                    }
                    else if (i == 20)
                    {
                        triangles2d[l][i + tris] = 4 + vert;
                    }
                    else if (i == 21)
                    {
                        triangles2d[l][i + tris] = 0 + vert;
                    }
                    else if (i == 22)
                    {
                        triangles2d[l][i + tris] = 4 + vert;
                    }
                    else if (i == 23)
                    {
                        triangles2d[l][i + tris] = 3 + vert;
                    }
                    else if (i == 24)
                    {
                        triangles2d[l][i + tris] = 5 + vert;
                    }
                    else if (i == 25)
                    {
                        triangles2d[l][i + tris] = 4 + vert;
                    }
                    else if (i == 26)
                    {
                        triangles2d[l][i + tris] = 7 + vert;
                    }
                    else if (i == 27)
                    {
                        triangles2d[l][i + tris] = 5 + vert;
                    }
                    else if (i == 28)
                    {
                        triangles2d[l][i + tris] = 7 + vert;
                    }
                    else if (i == 29)
                    {
                        triangles2d[l][i + tris] = 6 + vert;
                    }
                    else if (i == 30)
                    {
                        triangles2d[l][i + tris] = 0 + vert;
                    }
                    else if (i == 31)
                    {
                        triangles2d[l][i + tris] = 6 + vert;
                    }
                    else if (i == 32)
                    {
                        triangles2d[l][i + tris] = 7 + vert;
                    }
                    else if (i == 33)
                    {
                        triangles2d[l][i + tris] = 0 + vert;
                    }
                    else if (i == 34)
                    {
                        triangles2d[l][i + tris] = 1 + vert;
                    }
                    else if (i == 35)
                    {
                        triangles2d[l][i + tris] = 6 + vert;
                    }
                }
                vert += 8;
                tris += 36;
                for (int i = 0; i < 8; i++)
                {
                    ci = i + (8 * (k - first));
                    colors2d[l][ci] = new Color32((byte)Points[k].rgb[0], (byte)Points[k].rgb[1], (byte)Points[k].rgb[2], 255);
                }
            }
        }
    }
}

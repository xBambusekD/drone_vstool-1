using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RosSharp.RosBridgeClient;
using Random=UnityEngine.Random;
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

    private bool createGO = false;
    private bool updateMesh = false;

    private Vector3[][] vertices2d;
    private int[][] triangles2d;
    private Color[][] colors2d;

    public class RgbPoint3
    {
        public float x;
        public float y;
        public float z;
        public int[] rgb;

        public RgbPoint3(byte[] bytes, RosSharp.RosBridgeClient.Messages.Sensor.PointField[] fields)
        {
            foreach (var field in fields)
            {
                byte[] slice = new byte[field.count * 4];
                Array.Copy(bytes, field.offset, slice, 0, field.count * 4);
                switch (field.name)
                {
                    case "x":
                        x = GetValue(slice);
                        break;
                    case "y":
                        y = GetValue(slice);
                        break;
                    case "z":
                        z = GetValue(slice);
                        break;
                    case "rgb":
                        rgb = GetRGB(slice);
                        break;
                }
            }
        }

        public override string ToString()
        {
            return "xyz=(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")"
                + "  rgb=(" + rgb[0].ToString() + ", " + rgb[1].ToString() + ", " + rgb[2].ToString() + ")";
        }
        private static float GetValue(byte[] bytes)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            float result = BitConverter.ToSingle(bytes, 0);
            return result;
        }
        private static int[] GetRGB(byte[] bytes)
        {
            int[] rgb = new int[3];
            rgb[0] = Convert.ToInt16(bytes[0]);
            rgb[1] = Convert.ToInt16(bytes[1]);
            rgb[2] = Convert.ToInt16(bytes[2]);

			return rgb;
        }
    }
	

	void Update(){
		if(createGO){
			createGO = false;
			GameObject newMeshGameObject = new GameObject("MeshObject");
			newMeshGameObject.AddComponent<MeshFilter>();
			newMeshGameObject.AddComponent<MeshRenderer>();
			newMeshGameObject.transform.SetParent(transform);
			newMeshGameObject.transform.localPosition =new Vector3(0,0,0);	
			newMeshGameObject.transform.localEulerAngles = new Vector3(0,0,0);
			newMeshGameObject.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
			MeshGOList.Add(newMeshGameObject);		
		}

		if(updateMesh){
			updateMesh = false;
			for(int i = 0; i < layer; i++){
				Mesh mesh = new Mesh();
				mesh.vertices = vertices2d[i];
				mesh.triangles = triangles2d[i];
				mesh.colors = colors2d[i];
				MeshGOList[i].GetComponent<MeshFilter>().mesh = mesh;
			}
		}
	}

    public void StartOctomapSubscribe(string uri, string topicName) {
        rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri)); // 10.189.42.225:9090
        Subscribe(topicName);
    }

	public void Subscribe(string id)
	{
		subscriptionId  = rosSocket.Subscribe<sensor_msgs.PointCloud2>(id, SubscriptionHandler);
	}

	public void SubscriptionHandler(sensor_msgs.PointCloud2 message)
	{		

		long NumberOfClouds = message.data.Length / message.point_step;  // Cut the pointcloud to points
		Debug.Log(NumberOfClouds);
		if(NumberOfClouds > layer*MaxVerts){
			layer ++;
			createGO = true;	
		}

		RgbPoint3[] Points = new RgbPoint3[NumberOfClouds];
		byte[] byteSlice = new byte[message.point_step];

		for (long i = 0; i < NumberOfClouds; i++)
		{
			Array.Copy(message.data, i * message.point_step, byteSlice, 0, message.point_step);
			Points[i] = new RgbPoint3(byteSlice, message.fields);
		}

		newverts = new Vector3[NumberOfClouds];
		
		//Assign all PointCloud points to the Vector3[]
		for (var i = 0; i < NumberOfClouds; i++)
		{
			newverts[i] = new Vector3(Points[i].x, Points[i].z ,Points[i].y);
		}

		vertices2d = new Vector3[layer][];
		triangles2d = new int[layer][];
		colors2d = new Color[layer][];

		long arraySize; 
		for(int l = 0; l < layer; l++){
			if(l+1 == layer){
				arraySize = -(layer-1)*MaxVerts + NumberOfClouds;
			} else {
				arraySize = MaxVerts;
			}
			
			float inc = 0.3f;
			int vinc = 0;
			int tris = 0;
			int vert = 0;
			int first = l*MaxVerts;
			int last = l*(int)MaxVerts + (int)arraySize;
			
			vertices2d[l] = new Vector3[arraySize*8]; 
			triangles2d[l]  = new int[36*(last - first)];
			colors2d[l] = new Color[arraySize*8];
			


			for(int k = first; k < last; k++){
				for(int i = 0; i < (last-first)*8; i++){
					if(i==0+vinc){
						vertices2d[l][i] = new Vector3(newverts[k].x - inc, newverts[k].y - inc, newverts[k].z + inc);
					}
					else if(i==1+vinc){
						vertices2d[l][i] = new Vector3(newverts[k].x - inc, newverts[k].y - inc, newverts[k].z - inc);
					}
					else if(i==2+vinc){
						vertices2d[l][i] = new Vector3(newverts[k].x - inc, newverts[k].y + inc, newverts[k].z - inc);
					}
					else if(i==3+vinc){
						vertices2d[l][i] = new Vector3(newverts[k].x - inc, newverts[k].y + inc, newverts[k].z + inc);
					}
					else if(i==4+vinc){
						vertices2d[l][i] = new Vector3(newverts[k].x + inc, newverts[k].y + inc, newverts[k].z + inc);
					}
					else if(i==5+vinc){
						vertices2d[l][i] = new Vector3(newverts[k].x + inc, newverts[k].y + inc, newverts[k].z - inc);
					}
					else if(i==6+vinc){
						vertices2d[l][i] = new Vector3(newverts[k].x + inc, newverts[k].y - inc, newverts[k].z - inc);
					}
					else if(i==7+vinc){
						vertices2d[l][i] = new Vector3(newverts[k].x + inc, newverts[k].y - inc, newverts[k].z + inc);
					}
				}
				vinc = vinc + 8;
				for(int i = 0; i < 36; i++){
					if(i==0){
						triangles2d[l][i + tris] = 0 + vert;
					}
					else if(i==1){
						triangles2d[l][i + tris] = 2 + vert;
					}
					else if(i==2){
						triangles2d[l][i + tris] = 1 + vert;
					}
					else if(i==3){
						triangles2d[l][i + tris] = 0 + vert;
					}
					else if(i==4){
						triangles2d[l][i + tris] = 3 + vert;
					}
					else if(i==5){
						triangles2d[l][i + tris] = 2 + vert;
					}
					else if(i==6){
						triangles2d[l][i + tris] = 2 + vert;
					}
					else if(i==7){
						triangles2d[l][i + tris] = 3 + vert;
					}
					else if(i==8){
						triangles2d[l][i + tris] = 4 + vert;
					}
					else if(i==9){
						triangles2d[l][i + tris] = 2 + vert;
					}
					else if(i==10){
						triangles2d[l][i + tris] = 4 + vert;
					}
					else if(i==11){
						triangles2d[l][i + tris] = 5 + vert;
					}
					else if(i==12){
						triangles2d[l][i + tris] = 1 + vert;
					}
					else if(i==13){
						triangles2d[l][i + tris] = 2 + vert;
					}
					else if(i==14){
						triangles2d[l][i + tris] = 5 + vert;
					}
					else if(i==15){
						triangles2d[l][i + tris] = 1 + vert;
					}
					else if(i==16){
						triangles2d[l][i + tris] = 5 + vert;
					}
					else if(i==17){
						triangles2d[l][i + tris] = 6 + vert;
					}
					else if(i==18){
						triangles2d[l][i + tris] = 0 + vert;
					}
					else if(i==19){
						triangles2d[l][i + tris] = 7 + vert;
					}
					else if(i==20){
						triangles2d[l][i + tris] = 4 + vert;
					}
					else if(i==21){
						triangles2d[l][i + tris] = 0 + vert;
					}
					else if(i==22){
						triangles2d[l][i + tris] = 4 + vert;
					}
					else if(i==23){
						triangles2d[l][i + tris] = 3 + vert;
					}
					else if(i==24){
						triangles2d[l][i + tris] = 5 + vert;
					}
					else if(i==25){
						triangles2d[l][i + tris] = 4 + vert;
					}
					else if(i==26){
						triangles2d[l][i + tris] = 7 + vert;
					}
					else if(i==27){
						triangles2d[l][i + tris] = 5 + vert;
					}
					else if(i==28){
						triangles2d[l][i + tris] = 7 + vert;
					}
					else if(i==29){
						triangles2d[l][i + tris] = 6 + vert;
					}
					else if(i==30){
						triangles2d[l][i + tris] = 0 + vert;
					}
					else if(i==31){
						triangles2d[l][i + tris] = 6 + vert;
					}
					else if(i==32){
						triangles2d[l][i + tris] = 7 + vert;
					}
					else if(i==33){
						triangles2d[l][i + tris] = 0 + vert;
					}
					else if(i==34){
						triangles2d[l][i + tris] = 1 + vert;
					}
					else if(i==35){
						triangles2d[l][i + tris] = 6 + vert;
					}
				}
				vert+=8;
				tris+=36;
				for(int i = 0;  i < 8; i++){
					ci = i + (8*(k-first));
					colors2d[l][ci] = new Color32((byte)Points[k].rgb[0],(byte)Points[k].rgb[1],(byte)Points[k].rgb[2],255);
				}
			}
		}
		updateMesh = true;
	}
}

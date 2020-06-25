// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using RosSharp.RosBridgeClient;
// using std_msgs = RosSharp.RosBridgeClient.Messages.Sensor;
// using System.IO;
// using Object = UnityEngine.Object;
// using System;
// using Random=UnityEngine.Random;
// public class BasicROS : MonoBehaviour
// {

//     public string uri = "ws://192.168.56.101:9090";
//     private RosSocket rosSocket;
//     string subscriptionId = "";

//     public RgbPoint3[] Points;

// 	//Mesh components
// 	Mesh mesh;
// 	Vector3[] vertices;
// 	int[] triangles;
// 	int[] newtrigs;
// 	Vector3[] newverts;
// 	Vector3[] tempverts;
// 	Color[] colors; 
//     int[] choise;
// 	int index; 
// 	int ci;

// 	private bool update_mesh = false;
// 	PointOctree<GameObject> pointTree;
// 	BoundsOctree<GameObject> boundsTree;

//     void Start()
//     {
//         mesh = new Mesh(); 
// 		GetComponent<MeshFilter>().mesh = mesh;
//         rosSocket = new RosSocket(new
//         RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri)); // 10.189.42.225:9090
//         Subscribe("/octomap_point_cloud_centers");
//     }
//     void Update()
// 	{

// 		if(update_mesh) {
// 			update_mesh = false;

// 			//CreateShape();
// 			UpdateMesh();
// 		}

// 	}

//     void UpdateMesh()
// 	{
// 		//Debug.Log("received three");

// 		try {
// 			mesh.Clear();
// 		} catch (Exception e) {
// 			Debug.Log(e);
// 		}
// 		//Debug.Log("received four");

// 		mesh.vertices = vertices;
// 		//mesh.newverts = newverts;
// 		mesh.triangles = triangles;
// 		mesh.colors = colors;

// 	}
//     public void Subscribe(string id)
//     {
//         subscriptionId = rosSocket.Subscribe<std_msgs.PointCloud2>(id, SubscriptionHandler);
//     }

//     public void OnDrawGizmos()
// 	{
// 		Gizmos.color = new Color(1, 0.2F, 0, 0.5F);
// 		if(vertices == null)
// 			return;
// 		for(int i =0; i < vertices.Length; i++)
// 	 {
// 			Gizmos.DrawSphere(vertices[i], 0.03f);
// 			//Gizmos.DrawCube(vertices[i], new Vector3(0.1f, 0.1f, 0.1f));
// 			}
// 	}

//     public int  rndnumber(){
// 	//try {
// 		int number = Random.Range(0,10);
// 		Debug.Log("MMM   " + number);
// 			return number;
// 	//} catch (Exception e) {
// 	//	Debug.Log(e);
// 	//}
// 	}

//     public void ColorLayer(int k, int index){
// 		for(int i = 0;  i < 8; i++){
// 			ci = i + (8*k);
// 			if(index==0)
// 				colors[ci] = Color.Lerp(Color.green, Color.red, vertices[ci].y);
// 			else if(index==1)
// 				colors[ci] = Color.Lerp(Color.blue, Color.red, vertices[ci].y);
// 			//else if(index==2)
// 			//	colors[ci] = Color.Lerp(Color.black, Color.black, vertices[ci].y);
// 			else if(index==2)
// 				colors[ci] = Color.Lerp(Color.cyan, Color.cyan, vertices[ci].y);
// 			//else if(index==4)
// 			//	colors[ci] = Color.Lerp(Color.gray, Color.gray, vertices[ci].y);
// 			else if(index==3)
// 				colors[ci] = Color.Lerp(Color.magenta, Color.magenta, vertices[ci].y);
// 			else if(index==4)
// 				colors[ci] = Color.Lerp(Color.red, Color.red, vertices[ci].y);
// 			else if(index==5)
// 				colors[ci] = Color.Lerp(Color.yellow, Color.yellow, vertices[ci].y);
// 		}
// 	}

//     private IEnumerator WaitForKey()
//     {
//         Debug.Log("Press any key to close...");

//         while (!Input.anyKeyDown)
//         {
//             yield return null;
//         }

//         Debug.Log("Closed");
//         // rosSocket.Close();
//     }

//     private void SubscriptionHandler(std_msgs.PointCloud2 message)
//     {
//         long I = message.data.Length / message.point_step;
// 		Debug.Log("Long I   " + I);
// 		RgbPoint3[] Points = new RgbPoint3[I];
// 		byte[] byteSlice = new byte[message.point_step];

// 		for (long i = 0; i < I; i++)
// 		{
// 			Array.Copy(message.data, i * message.point_step, byteSlice, 0, message.point_step);
// 			Points[i] = new RgbPoint3(byteSlice, message.fields);
// 		}


// 		newverts = new Vector3[I];
// 		double[] y_array = new double[I];

// 		//Assign all PointCloud points to the Vecto3[]
// 		for (var i = 0; i < I; i++)
// 		{
// 			newverts[i] = new Vector3(Points[i].x, Points[i].z ,Points[i].y);
// 			y_array[i] = newverts[i].y;
// 		}

// 		float inc = 0.15f;

// 		/*int k =100;
// 		vertices = new Vector3[]{
			
// 			new Vector3 (newverts[k].x - inc, newverts[k].y - inc, newverts[k].z + inc),

// 			new Vector3 (newverts[k].x - inc, newverts[k].y - inc, newverts[k].z - inc),

// 			new Vector3 (newverts[k].x - inc, newverts[k].y + inc, newverts[k].z - inc),

// 			new Vector3 (newverts[k].x - inc, newverts[k].y + inc, newverts[k].z + inc),

// 			new Vector3 (newverts[k].x + inc, newverts[k].y + inc, newverts[k].z + inc),

// 			new Vector3 (newverts[k].x + inc, newverts[k].y + inc, newverts[k].z - inc),

// 			new Vector3 (newverts[k].x + inc, newverts[k].y - inc, newverts[k].z - inc),

// 			new Vector3 (newverts[k].x + inc, newverts[k].y - inc, newverts[k].z + inc),

// 		};*/


// 		//Assign all the vertices
// 		vertices = new Vector3[I*8];
// 		int vinc=0;
// 		for(int k=0; k < I; k++){
// 			for(int i = 0;  i < I*8; i++){
// 				if(i==0+vinc){
// 				vertices[i] = new Vector3(newverts[k].x - inc, newverts[k].y - inc, newverts[k].z + inc);
// 			}
// 				else if(i==1+vinc){
// 				vertices[i] = new Vector3(newverts[k].x - inc, newverts[k].y - inc, newverts[k].z - inc);
// 			}
// 				else if(i==2+vinc){
// 				vertices[i] = new Vector3(newverts[k].x - inc, newverts[k].y + inc, newverts[k].z - inc);
// 			}
// 				else if(i==3+vinc){
// 				vertices[i] = new Vector3(newverts[k].x - inc, newverts[k].y + inc, newverts[k].z + inc);
// 			}
// 				else if(i==4+vinc){
// 				vertices[i] = new Vector3(newverts[k].x + inc, newverts[k].y + inc, newverts[k].z + inc);
// 			}
// 				else if(i==5+vinc){
// 				vertices[i] = new Vector3(newverts[k].x + inc, newverts[k].y + inc, newverts[k].z - inc);
// 			}
// 				else if(i==6+vinc){
// 				vertices[i] = new Vector3(newverts[k].x + inc, newverts[k].y - inc, newverts[k].z - inc);
// 			}
// 				else if(i==7+vinc){
// 				vertices[i] = new Vector3(newverts[k].x + inc, newverts[k].y - inc, newverts[k].z + inc);
// 			}
					
// 			}
// 			vinc = vinc + 8;
// 		}





// 		int tris = 0;//36;
// 		int vert = 0;//8;

// 		triangles  = new int[36*I];


// 		//Assign triangules of all the vertices
// 		for(int k=0;k<I;k++){
// 			for(int i = 0; i < 36; i++){
// 				if(i==0){
// 					triangles[i + tris] = 0 + vert;
// 				}
// 				else if(i==1){
// 					triangles[i + tris] = 2 + vert;
// 				}
// 				else if(i==2){
// 					triangles[i + tris] = 1 + vert;
// 				}
// 				else if(i==3){
// 					triangles[i + tris] = 0 + vert;
// 				}
// 				else if(i==4){
// 					triangles[i + tris] = 3 + vert;
// 				}
// 				else if(i==5){
// 					triangles[i + tris] = 2 + vert;
// 				}
// 				 else if(i==6){
// 					triangles[i + tris] = 2 + vert;
// 				}
// 				else if(i==7){
// 					triangles[i + tris] = 3 + vert;
// 				}
// 				else if(i==8){
// 					triangles[i + tris] = 4 + vert;
// 				}
// 				else if(i==9){
// 					triangles[i + tris] = 2 + vert;
// 				}
// 				else if(i==10){
// 					triangles[i + tris] = 4 + vert;
// 				}
// 				else if(i==11){
// 					triangles[i + tris] = 5 + vert;
// 				}
// 				else if(i==12){
// 					triangles[i + tris] = 1 + vert;
// 				}
// 				else if(i==13){
// 					triangles[i + tris] = 2 + vert;
// 				}
// 				else if(i==14){
// 					triangles[i + tris] = 5 + vert;
// 				}
// 				else if(i==15){
// 					triangles[i + tris] = 1 + vert;
// 				}
// 				else if(i==16){
// 					triangles[i + tris] = 5 + vert;
// 				}
// 				else if(i==17){
// 					triangles[i + tris] = 6 + vert;
// 				}

// 				else if(i==18){
// 					triangles[i + tris] = 0 + vert;
// 				}
// 				else if(i==19){
// 					triangles[i + tris] = 7 + vert;
// 				}
// 				else if(i==20){
// 					triangles[i + tris] = 4 + vert;
// 				}
// 				else if(i==21){
// 					triangles[i + tris] = 0 + vert;
// 				}
// 				else if(i==22){
// 					triangles[i + tris] = 4 + vert;
// 				}
// 				else if(i==23){
// 					triangles[i + tris] = 3 + vert;
// 				}
// 				else if(i==24){
// 					triangles[i + tris] = 5 + vert;
// 				}
// 				else if(i==25){
// 					triangles[i + tris] = 4 + vert;
// 				}
// 				else if(i==26){
// 					triangles[i + tris] = 7 + vert;
// 				}
// 				else if(i==27){
// 					triangles[i + tris] = 5 + vert;
// 				}
// 				else if(i==28){
// 					triangles[i + tris] = 7 + vert;
// 				}
// 				else if(i==29){
// 					triangles[i + tris] = 6 + vert;
// 				}
// 				 else if(i==30){
// 					triangles[i + tris] = 0 + vert;
// 				}
// 				else if(i==31){
// 					triangles[i + tris] = 6 + vert;
// 				}
// 				else if(i==32){
// 					triangles[i + tris] = 7 + vert;
// 				}
// 				else if(i==33){
// 					triangles[i + tris] = 0 + vert;
// 				}
// 				else if(i==34){
// 					triangles[i + tris] = 1 + vert;
// 				}
// 				else if(i==35){
// 					triangles[i + tris] = 6 + vert;
// 				}
// 			}
// 			vert+=8;
// 			tris+=36;
// 		}

// 		colors = new Color[vertices.Length];

// 		index = 0;
// 		ci    = 0;
// 		//index=Random.Range(0,choise.Length);
// 		//Debug.Log("_X   " + index);
// 		//int numberindex = rndnumber();
// 		//Debug.Log("QQQQ   " + rndnumber());

// 		/*for(int k = 0;  k < I; k++){
// 			for(int i = 0;  i < 8; i++){
// 				ci = i + (8*k);
// 				if(index==0)
// 					colors[ci] = Color.Lerp(Color.green, Color.green, vertices[ci].y);
// 				else if(index==1)
// 					colors[ci] = Color.Lerp(Color.blue, Color.blue, vertices[ci].y);
// 				else if(index==2)
// 					colors[ci] = Color.Lerp(Color.black, Color.black, vertices[ci].y);
// 				else if(index==3)
// 					colors[ci] = Color.Lerp(Color.cyan, Color.cyan, vertices[ci].y);
// 				else if(index==4)
// 					colors[ci] = Color.Lerp(Color.gray, Color.gray, vertices[ci].y);
// 				else if(index==5)
// 					colors[ci] = Color.Lerp(Color.magenta, Color.magenta, vertices[ci].y);
// 				else if(index==6)
// 					colors[ci] = Color.Lerp(Color.red, Color.red, vertices[ci].y);
// 				else if(index==7)
// 					colors[ci] = Color.Lerp(Color.yellow, Color.yellow, vertices[ci].y);
// 			}

// 			index++;
// 			if(index == 8){
// 				index=0;
// 			}
// 		}*/


// 	/*	for(int k = 0;  k < I; k++){
// 			for(int i = 0;  i < 6; i++){
// 			ci = i + (8*k);
// 				index=i;
// 			if(index==0)
// 				colors[ci] = Color.Lerp(Color.green, Color.green, vertices[ci].y);
// 			else if(index==1)
// 				colors[ci] = Color.Lerp(Color.blue, Color.blue, vertices[ci].y);
// 			//else if(index==2)
// 			//	colors[ci] = Color.Lerp(Color.black, Color.black, vertices[ci].y);
// 			else if(index==2)
// 				colors[ci] = Color.Lerp(Color.cyan, Color.cyan, vertices[ci].y);
// 			//else if(index==3)
// 				//colors[ci] = Color.Lerp(Color.gray, Color.gray, vertices[ci].y);
// 			else if(index==3)
// 				colors[ci] = Color.Lerp(Color.magenta, Color.magenta, vertices[ci].y);
// 			else if(index==4)
// 				colors[ci] = Color.Lerp(Color.red, Color.red, vertices[ci].y);
// 			else if(index==5)
// 				colors[ci] = Color.Lerp(Color.yellow, Color.yellow, vertices[ci].y);
// 		}
// 	}*/

// 	//Arrange array [y] with no repeat values
// 		List<double> unique_y = new List<double>();
// 		for(int i = 0; i<y_array.Length; i++)
// 		{
// 			bool found = false;
// 			for(int prev=0; prev<i; prev++)
// 			{
// 				if(y_array[prev] == y_array[i])
// 				{
// 					found = true;
// 					break;
// 				}
// 			}

// 			if(!found)
// 			{
// 				//unique_y.Add(Math.Round(y_array[i],2));
// 			unique_y.Add(y_array[i]);
// 			}
// 		}
// 		//Debug.Log("Y_ARRAY   " + y_array.Length);
// 		y_array = unique_y.ToArray();


// 	//Color the octree cubes
// 	for(int i = 0;  i < y_array.Length; i++){
// 		for(int k = 0;  k < I; k++){
// 			if(y_array[i]==newverts[k].y){
// 				ColorLayer(k,index);
// 			}
// 		}
// 		index++;
// 		if(index == 6){
// 			index=0;
// 		}
// 		}
			


// 		/*triangles  = new int[]{
// 			    0, 2, 1, //face front
// 				0, 3, 2,
// 				2, 3, 4, //face top
// 				2, 4, 5,
// 				1, 2, 5, //face right
// 				1, 5, 6,
// 				0, 7, 4, //face left
// 				0, 4, 3,
// 				5, 4, 7, //face back
// 				5, 7, 6,
// 				0, 6, 7, //face bottom
// 				0, 1, 6,
// 			8, 10,9,
// 			8, 11,10,
// 			10, 11, 12, //face top
// 			10, 12, 13,
// 			9, 10, 13, //face right
// 			9, 13, 14,
// 			8, 15, 12, //face left
// 			8, 12, 11,
// 			13, 12, 15, //face back
// 			13, 15, 14,
// 			8, 14, 15,
// 			8, 9, 14
// 			};*/
	
// 		//triangles[1] = newtrigs;

// 		//vertices[0]=tempverts;



// 		 //vertices = new Vector3[]{
// 		//	new Vector3 (0, 0, 0),
// 		//	new Vector3 (1, 0, 0),
// 		//	new Vector3 (1, 1, 0),
// 		//	new Vector3 (0, 1, 0),
// 		//	new Vector3 (0, 1, 1),
// 		//	new Vector3 (1, 1, 1),
// 		//	new Vector3 (1, 0, 1),
// 		//	new Vector3 (0, 0, 1),
// 		//};


// 		update_mesh = true;

//     }
// }
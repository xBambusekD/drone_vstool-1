using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RosSharp.RosBridgeClient;
using Random=UnityEngine.Random;
using sensor_msgs = RosSharp.RosBridgeClient.Messages.Sensor;
using System.IO;
using Object = UnityEngine.Object;

public class PointCloudSubscriber1 : MonoBehaviour
{

	//public string uri = "ws://10.42.0.1:9090";
	public string uri = "ws://192.168.56.101:9090";
	private RosSocket rosSocket;
	string subscriptionId = "";

	public RgbPoint3[] Points;
	public GameObject PC2Octree;


	//Mesh components
	Mesh mesh;


	Vector3[] newverts;

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

	int[] choise;
	int index; 
	int ci;
	long delta = 1;
	int kk = 0;
	long Inew = 0;
	long Ikk = 0;
	int ninc = 0;
	int layer =0;
	private bool flag                 = true;
	private bool create_object        = false;
	private bool create_object_second = false;
	private bool create_mesh          = true;
	private bool create_mesh_second   = false;
	private bool flag_object          = false;
	private bool flag_object_init     = false;
	private bool flag_object_second   = false;
	private bool key                  = false;
	private bool key_second           = true;
	private bool keymesh              = true;
	private bool keyobject            = true;
	private bool key2                 = false;
	private bool key3                 = true;
	private bool allow_mesh           = false;
	private bool mesh_key             = false;
	int Ic = 0;
	int Ik = 4546;
	int objcount = 0;
	long Iold;


	private Vector3[] vertices;
	private int[] triangles;
	private Color[] colors;

	private bool update_mesh = false;

	List<GameObject> GObj = new List<GameObject>();
	List<Mesh> MObj = new List<Mesh>();



	// Start is called before the first frame update
	void Start()
	{
		Debug.Log("RosSocket Initialization!!!");
		rosSocket = new RosSocket(new  RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri)); // 10.189.42.225:9090
		Subscribe("/octomap_point_cloud_centers");
		//Subscribe("/zed/rtabmap/octomap_occupied_space"); 
	}



	void Update()
	{
		

		if((create_object==true) && (key ==true)){
			Mesh meshinit;
			GameObject newMeshGameObject = new GameObject("MeshObject");
			GObj.Add(newMeshGameObject);
			GObj[objcount].transform.parent = transform;

			MeshFilter meshFilter = GObj[objcount].AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = GObj[objcount].AddComponent<MeshRenderer>();
			meshinit = new Mesh();
			MObj.Add(meshinit);
			MObj[objcount] = GObj[objcount].GetComponent<MeshFilter>().mesh;
			GObj[objcount].GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material	;

			create_object=false;
			key=false;

		}
			
		if((create_mesh==true) && (key2 ==true)){

			MObj[objcount].Clear();
			MObj[objcount].vertices = vertices;
			MObj[objcount].triangles = triangles;
			MObj[objcount].colors = colors;

			create_mesh=false;
		
		}

	}





	public void Subscribe(string id)
	{
		subscriptionId  = rosSocket.Subscribe<sensor_msgs.PointCloud2>(id, SubscriptionHandler);
	}

	private IEnumerator WaitForKey()
	{
		Debug.Log("Press any key to close...");

		while (!Input.anyKeyDown)
		{
			yield return null;
		}

		Debug.Log("Closed");
		// rosSocket.Close();
	}


	public void SubscriptionHandler(sensor_msgs.PointCloud2 message)
	{


		long I = message.data.Length / message.point_step;

		RgbPoint3[] Points = new RgbPoint3[I];
		byte[] byteSlice = new byte[message.point_step];

		for (long i = 0; i < I; i++)
		{
			Array.Copy(message.data, i * message.point_step, byteSlice, 0, message.point_step);
			Points[i] = new RgbPoint3(byteSlice, message.fields);
		}


		newverts = new Vector3[I];
		double[] y_array = new double[I];

		//Assign all PointCloud points to the Vecto3[]
		for (var i = 0; i < I; i++)
		{
			newverts[i] = new Vector3(Points[i].x, Points[i].z ,Points[i].y);
			y_array[i] = newverts[i].y;
		}

		if(I<Ik){
			
			flag_object_init = true;
		}
		if(I>((layer+1)*Ik)){
			if(flag==true){
				layer++;
				flag=false;

			}
			if(I>(layer*Ik)){



				Ic=(int)Iold;//layer*Ik;
				flag=true;
				flag_object=true;
				create_mesh=false;
				objcount++;


			}
					
		}

		CreateMesh(I,Ic,newverts);
		Iold = I;
	
	}

	public void CreateMesh(long I, int Ic,Vector3[] newverts) {
   
		if(flag_object_init == true){


			if((I<Ik)&&(keyobject==true)){
				create_object = true;
				keyobject=false;
				key=true;
			}

			if((I<Ik)&&(keymesh==true)){

				Debug.Log(" IFIRST   " + I);
				Debug.Log(" IOLDFIRST   " + Iold);
				Debug.Log(" ICFIRST   " + Ic);
				FillVerts(I,Ic,newverts);
				create_mesh=true;
				//keymesh=false;
				key2 =true;
			}

		}

		if(flag_object==true){
			flag_object=false;

			create_object = true;
			create_mesh_second=true;
			key =true;

		}

		if(create_mesh_second==true){
				FillVerts(I,Ic,newverts);
				create_mesh=true;
				//keymesh=false;
				key2 =true;
			}





	}

	int FillVerts(long I, int Ic,Vector3[] newverts){


		float inc = 0.25f;
		//float inc = 0.15f;
		kk = Ic;

		vertices = new Vector3[(I-Ic)*8];
		//Debug.Log(" (I-kk)*8    "  + (I-kk)*8);
		int vinc=0;
		for(int k=kk; k < I; k++){
			for(int i = 0;  i < (I-Ic)*8; i++){
				if(i==0+vinc){
					vertices[i] = new Vector3(newverts[k].x - inc, newverts[k].y - inc, newverts[k].z + inc);
				}
				else if(i==1+vinc){
					vertices[i] = new Vector3(newverts[k].x - inc, newverts[k].y - inc, newverts[k].z - inc);
				}
				else if(i==2+vinc){
					vertices[i] = new Vector3(newverts[k].x - inc, newverts[k].y + inc, newverts[k].z - inc);
				}
				else if(i==3+vinc){
					vertices[i] = new Vector3(newverts[k].x - inc, newverts[k].y + inc, newverts[k].z + inc);
				}
				else if(i==4+vinc){
					vertices[i] = new Vector3(newverts[k].x + inc, newverts[k].y + inc, newverts[k].z + inc);
				}
				else if(i==5+vinc){
					vertices[i] = new Vector3(newverts[k].x + inc, newverts[k].y + inc, newverts[k].z - inc);
				}
				else if(i==6+vinc){
					vertices[i] = new Vector3(newverts[k].x + inc, newverts[k].y - inc, newverts[k].z - inc);
				}
				else if(i==7+vinc){
					vertices[i] = new Vector3(newverts[k].x + inc, newverts[k].y - inc, newverts[k].z + inc);
				}

			}
			vinc = vinc + 8;
		}



		int tris = 0;//36;
		int vert = 0;//8;

		triangles  = new int[36*(I-Ic)];

		//Assign triangules of all the vertices
		//Debug.Log("Working on Triangles");
		for(int k = kk;k<I;k++){
			for(int i = 0; i < 36; i++){
				if(i==0){
					triangles[i + tris] = 0 + vert;
				}
				else if(i==1){
					triangles[i + tris] = 2 + vert;
				}
				else if(i==2){
					triangles[i + tris] = 1 + vert;
				}
				else if(i==3){
					triangles[i + tris] = 0 + vert;
				}
				else if(i==4){
					triangles[i + tris] = 3 + vert;
				}
				else if(i==5){
					triangles[i + tris] = 2 + vert;
				}
				else if(i==6){
					triangles[i + tris] = 2 + vert;
				}
				else if(i==7){
					triangles[i + tris] = 3 + vert;
				}
				else if(i==8){
					triangles[i + tris] = 4 + vert;
				}
				else if(i==9){
					triangles[i + tris] = 2 + vert;
				}
				else if(i==10){
					triangles[i + tris] = 4 + vert;
				}
				else if(i==11){
					triangles[i + tris] = 5 + vert;
				}
				else if(i==12){
					triangles[i + tris] = 1 + vert;
				}
				else if(i==13){
					triangles[i + tris] = 2 + vert;
				}
				else if(i==14){
					triangles[i + tris] = 5 + vert;
				}
				else if(i==15){
					triangles[i + tris] = 1 + vert;
				}
				else if(i==16){
					triangles[i + tris] = 5 + vert;
				}
				else if(i==17){
					triangles[i + tris] = 6 + vert;
				}

				else if(i==18){
					triangles[i + tris] = 0 + vert;
				}
				else if(i==19){
					triangles[i + tris] = 7 + vert;
				}
				else if(i==20){
					triangles[i + tris] = 4 + vert;
				}
				else if(i==21){
					triangles[i + tris] = 0 + vert;
				}
				else if(i==22){
					triangles[i + tris] = 4 + vert;
				}
				else if(i==23){
					triangles[i + tris] = 3 + vert;
				}
				else if(i==24){
					triangles[i + tris] = 5 + vert;
				}
				else if(i==25){
					triangles[i + tris] = 4 + vert;
				}
				else if(i==26){
					triangles[i + tris] = 7 + vert;
				}
				else if(i==27){
					triangles[i + tris] = 5 + vert;
				}
				else if(i==28){
					triangles[i + tris] = 7 + vert;
				}
				else if(i==29){
					triangles[i + tris] = 6 + vert;
				}
				else if(i==30){
					triangles[i + tris] = 0 + vert;
				}
				else if(i==31){
					triangles[i + tris] = 6 + vert;
				}
				else if(i==32){
					triangles[i + tris] = 7 + vert;
				}
				else if(i==33){
					triangles[i + tris] = 0 + vert;
				}
				else if(i==34){
					triangles[i + tris] = 1 + vert;
				}
				else if(i==35){
					triangles[i + tris] = 6 + vert;
				}
			}
			vert+=8;
			tris+=36;
		}

		colors = new Color[vertices.Length];


		index = 0;
		ci    = 0;

		//Debug.Log("Working with colors");
		for(int k = 0;  k < (I-Ic); k++){
			//Debug.Log("Color 1");
			for(int i = 0;  i < 8; i++){
				//Debug.Log("Color 2");	
				ci = i + (8*k);
				if(index==0){
					//Debug.Log("Color 3");	
					//Debug.Log("Vertix   " + vertices[ci].y);
					colors[ci] = Color.Lerp(Color.green, Color.green, vertices[ci].y);
				}
				else if(index==1){
					//Debug.Log("Color 4");	
					colors[ci] = Color.Lerp(Color.blue, Color.blue, vertices[ci].y);
				}
				else if(index==2){
					//Debug.Log("Color 5");	
					colors[ci] = Color.Lerp(Color.black, Color.black, vertices[ci].y);
				}
				else if(index==3){
					//Debug.Log("Color 6");	
					colors[ci] = Color.Lerp(Color.cyan, Color.cyan, vertices[ci].y);
				}
				else if(index==4){
					//Debug.Log("Color 7");	
					colors[ci] = Color.Lerp(Color.gray, Color.gray, vertices[ci].y);
				}
				else if(index==5){
					//Debug.Log("Color 8");	
					colors[ci] = Color.Lerp(Color.magenta, Color.magenta, vertices[ci].y);
				}
				else if(index==6){
					//Debug.Log("Color 9");	
					colors[ci] = Color.Lerp(Color.red, Color.red, vertices[ci].y);
				}
				else if(index==7){
					//Debug.Log("Color 10");	
					colors[ci] = Color.Lerp(Color.yellow, Color.yellow, vertices[ci].y);
				}
			}

			index++;
			if(index == 8){
				index=0;
			}
		}

		return 0;
	}

}

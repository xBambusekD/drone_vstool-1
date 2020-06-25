/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class DroneImageRawSubscriber : Subscriber<Messages.Sensor.Image> , IDroneImageSubscriber
    {
        public Material material;
        public Material projectorMaterial;

        public int width = 672;
        public int height = 376;
        public bool optimalizeForProjector = false;
        //private bool textureResized = false;

        public bool OptimalizeForProjector //z interfacu
        {
            get { return optimalizeForProjector; }
            set { optimalizeForProjector = value; }
        }

        public Texture2D texture2D;
        private byte[] imageData;
        private bool isMessageReceived;

        private MyRosConnector droneRosConnector;

        public void Awake()
        {
            TimeStep = PlayerPrefs.GetFloat("VideoTimeStep");
            Topic = PlayerPrefs.GetString("VideoTopic");
            width = PlayerPrefs.GetInt("CameraResWidth");
            height = PlayerPrefs.GetInt("CameraResHeight");
        }

        protected override void Start()
        {
            base.Start();

            texture2D = new Texture2D(width, height, TextureFormat.BGRA32, false);
            texture2D.wrapMode = TextureWrapMode.Clamp;


            if (material != null)
            {
                material.SetTexture("_MainTex", texture2D);
                material.SetTextureScale("_MainTex", new Vector2(1, -1)); //z nějakýho důvodu to zobrazuje zrcadlově otočený,tím to to vrátím
            }
            if (projectorMaterial != null)
            {
                projectorMaterial.SetTexture("_MainTex", texture2D);
                projectorMaterial.SetTextureScale("_MainTex", new Vector2(1, 1));
            }

            droneRosConnector = gameObject.GetComponent<MyRosConnector>();
        }
        private void Update()
        {
            if (isMessageReceived)
            {
                ProcessMessage();
                if (droneRosConnector != null) droneRosConnector.RecievedMessageSetTime();
            }

        }

        protected override void ReceiveMessage(Messages.Sensor.Image image)
        {
            imageData = image.data;
            isMessageReceived = true;
        }

        /*
        //byla snaha automaticky upravitvelikost textury podle videa, bohužel to nefunguje
        protected void Resize() //toto z nějakého důvodu nefunguje v případě, že se to volá v čase běhu aplikace...Dokonce to spadne a Resized se nevypíše, asi chyba v UNITY
        {
            texture2D.Resize(672, 376);
            texture2D.Apply(false);
            Debug.Log("Resized");
            textureResized = true;
        }
        */

        private void ProcessMessage()
        {
            texture2D.LoadRawTextureData(imageData);

            //pro využití pro projektor jenutné, aby textura měla 1px rámeček
            if (optimalizeForProjector)
            {
                int y = 0;
                for (int x = 0; x < texture2D.width; x++) //first line
                {
                    texture2D.SetPixel(x, y, Color.white);
                }

                for (y = 1; y < texture2D.height - 1; y++)
                {
                    texture2D.SetPixel(0, y, Color.white);
                    texture2D.SetPixel(texture2D.width - 1, y, Color.white);
                }


                y++;
                for (int x = 0; x < texture2D.width; x++) //first line
                {
                    texture2D.SetPixel(x, y, Color.white);
                }
            }

            texture2D.Apply();


            if (!optimalizeForProjector) //klasicke platno 
                texture2D.wrapMode = TextureWrapMode.Repeat;

            else
                texture2D.wrapMode = TextureWrapMode.Clamp;

            isMessageReceived = false;
        }
    }
}
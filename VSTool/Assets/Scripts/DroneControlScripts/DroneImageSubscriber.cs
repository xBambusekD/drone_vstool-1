/*
Author: Bc. Kamil Sedlmajer
*/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class DroneImageSubscriber : Subscriber<Messages.Sensor.CompressedImage>, IDroneImageSubscriber
    {
        private Texture2D texture2D;
        public Material material;
        public Material projectorMaterial;

        public int width = 672;
        public int height = 376;
        private RawImage rawImage;

        public bool optimalizeForProjector = false;

        private byte[] imageData;
        private bool isMessageReceived;

        private MyRosConnector droneRosConnector;

        public bool OptimalizeForProjector
        {
            get { return optimalizeForProjector; }
            set { optimalizeForProjector = value; }
        }

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

            GameObject.Find("MainCanvas/VideoMovableWindow").transform.localScale = new Vector3(1, 1, 1);
            texture2D = new Texture2D(width, height); //, TextureFormat.BGRA32, false
            texture2D.wrapMode = TextureWrapMode.Clamp;

            if (material != null)
            {
                material.SetTexture("_MainTex", texture2D);
                material.SetTextureScale("_MainTex", new Vector2(1, 1)); //z nějakýho důvodu to zobrazuje zrcadlově otočený, tím to to vrátím
            }
            if (projectorMaterial != null)
            {
                projectorMaterial.SetTexture("_MainTex", texture2D);
                projectorMaterial.SetTextureScale("_MainTex", new Vector2(1, 1));
            }
            
            droneRosConnector = gameObject.GetComponent<MyRosConnector>();
            rawImage = GameObject.Find("MainCanvas/VideoMovableWindow/Content/Video").GetComponent<RawImage>();
            rawImage.texture = texture2D;
            GameObject.Find("MainCanvas/VideoMovableWindow").SetActive(false);

        }

        private void Update()
        {
            if (isMessageReceived)
            {
                ProcessMessage();
                if(droneRosConnector!=null) droneRosConnector.RecievedMessageSetTime();
            }
        }

        protected override void ReceiveMessage(Messages.Sensor.CompressedImage compressedImage)
        {
            imageData = compressedImage.data;
            isMessageReceived = true;
        }


        private void ProcessMessage()
        {
            //texture2D.LoadRawTextureData(imageData);pro nekomprimovaná data by to bylo takto
            texture2D.LoadImage(imageData);

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


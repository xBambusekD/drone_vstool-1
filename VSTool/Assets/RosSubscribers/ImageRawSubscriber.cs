/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class ImageRawSubscriber : Subscriber<Messages.Sensor.Image>
    {
        private Texture2D texture2D;
        public Material material;
        public Material projectorMaterial;

        public int width = 672;
        public int height = 376;

        //private Shader standardShader;
        //private Shader projectorShader;

        public bool optimalizeForProjector=false;
        //private bool textureResized = false;

        private byte[] imageData;
        private bool isMessageReceived;
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
        }
        private void Update()
        {
            if (isMessageReceived)
            {
                ProcessMessage();
            }
        }

        protected override void ReceiveMessage(Messages.Sensor.Image image)
        {
            imageData = image.data;
            //Debug.Log("ReceiveMessage: video image"+ imageData.Length);

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


            /* pokud by bylo nutné aktualizovat jeden pixel po druhém, naštěstí se ukázalo,že to není nutné, v současném unity jde nastavit format ntextury na TextureFormat.BGRA32 */

            //pro využití pro projektor jenutné, aby textura měla 1px rámeček
            if (optimalizeForProjector)
            {
                int y = 0;
                for (int x = 0; x < texture2D.width; x++) //first line
                {
                    texture2D.SetPixel(x, y, Color.white);
                }

                for (y = 1; y < texture2D.height-1; y++)
                {
                    texture2D.SetPixel(0, y, Color.white);
                    texture2D.SetPixel(texture2D.width-1, y, Color.white);
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
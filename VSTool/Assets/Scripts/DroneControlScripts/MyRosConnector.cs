/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace RosSharp.RosBridgeClient
{
    public class MyRosConnector : RosConnector
    {
        public int ConnectionState=0;

        private float lastMessageTime;

        private float lastAliveTime;
        /*
         * není možné zapisovat do GUI z jiného vlákna... proto se to dělá takto a GUI si musí hodnoty číst samo
         *  0 = connecting
         *  1 = connected
         *  2 = problematic
         *  3 = Probably dead
         * -1 = disconnected
         */

        private bool SubscribersEnabled = false;

        //Awake is called when the script instance is being loaded. Awake is called only once. Awake is always called before any Start functions.
        public  void Awake()
        {
            RosBridgeServerUrl = PlayerPrefs.GetString("RosBridgeURL");

            new Thread(ConnectAndWait).Start();
            Debug.Log("RosBridge: MyRosConnector Awake , Connecting URL "+ RosBridgeServerUrl);

            //nastavim parametry komponent
            
            if (PlayerPrefs.GetInt("VideoCompression") > 0)
            {
                DroneImageRawSubscriber imagerRawSubscriber = gameObject.GetComponent<DroneImageRawSubscriber>();
                Destroy(imagerRawSubscriber); //nepotrebnou komponentu na video odstranim
            }
            else
            { 
                DroneImageSubscriber imageSubscriber = gameObject.GetComponent<DroneImageSubscriber>();
                Destroy(imageSubscriber); //nepotrebnou komponentu na video odstranim
            }
        }

        void EnableSubscribers() //pokud je nechám aktivovaný od začátku, háže to exceptiony 
        {
            Debug.Log("EnableSubscribers");

           MonoBehaviour[] subs = GetComponents<MonoBehaviour>();
            for (int i = 0; i < subs.Length; i++)
            {
                subs[i].enabled = true;
            }
        }

        protected override void OnConnected(object sender, EventArgs e)
        {
            isConnected.Set();
            Debug.Log("Connected to RosBridge: " + RosBridgeServerUrl);
            //EnableSubscribers();
            ConnectionState = 1;
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            try
            {
                
                Debug.Log("Disconnected from RosBridge: " + RosBridgeServerUrl);
                ConnectionState = -1;
                isConnected.Reset();
                //Awake(); //další pokus
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, this);
            }

        }

        protected void Update()
        {
            if(ConnectionState>0 && !SubscribersEnabled)
            {
                SubscribersEnabled = true;
                EnableSubscribers();
            }

            if (ConnectionState>0)
            {
                /* nefunguje podle ocekavani
                if (RosSocket.protocol.IsAlive())
                {
                    ConnectionState = 1;
                    lastAliveTime = Time.realtimeSinceStartup;
                }
                else
                {
                    if ((Time.realtimeSinceStartup - lastAliveTime) > 5)
                        ConnectionState = 3;
                    else if ((Time.realtimeSinceStartup - lastAliveTime) > 2)
                        ConnectionState = 2;
                }
                */

                if ((Time.realtimeSinceStartup - lastMessageTime) > 5)
                    ConnectionState = 3;
                else if ((Time.realtimeSinceStartup - lastMessageTime) > 1)
                    ConnectionState = 2;
                else
                    ConnectionState = 1;
            }
        }

        //pomocí tohoto hlásí subscribeři, jestli jim došla zpráva, když dlouho nepřijde nic, prohlásím spojení za mrtvé
        public void RecievedMessageSetTime()
        {
            lastMessageTime = Time.realtimeSinceStartup;
        }
    }
}

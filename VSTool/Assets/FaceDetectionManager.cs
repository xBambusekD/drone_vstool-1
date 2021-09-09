using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class FaceDetectionManager : MonoBehaviour
{
    WebSocket ws;
    public int maxX = 1280;
    public int maxY = 720;

    [Serializable]
    public class FaceRegCoordinates {
        public int x;
        public int y;
        public int mh;
        public int mw;
        public int h;
    }
    public FaceRegCoordinates faceRegCoordinates = new FaceRegCoordinates();

    private void Start() {
        ws = new WebSocket("ws://192.168.56.101:8765");
        
        ws.Connect();
        
        ws.OnMessage += (sender, e) => {
            
            faceRegCoordinates = JsonUtility.FromJson<FaceRegCoordinates>(e.Data);
        };
    }

    void Update() {
        transform.localPosition = new Vector3(-calculatePosition(faceRegCoordinates.x, this.maxX),
            -calculatePosition(faceRegCoordinates.y, this.maxY), transform.position.z);
    }

    private float calculatePosition(int x, int max) {
        int centerCoordinate = max / 2;
        
        int diff = x - centerCoordinate;
        
        float result = (float) ((0.5 * diff) / centerCoordinate);
        return result;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GstCustomTexture))]
public class RTMPConverter : MonoBehaviour {
    private GstCustomTexture m_Texture;
    private string pipeline = "rtmpsrc location=rtmp://localhost:1935/live/test ! decodebin ! video/x-raw,format=I420 ! videoconvert ! x264enc name=videoEnc bitrate=2000 tune=zerolatency pass=qual !  rtph264pay ! udpsink host=127.0.0.1 port=1338 sync=false -v";
    // Start is called before the first frame update
    void Start() {
        m_Texture = gameObject.GetComponent<GstCustomTexture>();
        m_Texture.Initialize();
        m_Texture.SetPipeline(pipeline);
        m_Texture.Player.CreateStream();
        m_Texture.Player.Play();
    }
}

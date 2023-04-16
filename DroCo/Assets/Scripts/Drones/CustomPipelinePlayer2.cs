using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(GstCustomTexture))]
public class CustomPipelinePlayer2 : BaseVideoPlayer {

    public string pipeline = "";

    public DroneVehicleData VehicleData {
        get; set;
    }

    // Use this for initialization
    protected override string _GetPipeline() {
        string P = pipeline + " ! video/x-raw,format=I420 ! videoconvert ! appsink name=videoSink";

        return P;
    }

    protected override void Update() {
        base.Update();
        if (VehicleData != null) {
            foreach (MyRect r in VehicleData.rects) {
                Graphics.DrawTexture(new Rect(r.x, r.y, r.w, r.h), VideoTexture);
            }
        }
    }
}

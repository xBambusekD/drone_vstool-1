using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : Singleton<MissionManager> {

    private List<DistanceBillboard> distancesList = new List<DistanceBillboard>();

    public void AddToDistanceList(DistanceBillboard billboard) {
        distancesList.Add(billboard);
    }

    public void ChangeTargetFaceCamera(Transform target) {
        foreach (DistanceBillboard billboard in distancesList) {
            billboard.ObjectToFace = target;
        }
    }

    public void ChangeTarget(Transform target) {
        foreach (DistanceBillboard billboard in distancesList) {
            billboard.ObjectToComputeDistance = target;
        }
    }
}

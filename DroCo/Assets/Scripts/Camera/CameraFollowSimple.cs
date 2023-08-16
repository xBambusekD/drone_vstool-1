using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowSimple : MonoBehaviour {

    private Transform transformToFollow;

    private bool followingTarget;

    private void Update() {
        if (followingTarget && transformToFollow != null) {
            transform.position = transformToFollow.position;
            transform.rotation = transformToFollow.rotation;
        }
    }

    public void StartFollowing(Transform tf) {
        transformToFollow = tf;
        followingTarget = true;
    }

    public void StopFollowing() {
        transformToFollow = null;
        followingTarget = false;
    }
}

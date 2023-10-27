using System.Collections;
using System.Collections.Generic;
using Highlighters;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour {

    [SerializeField]
    private HighlighterTrigger HighlighterTrigger;

    public Camera TPVCamera;
    public Camera FPVOnlyCamera;
    public Transform DroneModel;

    [SerializeField]
    public CustomPipelinePlayer PipelinePlayer;

    public virtual void Highlight(bool highlight) {
        HighlighterTrigger.ChangeTriggeringState(highlight);
    }

    public virtual void FocusCamera() {
        CameraManager.Instance.StartFollowingTarget(transform, TPVCamera.transform);
    }

    public virtual void SetCameras() {
        CameraManager.Instance.SetCurrentInteractiveObject(this);
    }

    public virtual Texture GetCameraTexture() {
        return PipelinePlayer.VideoTexture;
    }

    public abstract void ChangeFlightDataDelay(float delay);

}

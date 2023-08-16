using System.Collections;
using System.Collections.Generic;
using Highlighters;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour {

    [SerializeField]
    private HighlighterTrigger HighlighterTrigger;

    public Camera TPVCamera;
    public Camera FPVOnlyCamera;

    public virtual void Highlight(bool highlight) {
        HighlighterTrigger.ChangeTriggeringState(highlight);
    }

    public virtual void FocusCamera() {
        CameraManager.Instance.StartFollowingTarget(TPVCamera.transform);
    }

}

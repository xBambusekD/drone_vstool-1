using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaypointGameObject : MonoBehaviour {

    [SerializeField]
    private RectTransform connectionBinder;
    public RectTransform ConnectionBinder => connectionBinder;

    [SerializeField]
    private TMP_Text text;

    public void SetText(string txt) {
        text.text = txt;
    }
}

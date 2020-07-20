using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SidePanelWidth : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rt = transform.GetComponent< RectTransform >( );
        rt.sizeDelta = new Vector2 (rt.sizeDelta.x, Screen.width*0.8f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

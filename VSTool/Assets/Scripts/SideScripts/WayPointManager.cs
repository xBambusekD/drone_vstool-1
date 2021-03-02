using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WayPointManager : MonoBehaviour
{
    public Image Icon;
    public Transform Waypoint;
    public TextMeshProUGUI Distance;

    public Camera cam;
    

    void Update()
    {
        float minX = Icon.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width*0.8f - minX;

        Vector2 pos = cam.WorldToScreenPoint(Waypoint.position);

        // Nie je to v obraze
        if(Vector3.Dot((Waypoint.position - transform.position), transform.forward) < 0){
            if(pos.x < Screen.width / 2)
                pos.x = maxX;
            else
                pos.x = minX;
        }
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, Icon.GetPixelAdjustedRect().height / 2 +20, Screen.height*0.96f - Icon.GetPixelAdjustedRect().height / 2);

        Icon.transform.position = pos;
        float dist = Vector3.Distance(Drones.drones[0].DroneGameObject.transform.position,Waypoint.position);
        Distance.text = Mathf.Round(dist) + "m";
    }
}

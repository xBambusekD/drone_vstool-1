using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconManager : MonoBehaviour
{
    public Transform iconTargetTransform;
    public GameObject iconPrefab;

    public Camera cam;
    public Camera defcam;
    public Camera mapcam;
    // Start is called before the first frame update
    private bool _onScreen;

    public GameObject Drone;

    public Transform middle;
 

    public int distanceOfSight = 50;
    
    private int i = 0;
    void Start()
    {
        foreach(Drone item in Drones.drones)
        {
            GameObject icon = (GameObject)Instantiate(iconPrefab);
            icon.transform.SetParent(iconTargetTransform);
            icon.name = "icon" + item.DroneGameObject.name;
            icon.transform.localScale = new Vector3(1,1,1);
            icon.gameObject.SetActive(false);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        float minimumDistance = distanceOfSight;
        float maximumDistance = 100.0F;
        
        float minimumDistanceScale = 1.0F;
        float maximumDistanceScale = 0.0F;
        
        

        if(GuiController.isMap)
            cam = mapcam;
        else
            cam = defcam;

        foreach(Transform child in iconTargetTransform.transform)
        {
            Image img = child.GetComponent<Image>();
            Image arrow = child.transform.Find("Arrow").GetComponent<Image>();
            arrow.gameObject.transform.right = middle.position - arrow.gameObject.transform.transform.position;
            arrow.color = img.color;

            float minX = img.GetPixelAdjustedRect().width / 2 +10;
            float maxX = Screen.width*0.8f - minX -10;

            float minY = img.GetPixelAdjustedRect().height / 2 +30; // 20 vyska textu vzdialenosti
            float maxY = Screen.height*0.96f - img.GetPixelAdjustedRect().height / 2 -10;

            Vector2 pos = cam.WorldToScreenPoint(Drones.drones[i].DroneGameObject.transform.position);
            
            if(Vector3.Dot((Drones.drones[i].DroneGameObject.transform.position - cam.transform.position), cam.transform.forward) < 0)
                if(pos.x < Screen.width / 2)
                    pos.x = maxX;
                else
                    pos.x = minX;
         
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            img.transform.position = pos;

            // // Ziskam text so vzdialenostou
            TextMeshProUGUI text = child.GetComponentInChildren<TextMeshProUGUI>();
            // Ziskam vzdialenost
            float dist = Vector3.Distance(Drones.drones[0].DroneGameObject.transform.position,Drones.drones[i].DroneGameObject.transform.position);
            text.text = Mathf.Round(dist) + "m";


            float norm = (dist - minimumDistance) / (maximumDistance - minimumDistance);
            norm = Mathf.Clamp01(norm);
            
            var minScale = Vector3.one * maximumDistanceScale;
            var maxScale = Vector3.one * minimumDistanceScale;
            
            child.transform.localScale = Vector3.Lerp(maxScale, minScale, norm);

            Vector3 iconPos = cam.WorldToScreenPoint(Drones.drones[i].DroneGameObject.transform.position);
            _onScreen = cam.pixelRect.Contains( iconPos ) && iconPos.z > cam.nearClipPlane;
            
            // && dist > 20.0
            if(_onScreen && i!= 0){
                img.enabled = true;
                arrow.enabled = false;
            } else {
                img.enabled = false;
                arrow.enabled = true;
            }
            i++;
        }
         i=0;
    }
}

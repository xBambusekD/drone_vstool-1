using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


public class Drones : MonoBehaviour
{
    public static List<Drone> drones = new List<Drone>();

    public static List<GameObject> icones = new List<GameObject>();

    public Transform tarfetTransform;
    public GameObject dronesPrefab;
    private TextMeshProUGUI[] droneName;
    public Camera c;
    float distance = 0;

    private GameObject droneBody;
    private GameObject activeDrone;

    public RenderTexture PopUpRenderTexture;
    private int i = 0;
    // Start is called before the first frame update
    public GameObject PopUp;
    void Start()
    {
        foreach(Drone item in drones)
        {
            GameObject droneDisplay = (GameObject)Instantiate(dronesPrefab);
            droneDisplay.transform.SetParent(tarfetTransform);
            droneName = droneDisplay.GetComponentsInChildren<TextMeshProUGUI>();
            droneName[0].text = item.DroneGameObject.name;
            droneName[1].text = drones.Count.ToString();
            droneDisplay.transform.localScale = new Vector3(1,1,1);
            droneDisplay.GetComponent<LookAtDrone>().PopUp = PopUp;
            droneDisplay.GetComponent<LookAtDrone>().RenderTexture = PopUpRenderTexture;
        }
    }


    // Update is called once per frame
    void Update()
    {
        foreach(Transform child in tarfetTransform.transform)
        {
            droneName = child.GetComponentsInChildren<TextMeshProUGUI>();
            distance = Vector3.Distance(drones[0].DroneGameObject.transform.position,drones[i].DroneGameObject.transform.position);
            droneName[1].text = Mathf.Round(distance) + "m";
            i++;
        }
        i=0;
    }

    public static void DroneAdded(Transform tarfetTransform, GameObject dronesPrefab,Transform iconTargetTransform,GameObject iconPrefab,GameObject PopUp,RenderTexture PopUpRenderTexture)
    {
        GameObject item = drones[drones.Count - 1].DroneGameObject;
        GameObject droneDisplay = (GameObject)Instantiate(dronesPrefab);
        droneDisplay.transform.SetParent(tarfetTransform);
        TextMeshProUGUI[] droneName = droneDisplay.GetComponentsInChildren<TextMeshProUGUI>();
        droneName[0].text = item.name;
        droneName[1].text = "0m";
        droneDisplay.GetComponent<LookAtDrone>().PopUp = PopUp;
        droneDisplay.GetComponent<LookAtDrone>().RenderTexture = PopUpRenderTexture;
        float distance = Vector3.Distance(drones[0].DroneGameObject.transform.position, item.transform.position);
        Debug.Log(distance);   

        // Icon
        GameObject icon = (GameObject)Instantiate(iconPrefab);
        icon.transform.SetParent(iconTargetTransform);
        icon.name = "icon" + item.name;
        icon.transform.localScale = new Vector3(1,1,1);
        Color col = new Color(
        Random.Range(0f, 1f), 
        Random.Range(0f, 1f), 
        Random.Range(0f, 1f),
        0.9F
        );
        icon.GetComponent<Image>().color = col;
        droneDisplay.transform.localScale = new Vector3(1,1,1);

        droneDisplay.transform.Find("Icon").GetComponent<Image>().color = col;
    }
    
    // public static void LookFunction(int i, GameObject PopUp){
    //     PopUp.SetActive(true);


    //     // if(GuiController.isMap){
    //     //     Vector3 newPosition = drones[i].transform.position;
    //     //     newPosition.y = Camera.main.transform.position.y;
    //     //     Camera.main.transform.position = newPosition;
    //     // } else {
    //     //     Transform Camera = drones[0].transform.Find("CameraBox/MainCameraPair");
    //     //     Debug.Log(Camera.gameObject.name);
    //     //     Camera.LookAt(Drones.drones[i].transform);
    //     // }   
        
    // }

    // public static void DontLookFunction(int i,GameObject PopUp){
    //     PopUp.SetActive(false);
    //     // if(GuiController.isMap){

    //     // } else {
    //     // Transform Camera = drones[0].transform.Find("CameraBox/MainCameraPair");
    //     // Camera.LookAt(Drones.drones[0].transform);
    //     // }
    // }
}

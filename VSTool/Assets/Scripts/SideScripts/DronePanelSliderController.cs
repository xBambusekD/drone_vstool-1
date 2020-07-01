using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DronePanelSliderController : MonoBehaviour
{
    public GameObject DronePanel;
    public GameObject DroneScroll;

    public Text NumberOfDrones;
    // Start is called before the first frame update
    private bool isShown = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        NumberOfDrones.text = Drones.drones.Count.ToString() + " drones in scene";
    }

    public void ShowPanel(){
        if(isShown){
            DronePanel.SetActive(false);
            DroneScroll.SetActive(false);

            isShown = !isShown;
        } else {
            DronePanel.SetActive(true);
            DroneScroll.SetActive(true);
            isShown = !isShown;
        }
    }
}

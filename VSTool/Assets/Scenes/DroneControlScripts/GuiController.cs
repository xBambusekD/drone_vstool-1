/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mapbox.Unity.Map;
using TMPro;
using Michsky.UI.ModernUIPack;

public class GuiController : MonoBehaviour
{
    //public GameObject videoScreen;
    //public GameObject videoProjector;
    public GameObject Settings;
    public GameObject MissionHandler;
    public GameObject HomePoint;
    public GameObject WayPoint;
    public AbstractMap Map;

    public GameObject droneObject; // potřebuju kvůli získání odkazu na connector
    public GameObject Navigation;
    private DroneController droneController;
    private NavigationController navigationController;
    private MapClickController mapClickController;
    private int activeNavigationPointId = -1;

    public Button NavigationButton;
    public Button DefineAreaButton;
    public Button ShowAreaButton;
    public Button ModeButton;
    public Button MinimapButton;
    public RawImage MinimapRawImage;

    public GameObject WayPointPanel;

    public GameObject ClickModePanel;

    public int MinimapState = 0;

    // Mapa premenne
    public GameObject MainCanvas;
    public GameObject MapCanvas;
    public GameObject MainCameras;
    public GameObject MapCamera;

    public TMP_InputField RosConnectorIF;
    public TMP_InputField Topic;
    public static bool isMap = false;

    public SwitchManager OctomapSwitch;
    
    // Start is called before the first frame update
    void Start()
    {
        droneController = droneObject.GetComponent<DroneController>();
        navigationController = Navigation.GetComponent<NavigationController>();
        mapClickController = Map.GetComponent<MapClickController>();

        // switchButton(ShowBuildingsButton, !ShowBuildings.BuildingsHidden);
        // switchButton(HomePointButton, true);

        changeModeIcon();

        ScreenButtonClick(); //vypnu screen,v defaultu je totiz zapnuty

        droneController.ShowOctomap(OctomapSwitch.isOn);
    }



    // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyUp("b"))
    //     {
    //         ScreenButtonClick(); 
    //     }

    //     if (Input.GetKeyUp("c"))
    //     {
    //         ReconnectButtonClick();
    //     }

    //     if (Input.GetKeyUp("l"))
    //     {
    //         ShowBuildingsButtonClick();
    //     }

    //     if (Input.GetKeyUp("m"))
    //     {
    //         droneController.changeDataSource(1);
    //         changeModeIcon();
    //     }

    //     if (Input.GetKeyUp("s"))
    //     {
    //         droneController.changeDataSource(0);
    //         changeModeIcon();
    //     }

    //     if (Input.GetKeyUp("r"))
    //     {
    //         droneController.changeDataSource(2);
    //         changeModeIcon();
    //     }

    // }

    public void loadMission(){
        MissionHandler.SetActive(true);
        HomePoint.SetActive(true);
        WayPoint.SetActive(true);
    }

    public void changeTopic(){
        PlayerPrefs.SetString("VideoTopic",Topic.text);
    }

    public void ChangeOctomapTopic(string topicName) {
        PlayerPrefs.SetString("OctomapTopic", topicName);
        droneController.ChangeOctomapTopic(topicName);
    }

    public void SettingsButtonClick(){
        Settings.SetActive(!Settings.activeSelf);
    }

    public void changeModeIcon()
    {
        int mode = droneController.getDataSource();

        if(mode==0) ModeButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("GUI/style02/rand_style02_color10");
        else if(mode == 1) ModeButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("GUI/style02/gamepad_style02_color10");
        else ModeButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("GUI/style02/ROS_style02_color10");
    }


    public void ScreenButtonClick()
    {
        droneController.SwitchVideoScreen();

        // switchButton(ScreenButton, droneController.isVideoScreenActive);
        // switchButton(ProjectorButton, droneController.isProjectorActive);
    }

    public void ShowOctomap() {
        droneController.ShowOctomap(true);
    }

    public void HideOctomap() {
        droneController.ShowOctomap(false);
    }

    public void ProjecorButtonClick()
    {
        droneController.SwitchProjector();

        // switchButton(ScreenButton, droneController.isVideoScreenActive);
        // switchButton(ProjectorButton, droneController.isProjectorActive);
    }

    public void OpenMap()
    {
        isMap = true;
        ShowBuildings.BuildingsHidden = true;
        MainCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(true);
        MainCameras.gameObject.SetActive(false);
        MapCamera.gameObject.SetActive(true);
    }


    public void HomeButtonClick()
    {
       bool res = navigationController.ChangeHomeArrowActivity(); 
    //    switchButton(HomePointButton, res);
    }

    public void SetHomeButtonClick()
    {
        //Debug.Log("SetHomeButtonClick");
        navigationController.ChangeHomePosition(droneObject.transform.localPosition, true);
    }

    public void ModeButtonClick()
    {
        //Debug.Log("ModeButtonClick");
        droneController.changeDataSource();
        changeModeIcon();
    }


    private void OpenWayPointPanel()
    {
        Button WayPointButtonPrefab = Resources.Load<Button>("GUI/WayPointButtonPrefab");
        Button WayPointDeleteButtonPrefab = Resources.Load<Button>("GUI/WpDeleteButtonPrefab");
        //Transform waypoints = WayPointPanel.transform.Find("WayPoints");

        //odstraním starý seznam bodů
        foreach (Transform child in WayPointPanel.transform)
        {
            if (child.gameObject.tag == "WayPointButton") GameObject.Destroy(child.gameObject);
        }

        WayPointPanel.SetActive(true);

        int i = 0;
        List<NavigationPoint> points = navigationController.getPoints();
        // WayPointPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(155, 3 * 32 + points.Count * 32);

        foreach (NavigationPoint point in points)
        {
            Button guiPoint = Instantiate(WayPointButtonPrefab, WayPointPanel.transform, false);//WayPointPanel.transform.GetChild(i).GetComponent<Button>();
            int index = i; //toto je nutný,aby se vytvořila kopie toho objektu
            guiPoint.onClick.AddListener(() => { this.NavigateToPoint(index); });
            guiPoint.transform.Find("ColorRect").gameObject.GetComponent<Image>().color = point.color;
            guiPoint.transform.Find("Text").gameObject.GetComponent<Text>().text = point.name;
            guiPoint.GetComponent<RectTransform>().anchoredPosition = new Vector3(guiPoint.GetComponent<RectTransform>().anchoredPosition.x, -112 - 32 * (i), 0);//-32*(i)

            Button guiDeletePoint = Instantiate(WayPointDeleteButtonPrefab, WayPointPanel.transform, false);
            guiDeletePoint.GetComponent<RectTransform>().anchoredPosition = new Vector3(guiDeletePoint.GetComponent<RectTransform>().anchoredPosition.x, -112 - 32 * (i), 0);//-32*(i)
            guiDeletePoint.onClick.AddListener(() => { this.DeleteNavigPoint(index); });
            i++;
        }
    }

    public void NavigationButtonClick()
    {
        if (!WayPointPanel.activeSelf)
        {
            OpenWayPointPanel();
        }
        else
            StopNavigationButtonClick();
    }

    public void StopNavigationButtonClick()
    {
        WayPointPanel.SetActive(false);
        navigationController.showNavigationArrow(-1);
        activeNavigationPointId = -1;
        switchButton(NavigationButton, false);
    }

    public void NavigateToPoint(int pointIndex) {
        WayPointPanel.SetActive(false);

        navigationController.showNavigationArrow(pointIndex);
        activeNavigationPointId = pointIndex;
        switchButton(NavigationButton, true);
    }

    public void DeleteNavigPoint(int pointIndex)
    {
        if (activeNavigationPointId == pointIndex) //je aktivni navigace k tomuto bodu, zruším ji
            StopNavigationButtonClick();

        navigationController.DeletePoint(pointIndex);
        OpenWayPointPanel();
    }

    public void AddNavigationPointByClick()
    {
        Debug.Log("AddNavigationPointByClick");
        mapClickController.mode = MapClickController.MapClickMode.WayPoints;
        ClickModePanel.SetActive(true); 

        WayPointPanel.SetActive(false);
    }

    //toto je volano zpet z map controlleru, když v tomto režimu je kliknuto do mapy
    public void OnMapClickInWaypointMode(Vector3 position, float distance)
    {
        navigationController.AddPoint(position, true);
        WayPointPanel.SetActive(false);

        TextMeshProUGUI WPInsertedText = Instantiate(Resources.Load<TextMeshProUGUI>("GUI/WayPointInserted"));
        WPInsertedText.transform.parent = transform;
        WPInsertedText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,210,0);
        WPInsertedText.text = "WayPoint inserted!";
        Destroy(WPInsertedText, 2.5F);

        mapClickController.mode = MapClickController.MapClickMode.Off;
        ClickModePanel.SetActive(false);
    }

    public void AddNavigationPointToActualPos()
    {
        navigationController.AddPoint(droneObject.transform.localPosition, false);
        WayPointPanel.SetActive(false);
    }

    public void ShowBuildingsButtonClick()
    {
        ShowBuildings.BuildingsHidden = !ShowBuildings.BuildingsHidden;
        // switchButton(ShowBuildingsButton, !ShowBuildings.BuildingsHidden);
    }
    public void ChangeIP(string value){
        Debug.Log(RosConnectorIF.text);
        PlayerPrefs.SetString("RosBridgeURL", RosConnectorIF.text);
        ReconnectButtonClick();
    }

    public void ChangeDrocoServerIP(string value) {
        // reconnect only if url really changed
        if (!WebSocketManager.Instance.APIDomainWS.Equals(value)) {
            Debug.Log("Reconnecting to the DroCo server: " + value);
            PlayerPrefs.SetString("DrocoServerURL", value);
            WebSocketManager.Instance.ReconnectToServer(value);
        }
    }
    
    public void ReconnectButtonClick()
    {
        droneController.ConnectToRos();
    }

    public void DefineAreaButtonClick()
    {
        Debug.Log("DefineAreaButtonClick");
        //mapClickController.enabled = (!mapClickController.enabled);
        bool active = (mapClickController.mode == MapClickController.MapClickMode.Zones);
        
        if (active) mapClickController.mode = MapClickController.MapClickMode.Off;
        else mapClickController.mode = MapClickController.MapClickMode.Zones;

        active = !active; 
        switchButton(DefineAreaButton, active);
        ShowZones.ZonePointsHidden = !active;

        //nejde editovat skryte zony, proto si je zobrazim
        if (active && ShowZones.ZonesHidden) ShowAreaButtonClick(); 
    }

    public void ShowAreaButtonClick()
    {
        Debug.Log("ShowAreaButtonClick");
        bool active = ShowZones.ZonesHidden; 
        ShowZones.ZonesHidden = !active;
        // switchButton(ShowAreaButton, active);

        //nejde editovat zony, kdyz nejsou zobrazene, proto je take skryju
        if (!active && !ShowZones.ZonePointsHidden) DefineAreaButtonClick();
    }


    private void switchButton(Button b, bool value)
    {
        if(value)
            b.GetComponent<Image>().color = new Color(0.1f, 1.0f, 0.1f, 1.0f); 
        else
            b.GetComponent<Image>().color = Color.white;
    }


}

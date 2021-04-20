/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using TMPro;

public class NavigationController : MonoBehaviour
{
    public AbstractMap Map;

    private bool positionActual = false;
    private int PointCounter = 1;

    public GameObject HomeArrow;
    public GameObject NavigationArrow;

    public TextMeshProUGUI HomeDistanceText;
    public TextMeshProUGUI NavigationDistanceText;

    public TextMeshProUGUI IconDistance;

    public GameObject HomePointObject;
   // public GameObject droneObject;
    private NavigationPoint HomePoint;
    private List<NavigationPoint> navigationPoints = new List<NavigationPoint>();
    private NavigationPoint activeNavigationPoint;

    private Color[] colorArray = new Color[] {Color.red, Color.blue, Color.cyan, Color.magenta,  Color.yellow, new Color(1, 0.5f, 0.5f, 0.8f), new Color(1, 0.5f, 0f, 0.8f), Color.white};

    public List<NavigationPoint> getPoints() { return navigationPoints; }

    void Start()
    {
        HomePoint = new NavigationPoint(new Color(0,1,0, 0.8f), "Home", true, HomePointObject);

        /*
        NavigationPoint point1 = new NavigationPoint(new Color(1, 0, 0, 0.8f), "Point Red", true, HomePointObject);
        navigationPoints.Add(point1);
        NavigationPoint point2 = new NavigationPoint(new Color(1,1, 1, 0.8f), "Point White", true, HomePointObject);
        navigationPoints.Add(point2);
        NavigationPoint point3 = new NavigationPoint(new Color(1, 0.5f, 0.5f, 0.8f), "Point Pink", true, HomePointObject);
        navigationPoints.Add(point3);
        */
    }

    public void AddPoint(Vector3 position, bool onGround)
    {
        Color col = colorArray[navigationPoints.Count % 8];

        GameObject WayPointPointerPrefab;

        if(!onGround) WayPointPointerPrefab = Resources.Load<GameObject>("Prefabs/WayPointPointer");
        else WayPointPointerPrefab = Resources.Load<GameObject>("Prefabs/WayPointGroundPointer");

        GameObject WayPointPointer = Instantiate(WayPointPointerPrefab);
        WayPointPointer.transform.localPosition = position;
        WayPointPointer.transform.parent = transform;
        WayPointPointer.GetComponent<Renderer>().material.color = new Color(col.r, col.g, col.b, 0.5f);

        NavigationPoint point = new NavigationPoint(col, "Point "+ PointCounter, onGround, WayPointPointer);
        PointCounter++;
        navigationPoints.Add(point);
    }

    public void DeletePoint(int index)
    {
        NavigationPoint point = navigationPoints[index];
        GameObject.Destroy(point.pointObject);
        navigationPoints.RemoveAt(index);
    }


        // Update is called once per frame
        void Update()
    {
        if (!positionActual) //INICIALIZACE NA ZAČÁTKU. neni ale možné provest ve start, protože ještě není inicializovaná mapa
        {
            ChangeHomePosition(HomePoint.pointObject.transform.localPosition, true);
            positionActual = true;
        }

        HomeArrow.transform.LookAt(HomePoint.pointObject.transform);

        float dist = Vector3.Distance(HomeArrow.transform.position, HomePoint.pointObject.transform.position);
        HomeDistanceText.text = Mathf.Round(dist) + "m";
        IconDistance.text = Mathf.Round(dist) + "m";

        if (activeNavigationPoint != null)
        {
            NavigationArrow.transform.LookAt(activeNavigationPoint.pointObject.transform);

            float dist2 = Vector3.Distance(NavigationArrow.transform.position, activeNavigationPoint.pointObject.transform.position);
            NavigationDistanceText.text =  Mathf.Round(dist2) + "m";
        }
    }

    //show and hide navigation arrow
    public bool ChangeHomeArrowActivity()
    {
        HomeArrow.SetActive(!HomeArrow.activeSelf);
        HomeDistanceText.gameObject.SetActive(HomeArrow.activeSelf);

        if (HomeArrow.activeSelf && activeNavigationPoint!=null)
            showBothArrows();
        else
            showOneArrow();

        return HomeArrow.activeSelf;
    }


        //for pointID=0 navigatios arrowwill be hidden
        public void showNavigationArrow(int pointID)
    {
        if(pointID>=0 && navigationPoints[pointID]!=null)
        {
            NavigationArrow.SetActive(true);
            NavigationDistanceText.gameObject.SetActive(true);

            activeNavigationPoint = navigationPoints[pointID];

            //teď se probourat dovnitř a nastavit barvičky jednotlivých komponent
            NavigationArrow.transform.GetChild(0).Find("arrow").gameObject.GetComponent<Renderer>().material.color = activeNavigationPoint.color;
            NavigationArrow.transform.GetChild(0).Find("backside").gameObject.GetComponent<Renderer>().material.color = activeNavigationPoint.backsideColor;

            if (HomeArrow.activeSelf)
                showBothArrows();
            else
                showOneArrow();
        }
        else
        {
            NavigationArrow.SetActive(false);
            NavigationDistanceText.gameObject.SetActive(false);
            activeNavigationPoint = null;
            showOneArrow();  
        }
    }

    //pozice se zadává v unity Units
    public void ChangeHomePosition(Vector3 position, bool onGround)
    {
        if (HomePoint.onGround)
        {
            float groundAltitude = Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(position));
            HomePoint.pointObject.transform.localPosition = new Vector3(position.x, groundAltitude, position.z);
        }
        else
            HomePoint.pointObject.transform.localPosition = position;

        HomePoint.onGround = onGround;
    }

    public Vector3 getHomePosition()
    {
        return transform.localPosition;
    }


    protected void showOneArrow()
    {
        HomeArrow.transform.localPosition = new Vector3(0, HomeArrow.transform.localPosition.y, HomeArrow.transform.localPosition.z);
        NavigationArrow.transform.localPosition = new Vector3(0, NavigationArrow.transform.localPosition.y, NavigationArrow.transform.localPosition.z);

        Vector3 homeTextPos = HomeDistanceText.GetComponent<RectTransform>().anchoredPosition;
        HomeDistanceText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, homeTextPos.y, homeTextPos.z);

        Vector3 navPointTextPos = NavigationDistanceText.GetComponent<RectTransform>().anchoredPosition;
        NavigationDistanceText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, navPointTextPos.y, navPointTextPos.z);
    }

    protected void showBothArrows()
    {
        HomeArrow.transform.localPosition = new Vector3(-0.1f, HomeArrow.transform.localPosition.y, HomeArrow.transform.localPosition.z);
        NavigationArrow.transform.localPosition = new Vector3(0.1f, NavigationArrow.transform.localPosition.y, NavigationArrow.transform.localPosition.z);

        Vector3 homeTextPos = HomeDistanceText.GetComponent<RectTransform>().anchoredPosition;
        HomeDistanceText.GetComponent<RectTransform>().anchoredPosition = new Vector3(-30,homeTextPos.y, homeTextPos.z);

        Vector3 navPointTextPos = NavigationDistanceText.GetComponent<RectTransform>().anchoredPosition;
        NavigationDistanceText.GetComponent<RectTransform>().anchoredPosition = new Vector3(30, navPointTextPos.y, navPointTextPos.z);
    }
}

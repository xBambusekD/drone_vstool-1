/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;

public class HomePointController : MonoBehaviour
{
    public AbstractMap Map;

    private bool positionActual=false;
    private bool OnGround = false;
    public GameObject  arrow;
    public GameObject droneObject;

    //show and hide navigation arrow
    public bool ChangeArrowActivity() 
    {
        arrow.SetActive(!arrow.activeSelf);
        return arrow.activeSelf;
    }

    // Update is called once per frame
    void Update()
    {
        if (!positionActual) //INICIALIZACE NA ZAČÁTKU. neni ale možné provest ve start, protože ještě není inicializovaná mapa
        {
            ChangeHomePoint(transform.localPosition, true);
            positionActual = true;
        }

        arrow.transform.LookAt(transform);
    }

    public void ChangeHomePoint(Vector3 position, bool onGround)
    {
        if (OnGround)
        {
            float groundAltitude = Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(transform.localPosition));
            Debug.Log("HomePoint altitude " + groundAltitude);
            transform.localPosition = new Vector3(transform.localPosition.x, groundAltitude, transform.localPosition.z);
        }
        else
            transform.localPosition = position;

        OnGround = onGround;
    }

    public Vector3 getPosition()
    {
        return transform.localPosition;
    }
}

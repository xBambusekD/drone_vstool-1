/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShowZones : MonoBehaviour
{
    public static bool ZonesHidden = false;
    private bool thisCamZonesHidden = false;

    public static bool ZonePointsHidden = false;
    private bool thisCamZonePointsHidden = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ShowZones.ZonesHidden != thisCamZonesHidden)
        {
            Camera cam = gameObject.GetComponent<Camera>();

            if (thisCamZonesHidden)
                cam.cullingMask |= 1 << LayerMask.NameToLayer("zoneWall");
            else
                cam.cullingMask &= ~(1 << LayerMask.NameToLayer("zoneWall"));// ignore  buildings layer

            thisCamZonesHidden = ShowZones.ZonesHidden;
            Debug.Log("oblasti skryty/odkryty");
        }

        if (ShowZones.ZonePointsHidden != thisCamZonePointsHidden)
        {
            Camera cam = gameObject.GetComponent<Camera>();

            if (thisCamZonePointsHidden)
                cam.cullingMask |= 1 << LayerMask.NameToLayer("zonePoint");
            else
                cam.cullingMask &= ~(1 << LayerMask.NameToLayer("zonePoint"));// ignore  buildings layer

            thisCamZonePointsHidden = ShowZones.ZonePointsHidden;
            Debug.Log("body oblasti skryty/odkryty");
        }
    }
}

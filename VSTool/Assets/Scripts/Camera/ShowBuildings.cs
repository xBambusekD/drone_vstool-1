/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBuildings : MonoBehaviour
{
    public static bool BuildingsHidden = false;
    private bool thisCamBuildingsHidden = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if (ShowBuildings.BuildingsHidden != thisCamBuildingsHidden)
        {
            Camera cam = gameObject.GetComponent<Camera>();

            if (thisCamBuildingsHidden)
                cam.cullingMask |= 1 << LayerMask.NameToLayer("buildings");
            else
                cam.cullingMask &= ~(1 << LayerMask.NameToLayer("buildings"));// ignore  buildings layer

            thisCamBuildingsHidden = ShowBuildings.BuildingsHidden;
            Debug.Log("budovy skryty/odkryty");
        }
    }
}

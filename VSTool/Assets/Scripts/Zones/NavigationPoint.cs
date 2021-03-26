/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;

public class NavigationPoint 
{
    public Color color;
    public Color backsideColor;
    public string name;
    public bool onGround = true;
    public GameObject pointObject;

    public NavigationPoint(Color _color, string _name,  bool _onGround, GameObject _pointObject)
    {
        color = _color;
        backsideColor = new Color(color.r*0.4f, color.g * 0.4f, color.b * 0.4f, 0.9f);
        name = _name;
        onGround = _onGround;
        pointObject =_pointObject;
    }
}

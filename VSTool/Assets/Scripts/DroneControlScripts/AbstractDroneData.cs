/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;

public abstract class AbstractDroneData
{
    protected Vector3 position;
    protected Vector3 startPos;
    protected float groundAltitude;
    protected float rotation;
    protected AbstractMap Map;

    public AbstractDroneData(AbstractMap map, Vector3 defPos)
    {
        position = defPos;
        rotation = 0;
        startPos = defPos;
        Map = map;
    }

    public virtual void update() //volano v každém kroku pro získání nových hodnot
    {
        groundAltitude = Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(position));
    }

    //volano při každém přepnutí zdroje dat, zresetuje rychlosti akcelerace a podobně, dale nastavipozici
    abstract public void reset(Vector3 pos, Vector3 rot);

    //vrací výšku od terénu v metrech
    public float getGroundAltitute()
    {
        return groundAltitude;
    }

    //vrací vzdálenost od HomePosition v metrech
    public float GetHomeDistance()
    {
        return Mathf.Sqrt(Mathf.Pow(startPos.x - position.x, 2) + Mathf.Pow(startPos.z - position.z, 2));
    }

    //vrací výšku od terénu v metrech
    public float getGroundAltitude()
    {
        groundAltitude = Map.QueryElevationInUnityUnitsAt(Map.WorldToGeoPosition(position));
        return groundAltitude;
    }

    abstract public Vector3 GetPosition();
    abstract public Vector3 GetRotation();
    abstract public Vector3 GetPitchRoll();
    abstract public Vector3 GetCameraRotation();
}
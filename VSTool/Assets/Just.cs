using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
public class Just : MonoBehaviour
{
    public AbstractMap Map;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Kokot(){
        Mapbox.Utils.Vector2d p = new Mapbox.Utils.Vector2d(49.227689,16.597898);
        Vector3 j= Map.GeoToWorldPosition(new Mapbox.Utils.Vector2d(49.227689,16.597898),false);
        Debug.Log(j);
    }
}


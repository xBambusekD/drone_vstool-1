using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[Serializable]
public class jsonDataClass{
    public double height;
    public double latitude;
    public double longitude;
}



public class UrlHandler : MonoBehaviour
{
    public jsonDataClass jsonData;
    public string jsonURL;
    // Start is called before the first frame update
    void Update()
    {
        StartCoroutine(getData());
        
    }

    IEnumerator getData(){
        WWW _www = new WWW(jsonURL);
        yield return _www;
        if(_www.error == null){
            processJsonData(_www.text);
        } else {
            Debug.Log("error");
        }
    }

    private void processJsonData(string _url){
        jsonDataClass jsonData = JsonUtility.FromJson<jsonDataClass>(_url);
        Debug.Log(jsonData.height);
        Debug.Log(jsonData.latitude);
        Debug.Log(jsonData.longitude);
    }

}

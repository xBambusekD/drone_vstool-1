using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
public class SaveManager : MonoBehaviour
{
    public GameObject savesListGO;
    // Start is called before the first frame update
    void Start()
    {
        GameObject savePrefab = Resources.Load<GameObject>("GUI/SaveItem");
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/Saves/");
        FileInfo[] info = dir.GetFiles("*.json");

        foreach (FileInfo f in info)
        {
            GameObject saveItem = Instantiate(savePrefab);
            saveItem.GetComponentInChildren<TextMeshProUGUI>().text = f.Name;
            saveItem.transform.SetParent(savesListGO.transform);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

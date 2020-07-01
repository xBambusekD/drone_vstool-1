using UnityEngine;
 
 public class LineHandler : MonoBehaviour
 {
     public GameObject objectOne;    // The first object you instantiate
    
     public GameObject parent;

     public int dis = 10;
 
    Color c1 = Color.white;
    Color c2 = new Color(1, 1, 1, 0);


    void Start(){
        int i = 0;
        foreach(Transform child in parent.transform){
                GameObject empty = new GameObject("line");
                empty.transform.SetParent(transform);
                empty.AddComponent<LineRenderer>();
            i++;
        }
        i = 0;
        
    }
     void Update()
     {
         int i = 0;
         transform.gameObject.SetActive(true);
         foreach(Transform child in parent.transform){
                  if(Vector3.Distance(transform.position,child.position) < dis){
                      transform.gameObject.SetActive(true);
                        LineRenderer[] lineRend = transform.GetComponentsInChildren<LineRenderer>(true);
                        LineRenderer tmp = lineRend[i];
                        tmp.gameObject.SetActive(true);
                        tmp.positionCount = 2;
                        Transform first = objectOne.transform;
                        Transform second = child;
                    
                        tmp.SetPosition(0, first.position);
                        tmp.SetPosition(1, second.position);
                        tmp.startWidth = 0.05F;
                        tmp.endWidth = 0.05F;
                        tmp.material.color = Color.white;
                        if(Vector3.Distance(transform.position,child.position) < 2){
                            tmp.material.color = Color.red;
                        }
                  } else {
                    LineRenderer[] lineRend = transform.GetComponentsInChildren<LineRenderer>(true);
                    LineRenderer tmp = lineRend[i];
                    tmp.gameObject.SetActive(false);
                  }
              i++;
         }
         i=0;
     }


 }
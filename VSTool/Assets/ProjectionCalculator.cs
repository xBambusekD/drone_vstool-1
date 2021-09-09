using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionCalculator : MonoBehaviour
{
    public Camera ProjectorCamera;
    public Vector3 poss;
    public GameObject Cube;
    public GameObject Cube1;
    public GameObject Cube2;
    public GameObject Cube3;
    // Start is called before the first frame update
    void Start() {
        ProjectorCamera = transform.gameObject.GetComponent<Camera>();
        poss = new Vector3(0, 0, 0);
    }

    void Update() {
        
    }

    // Update is called once per frame
   public void Calculate() {
       Ray ray = ProjectorCamera.ScreenPointToRay(poss);
       Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
       RaycastHit hit;
       if (Physics.Raycast(ray, out hit)) {
           Cube.transform.position = hit.point;
       }
       Ray ray2 = ProjectorCamera.ScreenPointToRay(new Vector3(ProjectorCamera.pixelWidth - 1, ProjectorCamera.pixelHeight - 1, 0));
       Debug.DrawRay(ray2.origin, ray2.direction * 10, Color.yellow);
       if (Physics.Raycast(ray2, out hit)) {
           Cube1.transform.position = hit.point;
       }
       Ray ray3 = ProjectorCamera.ScreenPointToRay(new Vector3(0, ProjectorCamera.pixelHeight - 1, 0));
       Debug.DrawRay(ray3.origin, ray3.direction * 10, Color.yellow);
       if (Physics.Raycast(ray3, out hit)) {
           Cube2.transform.position = hit.point;
       }
       Ray ray4 = ProjectorCamera.ScreenPointToRay(new Vector3(ProjectorCamera.pixelWidth - 1, 0, 0));
       Debug.DrawRay(ray4.origin, ray4.direction * 10, Color.yellow);
       if (Physics.Raycast(ray4, out hit)) {
           Cube3.transform.position = hit.point;
       }

    }
}

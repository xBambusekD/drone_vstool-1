using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class PersonDetectionManager : MonoBehaviour
{
    WebSocket ws;
    public Camera ProjectorCamera;
    public int maxX = 1280;
    public int maxY = 720;
    public GameObject generator;
    public GameObject prefab;
    private int instantiatedObjects = 0;
    private List<GameObject> persons = new List<GameObject>();
    private bool wasGenerated = false;

    [Serializable]
    public class PeopleRegCoordinates {
        public int mh;
        public int mw;
        public int count;
        public List<Coordinates> persons;
    }

    public PeopleRegCoordinates peopleRegCoordinates;


    [Serializable]
    public class Coordinates {
        public int x;
        public int y;
    }

    // Start is called before the first frame update
    void Start()
    {
        peopleRegCoordinates = new PeopleRegCoordinates();
        peopleRegCoordinates.persons = new List<Coordinates>();
        peopleRegCoordinates.count = 0;
        ws = new WebSocket("ws://192.168.56.101:8765");
      
        ws.Connect();

        ws.OnMessage += (sender, e) => {
            peopleRegCoordinates = JsonUtility.FromJson<PeopleRegCoordinates>(e.Data);
        };
    }

    private int c = 0;

    // Update is called once per frame
    void Update()
    {
        c++;
        wasGenerated = false;
        if (c == 10) {
            for (int i = 0; i < peopleRegCoordinates.count - instantiatedObjects; i++) {
                GameObject person = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
                person.transform.parent = generator.transform;
                persons.Add(person);
                wasGenerated = true;
            }
            if (wasGenerated)
                instantiatedObjects = peopleRegCoordinates.count;
          
            for (int i = 0; i < peopleRegCoordinates.count; i++) {
                persons[i].SetActive(true);
               
                float x = convertCoordinate(peopleRegCoordinates.persons[i].x, peopleRegCoordinates.mw, ProjectorCamera.pixelWidth - 1);
                float y = convertCoordinate(peopleRegCoordinates.persons[i].y, peopleRegCoordinates.mh, ProjectorCamera.pixelHeight - 1);
                y = ProjectorCamera.pixelHeight - 1 - y;
                Vector3 personPosition = new Vector3(x, y, 0);
                RaycastHit hit;
                Ray ray = ProjectorCamera.ScreenPointToRay(personPosition);
                if (Physics.Raycast(ray, out hit)) {
                    persons[i].transform.position = hit.point;
                }
            }
            c = 0;

            for (int i = peopleRegCoordinates.persons.Count; i < instantiatedObjects; i++) {
                persons[i].SetActive(false);
            }

        }
        

        
    }

    private float convertCoordinate(int coordinate, int maxCoordinate, int maxNew) {
        return ((float)coordinate / (float) maxCoordinate) * (float) maxNew;
    }
}

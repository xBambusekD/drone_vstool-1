using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    private bool topCameraMode = false;

    private bool left = false;
    private bool right = false;
    private bool up = false;
    private bool down = false;

    private bool leftFree = false;
    private bool rightFree = false;
    private bool upFree = false;
    private bool downFree = false;

    private bool FreeModeSet = false;
    private bool CockpitModeSet = false;
    private bool StandardModeSet = true;

    public GameObject DroneGameObject;
    public GameObject DroneModel;
    public GameObject VideoScreen;
    public GameObject DownDangerGameObject;
    public GameObject UPDangerGameObject;
    public GameObject DroneDangerGameObject;
    public GameObject VideoUI;
    public GameObject Buttons;
    public GameObject CompassGameObject;
    public GameObject DroneModelGameObject;
    public IconManager Icons;
    public GameObject IconsGO;
    public GameObject PitchAndScroollGameObject;
    public GameObject MainCameras;
    public GameObject ResetButton;
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 0.0f;
    public float maxDistance = 5;
    public float minDistance = 0;
    public float xSpeed = 5.0f;
    public float ySpeed = 5.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public float zoomRate = 100.0f;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    public Joystick Joystick;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    private Vector3 FirstPosition;
    private Vector3 SecondPosition;
    private Vector3 delta;
    private Vector3 lastOffset;
    private Vector3 lastOffsettemp;
    // Update is called once per frame

    private void SetGameObjects(bool FreeModeDisabled)
    {
        IconsGO.transform.GetChild(0).gameObject.SetActive(!FreeModeDisabled);
        VideoUI.SetActive(!FreeModeDisabled);
        VideoScreen.SetActive(FreeModeDisabled);
        DownDangerGameObject.SetActive(FreeModeDisabled);
        UPDangerGameObject.SetActive(FreeModeDisabled);
        DroneDangerGameObject.SetActive(FreeModeDisabled);
        CompassGameObject.SetActive(FreeModeDisabled);
        ResetButton.SetActive(FreeModeDisabled);

        #if UNITY_ANDROID
        Joystick.gameObject.SetActive(!FreeModeDisabled);
        #endif
    }
    void Start() { Init(); }
    void OnEnable() { Init(); }


    public void resetRotation()
    {
        transform.localRotation = new Quaternion(0, 0, 0,0);
    }
    public void Init()
    {
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    void LateUpdate()
    {
        if (!FreeModeSet)
        {
            float scroll = 0;

            scroll = Input.GetAxis("CameraZoom") * 0.5f;
            if ((MainCameras.transform.localPosition.z > -7 || scroll > 0) && (MainCameras.transform.localPosition.z < 0.8f || scroll < 0))
            {
                MainCameras.transform.localPosition = MainCameras.transform.localPosition + new Vector3(0, 0, scroll * 0.4f);
            }

            //movement
            if (Input.GetKeyUp("k")) //set initial position to 3rd person view
            {
                transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (Input.GetKeyUp("j")) //set initial position to 3rd person view
            {
                transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            }
            else
            {
                //movement
                float moveVertical = Input.GetAxis("Vertical");
                float moveHorizontal = Input.GetAxis("Horizontal");

                Vector3 rotace = new Vector3(moveVertical * -0.8f, moveHorizontal * 0.8f, 0);

                //move up 
                if (transform.localRotation.eulerAngles.x >= 90.0f && transform.localRotation.eulerAngles.x < 180.0f && rotace.x > 0) rotace.x = 0;

                //down
                if (transform.localRotation.eulerAngles.x <= 330.0f && transform.localRotation.eulerAngles.x > 180.0f && rotace.x < 0) rotace.x = 0;

                //move to sides
                if (transform.localRotation.eulerAngles.y >= 45.0f && transform.localRotation.eulerAngles.y < 180.0f && rotace.y > 0) rotace.y = 0;
                if (transform.localRotation.eulerAngles.y <= 315.0f && transform.localRotation.eulerAngles.y > 180.0f && rotace.y < 0) rotace.y = 0;

                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotace);
            }
        }

        float horizontalMove = Joystick.Horizontal * 0.05f;
        float verticalMove = Joystick.Vertical * 0.05f;

        transform.position += transform.right * horizontalMove;
        transform.position += transform.up * verticalMove;

        if (CockpitModeSet)
            CompassGameObject.transform.eulerAngles = new Vector3(0, 0, DroneModelGameObject.transform.eulerAngles.x);
        else
            CompassGameObject.transform.eulerAngles = new Vector3(0, 0, 0);

#if UNITY_ANDROID
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);

            Touch touchOne = Input.GetTouch(1);



            Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;

            Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;



            float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;

            float TouchDeltaMag = (touchZero.position - touchOne.position).magnitude;



            float deltaMagDiff = prevTouchDeltaMag - TouchDeltaMag;

            desiredDistance += deltaMagDiff * Time.deltaTime * zoomRate * 0.0025f * Mathf.Abs(desiredDistance);
            if (FreeModeSet)
                transform.position -= transform.forward * (deltaMagDiff * Time.deltaTime * zoomRate * 0.0025f );
        }
        // If middle mouse and left alt are selected? ORBIT
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchposition = Input.GetTouch(0).deltaPosition;
            xDeg += touchposition.x * xSpeed * 0.002f;
            yDeg -= touchposition.y * ySpeed * 0.002f;
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            transform.eulerAngles = new Vector3(yDeg, xDeg, 0);
        }

        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        if (!FreeModeSet)
        {
            position = target.position - (rotation * transform.forward * currentDistance);
            transform.position = position;
        }
#endif
    }
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void SetFreeMode()
    {
        transform.GetComponent<FreeCamera>().enabled = true;
        FreeModeSet = true;
        Icons.setFreeMode();
        SetGameObjects(false);
        transform.parent = null;
        StandardModeSet = false;
        CockpitModeSet = false;
        
    }

    public void SetCokcpitMode()
    {
        transform.GetComponent<FreeCamera>().enabled = false;
        StandardModeSet = false;
        CockpitModeSet = true;
        FreeModeSet = false;
        
        Icons.unsetFreeMode();
        SetGameObjects(true);
        transform.SetParent(DroneModel.transform.GetChild(2).transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        targetOffset = new Vector3(0, 0, 0);

        StandardModeSet = false;
        CockpitModeSet = true;
        FreeModeSet = false;
    }

    public void SetStandardMode()
    {
        transform.GetComponent<FreeCamera>().enabled = false;
        StandardModeSet = true;
        CockpitModeSet = false;
        FreeModeSet = false;
        Icons.unsetFreeMode();
        SetGameObjects(true);
        transform.SetParent(DroneGameObject.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        targetOffset = new Vector3(0, 0, 0);

        StandardModeSet = true;
        CockpitModeSet = false;
        FreeModeSet = false;
    }
}

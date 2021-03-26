using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;

class DroneDataManual : AbstractDroneData
{
    private float moveX, moveY, moveZ;
    private float rotate, sensitivity;

    public DroneDataManual(AbstractMap map, Vector3 defPos) : base(map, defPos)
    {
        position = defPos;
        rotation = 0;
        startPos = defPos;
        moveY = 0.5f;
    }

    public override void reset(Vector3 pos, Vector3 rot) //zresetuje rychlosti akcelerace
    {
        moveX = moveX = moveZ = rotate = 0; //ResetRotation rychlosti akcelerace
        position = pos;
        rotation = rot.y;
    }

    public override void update()
    {
        //s větší výškou chceme umožnit rychlejší pohyb,u země naopak přesnější
        sensitivity = ((float)position.y - (float)groundAltitude) / 110f + 0.07f;
        if (sensitivity > 1.0f) sensitivity = 1.0f;

        moveY = Input.GetAxis("Throttle"); //dame si tu mensi citlivost
        moveX = Input.GetAxis("Roll");
        moveZ = Input.GetAxis("Pitch");
        rotate = Input.GetAxis("Yaw");


        if (position.y <= (groundAltitude + 0.1) && moveY < 0) moveY = 0; //nechci, aby šlo létat pod terén


        Vector3 pohyb = new Vector3(moveX, moveY, moveZ);
        Vector3 rotatedVector = Quaternion.AngleAxis(rotation, Vector3.up) * pohyb;
        pohyb = rotatedVector;

        position = position + pohyb * Time.deltaTime * 80 * sensitivity;

        if (position.y < groundAltitude)
            position.y = groundAltitude;

        rotation = rotation + rotate * Time.deltaTime * 50 * (0.2f + sensitivity);


        base.update(); //zavolám metodu rodiče
    }

    public override Vector3 GetPosition()
    {
        return position;
    }

    public override Vector3 GetRotation()
    {
        return new Vector3(0, rotation, 0);
    }

    public override Vector3 GetPitchRoll()
    {
        return new Vector3((15.0f * (sensitivity + 0.5f) * moveX), 0, (15.0f * (sensitivity + 0.5f) * moveZ));
    }


    public override Vector3 GetCameraRotation()
    {
        return new Vector3(0,0,0);  //nestabilozovaná kamera
        //return -1 * GetPitchRoll(); //stabilozovaná kamera
    }
}
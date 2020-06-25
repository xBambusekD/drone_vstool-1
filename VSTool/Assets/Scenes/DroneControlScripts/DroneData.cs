using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;

class DroneData : AbstractDroneData
{
    private float moveX, moveY, moveZ;
    private float rotate;
   

    public DroneData(AbstractMap map, Vector3 defPos) : base(map, defPos)
    {
        position = defPos;
        rotation = 0;
        startPos = defPos;
        moveY = 0.5f;
    }

    public override void reset(Vector3 pos, Vector3 rot) //zresetuje rychlosti akcelerace
    {
        moveX = moveX = moveZ = rotate = 0; //reset rychlosti akcelerace
        position = pos;
        rotation = rot.y;
    }

    public override void update()
    {
         moveX = moveX + Random.Range(-0.01f, 0.01f); // creates a number between 1 and 12
         moveY = moveY + Random.Range(-0.05f, 0.05f);   // creates a number between 1 and 6
         moveZ = moveZ + Random.Range(-0.01f, 0.01f);

         if (moveX > 1.0f) moveX = 1.0f;
         if (moveY > 0.7f) moveY = 0.7f;
         if (moveZ > 1.0f) moveZ = 1.0f;
         if (moveX < -1.0f) moveX = -1.0f;
         if (moveY < -0.7f) moveY = -0.7f;
         if (moveZ < -1.0f) moveZ = -1.0f;

         if (position.x < -400) { moveX = moveX + Random.Range(0, 0.08f); }
         if (position.x > 400) { moveX = moveX - Random.Range(0, 0.08f); }
         if (position.z < -400) { moveZ = moveZ + Random.Range(0, 0.08f); }
         if (position.z > 400) { moveZ = moveZ - Random.Range(0, 0.08f); }

         if (position.y < (groundAltitude + 1)) { moveY = moveY + Random.Range(0.03f, 0.1f); }
         if (position.y >= groundAltitude + 220) { moveY = moveY - Random.Range(0, 0.05f); }

         rotate = rotate + Random.Range(-1.0f, 1.0f);
         if (rotate > 15) rotate = 15;
         if (rotate < -15) rotate = -15;
       

        Vector3 pohyb = new Vector3(moveX, moveY, moveZ);
        Vector3 rotatedVector = Quaternion.AngleAxis(rotation, Vector3.up) * pohyb;
        pohyb = rotatedVector;

        position = position + pohyb * Time.deltaTime * 40;            

        rotation = rotation + rotate * Time.deltaTime;


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
        return  new Vector3((20.0f * moveX), 0, (20.0f * moveZ)); 
    }

    public override Vector3 GetCameraRotation()
    {
        return new Vector3(0, 0, 0);  //nestabilozovaná kamera
        //return -1 * GetPitchRoll(); //stabilozovaná kamera
    }
}
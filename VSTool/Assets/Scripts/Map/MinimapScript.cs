using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour
{
    public Transform player;

    private bool minusb = false;
    private bool plusb = false;
    
    void Update(){
        if(minusb){
            transform.position = new Vector3(transform.position.x ,transform.position.y + 0.5f,transform.position.z);
        }

        if(plusb){
            transform.position = new Vector3(transform.position.x ,transform.position.y - 0.5f,transform.position.z);
        }
    }

    public void minus(){
        transform.position = new Vector3(transform.position.x ,transform.position.y + 5,transform.position.z);
    }
    public void plus(){
        transform.position = new Vector3(transform.position.x ,transform.position.y - 5,transform.position.z);
    }

    public void minushold(){
        minusb = true;
    }

    public void plushold(){
        plusb =true;
    }

    public void minusholdrelease(){
        minusb = false;
    }

    public void plusholdrelease(){
        plusb =false;
    }
    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}

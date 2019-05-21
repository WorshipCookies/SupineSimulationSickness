using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusLimitScript : MonoBehaviour {

    public bool inWall;
    public bool activated;
    public Vector3 destinationPosition;
    public Transform destination;

    private void Update()
    {
        transform.position = destination.position;
        /*
        if (activated)
        {
            gameObject.SetActive(true);
        } else
        {
            gameObject.SetActive(false);
        }

        Debug.Log("Value : " + inWall);
        */
    }

    public void SetPosition(Vector3 position)
    {
        destinationPosition = new Vector3(position.x, position.y, position.z);
    }
    private void Start()
    {
        inWall = false;
    }

    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "internal wall" || other.gameObject.tag == "external wall")
        {
            inWall = true;
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "internal wall" || other.gameObject.tag == "external wall")
        {
            inWall = false;
        }
    }

    /*
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "ennemy"){
            inWall = true;
        } else if(other.gameObject.tag ==)
        {

        }
        else if (other.gameObject.tag == "internal wall" || other.gameObject.tag == "external wall" && other.gameObject.tag == "ground")
        {
            inWall = true;
            Debug.Log("Cursor in wall");
        } else if (other.gameObject.tag == "ground" && other.gameObject.tag != "internal wall" && other.gameObject.tag != "external wall")
        {
            inWall = false;
            Debug.Log("Cursor not in wall");
        }
    }*/
}

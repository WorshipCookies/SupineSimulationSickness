using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollideRadiusLimit : MonoBehaviour {

    private RadiusLimitScript RLS;

    private void Awake()
    {
        RLS = GameObject.Find("RadiusLimit").GetComponent<RadiusLimitScript>();
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("test");
        if (collision.gameObject.tag == "radiusLimit")
        {
            RLS.inWall = true;
            Debug.Log("In wall");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "radiusLimit")
        {
            RLS.inWall = false;
            Debug.Log("Not in wall");
        }
    }
}

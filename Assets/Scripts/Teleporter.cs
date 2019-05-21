using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {


    public GameObject TeleportMarker;
    public Transform Player;
    public float RayLength = 50f;


	// Use this for initialization
	void Start () {
		
	}

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            CastRay();
        }
        else
        {
            TeleportMarker.SetActive(false);
        }
    }
    void CastRay () {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, RayLength))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            if (hit.collider.tag == "ground")
            {
                Debug.Log("on the ground");
                TeleportMarker.SetActive(true);

                TeleportMarker.transform.position = hit.point + new Vector3(0,0.2f, 0);
            } else
            {
                TeleportMarker.SetActive(false);
            }
        } else
        {
            TeleportMarker.SetActive(false);
        }
	}
    /*
    private void OnFailedToConnectToMasterServer(NetworkConnectionError error)
    {
        if (TeleportMarker.activeSelf)
        {
            Vector3 markerPosition = TeleportMarker.transform.position;
            Player.position = new Vector3(markerPosition.x, Player.position.y, markerPosition.z);
        }
        
    }*/
}

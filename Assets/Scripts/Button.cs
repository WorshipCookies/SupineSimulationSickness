using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    GameManagerScript GMS;

    private void Awake()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            //Debug.Log("buttonTouched");
            if (Input.GetKeyDown(KeyCode.Return) || OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)) // button on right touch
            {
                //Debug.Log("buttonpressed when In");
                //GMS.SetIsButtonPressed(true);
                //run timer
                //start count down
                //GMS.CoolDownInvisibleWallStart();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCaptor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            GameObject.Find("SpeedCalculator").GetComponent<SpeedCalculator>().startTime = Time.time;
        }
    }
}

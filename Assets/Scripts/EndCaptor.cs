using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCaptor : MonoBehaviour {

    SpeedCalculator speedCalc;

	// Use this for initialization
	void Start () {
        speedCalc = GameObject.Find("SpeedCalculator").GetComponent<SpeedCalculator>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            speedCalc.endTime = Time.time;
            speedCalc.ComputeSpeed();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCalculator : MonoBehaviour {
    public float startTime;
    public float endTime;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ComputeSpeed()
    {
        float interval = endTime - startTime;
        float speed = 1.0f / interval;
        Debug.Log("Speed " + speed);
    }
}

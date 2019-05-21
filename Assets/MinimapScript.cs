using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour {

    public Transform player;
    private bool sticked;

	// Use this for initialization
	void Start () {
        sticked = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /*
    private void LateUpdate()
    {
        if (!sticked) { 
            Vector3 newPosition = player.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
            transform.rotation = Quaternion.Euler(90f, player.localEulerAngles.y, 0f);

        }
    }*/

    public void UpdateMinimapPosition()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(90f, player.localEulerAngles.y, 0f);
    }
}

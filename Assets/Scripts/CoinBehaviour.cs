using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour {

    public float rotateSpeed = 5f;
    GameManagerScript GMS;
    OpenvibeEventNotifier oen;

    private void Awake()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        oen = GameObject.Find("GlobalControl").GetComponent<OpenvibeEventNotifier>();
 
        GMS.coinsCounter++;
    }

    // Use this for initialization
    void Start () {
        transform.GetChild(1).gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        IsLastCoin();
        transform.Rotate(Vector3.forward * rotateSpeed);
        transform.GetChild(1).Rotate(Vector3.back * rotateSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            //Debug.Log("Coin collected");
            oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.PLAYER_COIN);
            GMS.AddCollectedCoin(gameObject);
            gameObject.SetActive(false);
            //Destroy(gameObject);
            GMS.coinsCounter--;
            //adding score...
        }
    }

    private void IsLastCoin()
    {
        if(GMS.coinsCounter == 1)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}

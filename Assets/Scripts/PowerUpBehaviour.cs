using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehaviour : MonoBehaviour
{


    GameManagerScript GMS;
    OpenvibeEventNotifier oen;

    private void Awake()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        oen = GameObject.Find("GlobalControl").GetComponent<OpenvibeEventNotifier>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.PLAYER_POWERUP);
            Destroy(gameObject);
            GMS.ennemyChase = false;
            //startTimer
            GMS.CoolDownPowerUpStart();
        }
    }
}

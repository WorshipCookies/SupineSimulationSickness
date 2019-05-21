using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {

    public int levelToLoad;
    EnemyNavigation EN;
    OpenvibeEventNotifier oen;
    ExperimentManager em;

    private void Awake()
    {
        EN = GameObject.Find("Enemy").GetComponent<EnemyNavigation>();
        oen = GameObject.Find("GlobalControl").GetComponent<OpenvibeEventNotifier>();
        em = GameObject.Find("GameManager").GetComponent<ExperimentManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "player")
        {
            if (em.isTraining)
            {
                //training mode
                em.ApplyBlackScreen();

            } else
            {
                //playing mode
                oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.PLAYER_PORTAL);
                EN.SaveAndIncrementEnemySpeed();

                SceneManager.LoadScene(levelToLoad);
            }

        }
    }

    //for testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            EN.SaveAndIncrementEnemySpeed();
            SceneManager.LoadScene(levelToLoad);
        }
    }
}

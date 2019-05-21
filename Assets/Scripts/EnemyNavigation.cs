using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour {

    public NavMeshAgent navMeshAgent;
    private Transform player;
    public GameObject EnnemyRepopArea;
    public GameObject EnnemyRunningWayAreas;
    public Material DeadMaterial;
    public Material RunningAwayMaterial;
    private Material InitialMaterial;

    public Material MinimapDeadMaterial;
    public Material MinimapRunningAwayMaterial;
    private Material MinimapInitialMaterial;

    public bool trainingBehaviour;

    private Vector3 destination;
    private bool movingFrom;
    public Transform[] nodes;

    public float enemySpeedAugmentation = 0.1f;

    GameManagerScript GMS;
    OpenvibeEventNotifier oen;

    private bool alive;

    private void Awake()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        oen = GameObject.Find("GlobalControl").GetComponent<OpenvibeEventNotifier>();
    }
        // Use this for initialization
    void Start () {
        navMeshAgent = GetComponent<NavMeshAgent>();

        InitialMaterial = transform.GetComponent<Renderer>().material;
        MinimapInitialMaterial = transform.GetChild(0).GetComponent<Renderer>().material;
        player = GameObject.FindWithTag("player").transform;
        alive = true;
        movingFrom = false;
        nodes = EnnemyRunningWayAreas.GetComponentsInChildren<Transform>();
        //gameObject.SetActive(false);

        LoadEnemySpeed();
    }
	
	// Update is called once per frame
	void Update () {
        CheckRespawn();
        UpdateMovementBehaviour();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            if (GMS.ennemyChase)
            {
                //ennemy kills player
                oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.ENEMY_TOUCH);
                KillPlayer();
                
            }
            else
            {
                //player kills ennemy
                oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.PLAYER_TOUCH);
                //gameObject.collider
                alive = false;
                transform.GetComponent<Renderer>().sharedMaterial = DeadMaterial;
                transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = MinimapDeadMaterial;

            }
        }
    }
    private void UpdateMovementBehaviour()
    {
        if (alive)
        {

            if (GMS.ennemyChase)
            {
                transform.GetComponent<Renderer>().sharedMaterial = InitialMaterial;
                transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = MinimapInitialMaterial;

                if (trainingBehaviour)
                {
                    //simple loop path
                    destination = SimpleLoopPath();
                } else
                {
                    //chase player
                    destination = player.position;
                }
                
            }
            else
            {
                transform.GetComponent<Renderer>().sharedMaterial = RunningAwayMaterial;
                transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = MinimapRunningAwayMaterial;
                //run away from player
                if (trainingBehaviour)
                {
                    destination = SimpleLoopPath();
                } else
                {
                    destination = RunAwayDestination();
                }
            }
        }
        else
        {
            //go to repop area
            destination = EnnemyRepopArea.transform.position;
        }

            navMeshAgent.SetDestination(destination);
    }

    private void CheckRespawn()
    {
        if (!alive && transform.position.x == EnnemyRepopArea.transform.position.x && transform.position.z == EnnemyRepopArea.transform.position.z)
        {
            Debug.Log("Arrived at respawn");
            alive = true;
            transform.GetComponent<Renderer>().sharedMaterial = InitialMaterial;
        }
        //for Training
        if (transform.position.x == nodes[1].position.x && transform.position.z == nodes[1].position.z)
        {
            movingFrom = false;
            Debug.Log("Arrived at : " + nodes[1].name + " Now movingFrom = " + movingFrom);
            
        } else if (transform.position.x == nodes[2].position.x && transform.position.z == nodes[2].position.z){
            movingFrom = true;
            Debug.Log("Arrived at : " + nodes[2].name + "Now movingFrom = " + movingFrom);
        }
    }

    private Vector3 RunAwayDestination()
    {
        Vector3 farthestArea = new Vector3();
        float maxDistance = -Mathf.Infinity;

        foreach (Transform child in EnnemyRunningWayAreas.GetComponentInChildren<Transform>())
        {
            if(Vector3.Distance(player.position, child.position) > maxDistance)
            {
                maxDistance = Vector3.Distance(player.position, child.position);
                farthestArea = child.position;
            }    
        }
        return farthestArea;
    }

    private void KillPlayer()
    {
        //player respawn
        GMS.RespawnPlayer();

        //ennemy reset
        ResetEnnemy();

        //lose half coins
        GMS.DropCoins(2);
    }

    private void ResetEnnemy()
    {
        navMeshAgent.enabled = false;
        transform.position = new Vector3(EnnemyRepopArea.transform.position.x, transform.position.y, EnnemyRepopArea.transform.position.z);
        navMeshAgent.enabled = true;
    }

    private Vector3 SimpleLoopPath()
    {
        Vector3 retDest = new Vector3();

        if (movingFrom)
        {
            retDest = nodes[1].position;
        } else
        {
            retDest = nodes[2].position;
        }
        
        return retDest;
    }
    public void ActivateEnemy()
    {
        gameObject.SetActive(true);
    }

    //save data to global control
    public void SaveAndIncrementEnemySpeed()
    {
        GlobalControl.Instance.enemySpeed = navMeshAgent.speed + enemySpeedAugmentation;
    }

    private void LoadEnemySpeed()
    {
        navMeshAgent.speed = GlobalControl.Instance.enemySpeed;
    }
}

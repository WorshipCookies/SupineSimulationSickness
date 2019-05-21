using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {

    public bool isTraining;
    //public Text coinsLeft;
    public int coinsCounter = 0;
    private int maxCoins = 0;
    private int collectedCoins;
    //private bool isButtonPressed;
    //private bool invisibleWalls;
    public GameObject Walls;
    public Material Glass;
    private Material InitialMaterial;
    //public OpenvibeEventNotifier oen;

    public float invisibleWallsMaxTimeDuration = 5;
    //public Text timerText;
    private float invisibleWallsStartingTimer;

    public float desactivatedButtonMaxTimeDuration = 15;
    private float desactivatedButtonStartingTimer;
    //public GameObject Button;

    public GameObject FinishPortal;
    public GameObject Player;
    public GameObject PlayerRespawnArea;

    public List<GameObject> collectedCoinsList;

    public float powerUpDuration = 8;
    private float powerUpStartingTimer;
    public bool ennemyChase;

    public Text UI;
    private float gameOverStartingTimer;
    public float gameOverDuration = 3;
    private Quaternion initialPlayerRotation;
    public GameObject MinimapEnvironment;

    private float minimapStartingCooldown;
    public float minimapCooldownDuration = 5;
    private bool isMinimapAvailable;
    private float minimapCooldownTimer;

    // Use this for initialization
    void Start () {
        collectedCoinsList = new List<GameObject>();
        //InitialMaterial = Walls.GetComponentInChildren<Transform>().GetChild(0).GetComponent<Renderer>().material;
        //isButtonPressed = false;
        //invisibleWalls = false;
        ennemyChase = true;
        maxCoins = coinsCounter;
        FinishPortal.SetActive(false);
        initialPlayerRotation = Player.transform.rotation;
        MinimapEnvironment.SetActive(true);
        isMinimapAvailable = true;

        if (!isTraining)
        {
            GameObject.Find("GlobalControl").GetComponent<CSVSave>().SetTransformToRecord();
        }
    }
	
	// Update is called once per frame
	void Update () {
        collectedCoins = maxCoins - coinsCounter;

        /*
        if (isButtonPressed)
        {
            MakeWallsInvisible();
            InvisibleWallsTimerUpdate();

        }
        else
        {
            timerText.text = "";
            MakeWallsVisible();
        }
        
        //ButtonTimerUpdate();
        */
        PowerUpUpdate();
        GameOverTimerManager();
        UpdateMinimapCooldown();


        UpdatePortal();
    }

    public void UpdatePortal()
    {
        if(coinsCounter > 0)
        {
            FinishPortal.SetActive(false);
            //coinsLeft.text = "Coins Left: " + coins_counter.ToString("D3") + "/" + max_coins.ToString("D3");
        }
        else if(coinsCounter <= 0)
        {
            //go to portal to win
            FinishPortal.SetActive(true);
        }
    }

    /*
    private void MakeWallsInvisible()
    {
        foreach(Transform child in Walls.GetComponentInChildren<Transform>())
        {
            child.GetComponent<Renderer>().sharedMaterial = Glass;
        }
    }

    private void MakeWallsVisible()
    {
        foreach (Transform child in Walls.GetComponentInChildren<Transform>())
        {
            child.GetComponent<Renderer>().sharedMaterial = InitialMaterial;
        }
    }
    private void InvisibleWallsTimerUpdate()
    {
        float remainingTime = invisibleWallsMaxTimeDuration - (Time.time - invisibleWallsStartingTimer);

        string remainingSeconds = (remainingTime % 60).ToString("f0");

        
        if (remainingTime <= 0)
        {
            MakeWallsVisible();
            isButtonPressed = false;
            
            remainingSeconds = "";

            //CoolDownButtonStart();
        }


        //timerText.text = remainingSeconds;
    }

    public void CoolDownButtonStart()
    {
        desactivatedButtonStartingTimer = Time.time;
        Button.SetActive(false);
    }

    public void CoolDownInvisibleWallStart()
    {
        invisibleWallsStartingTimer = Time.time;
    }


    private void ButtonTimerUpdate()
    {
        float remainingTime = desactivatedButtonMaxTimeDuration - (Time.time - desactivatedButtonStartingTimer);

        if(remainingTime <= 0)
        {
            Button.SetActive(true);
        }
    }
    */

    private void PowerUpUpdate()
    {
        float remainingTime = powerUpDuration - (Time.time - powerUpStartingTimer);

        if (remainingTime <= 0)
        {
            ennemyChase = true;
        }
    }
    public void CoolDownPowerUpStart()
    {
        //Debug.Log("Power up timer started");
        powerUpStartingTimer = Time.time;
    }
    /*
    public bool GetIsButtonPressed()
    {
        return isButtonPressed;
    }
    public void SetIsButtonPressed(bool isButtonPressed)
    {
        this.isButtonPressed = isButtonPressed;
    }
    */
    public void RespawnPlayer()
    {
        PrintText("You have been touched by the enemy !\nYou lose half of your coins and restart the game");

        Player.transform.position = new Vector3(PlayerRespawnArea.transform.position.x, Player.transform.position.y, PlayerRespawnArea.transform.position.z);
        Player.transform.rotation = initialPlayerRotation;

        //oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.BEGIN_BODY_FIRST);
    }

    public void PrintText(string text)
    {
        gameOverStartingTimer = Time.time;
        UI.text = text;
    }

    public void DropCoins(int division)
    {
        int nbCoinsToDrop =  (int) Mathf.Floor(collectedCoins / division);
        Debug.Log("Dropping coins : " + nbCoinsToDrop);

        if(nbCoinsToDrop > 0)
        {
            Debug.Log("Collected coins nb: " + (collectedCoinsList.Count - 1));
            int size = collectedCoinsList.Count;
            for (int i = size - nbCoinsToDrop; i < size; i++)
            {
                Debug.Log("index : " + i);
                collectedCoinsList[i].SetActive(true);
                coinsCounter++;
            }
            collectedCoinsList.RemoveRange(size - nbCoinsToDrop, nbCoinsToDrop);
        }
    }

    public void AddCollectedCoin(GameObject coin)
    {
        collectedCoinsList.Add(coin);
    }

    private void GameOverTimerManager()
    {
        
        if(gameOverDuration - (Time.time - gameOverStartingTimer) <= 0)
        {
            UI.text = "";
        }
    }

    public void StartMinimapCooldown()
    {
        isMinimapAvailable = false;
        minimapStartingCooldown = Time.time;
    }

    private void UpdateMinimapCooldown()
    {
        minimapCooldownTimer = minimapCooldownDuration - (Time.time - minimapStartingCooldown);
        if (minimapCooldownTimer <= 0)
        {
            isMinimapAvailable = true;
        }
    }

    public bool GetIsMinimapAvailable()
    {
        return isMinimapAvailable;
    }
    public float GetMinimapCooldownTimer()
    {
        return minimapCooldownTimer;
    }

    /*public void OnApplicationQuit()
    {
        oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.END_EXPERIMENT);
    }*/
}

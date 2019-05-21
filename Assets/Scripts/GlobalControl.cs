using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;

    public float gamePhaseDuration = 360;
    private float gamePhaseTimer;
    private float gamePhaseStartTime;

    public float enemySpeed;

    public bool experimentAlreadyRunning = false;

    public String pos;
    public String fov;
    public String rest;
    public String nav;

    private bool gameTimerStarted = false;

    //for frameRate
    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    float m_lastFramerate = 0.0f;
    public float m_refreshTime = 0.5f; //time intervale for calculating average FPS

    //EnemyNavigation EN;

    void Awake()
    {
        //EN = GameObject.Find("Enemy").GetComponent<EnemyNavigation>();


        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartGamePhaseTimer()
    {
        gameTimerStarted = true;
        gamePhaseStartTime = Time.time;
    }

    public void UpdateGameTimer()
    {
        ComputeFPS();

        if (gameTimerStarted)
        {
            gamePhaseTimer = gamePhaseDuration - (Time.time - gamePhaseStartTime);

            
            //Debug.Log("Game Timer: " + (gamePhaseTimer));
            if (gamePhaseTimer <= 0)
            {
                GameObject.Find("GameManager").GetComponent<ExperimentManager>().EndGamePhase();
            }
            else if (gamePhaseTimer <= 6)
            {
                GameObject.Find("GameManager").GetComponent<GameManagerScript>().PrintText("Game is ending soon!");
            }
        }
    }
    void OnGUI()
    {
        GUILayout.TextArea("Game Timer: " + gamePhaseTimer + "\n" + "FPS: " + m_lastFramerate);
    }

    private void ComputeFPS()
    {
        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            m_lastFramerate = (float)m_frameCounter / m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }
    }
}

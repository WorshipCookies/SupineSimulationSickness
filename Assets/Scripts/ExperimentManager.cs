using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using Sigtrap.ImageEffects;
using UnityEngine.AI;

public class ExperimentManager : MonoBehaviour {

    public bool useConfigFile = true;

    public String nameFile = "config.csv";
    StreamReader inputStream;
    public int subjectId = 1; //subject Id
    public int session = 1;
    public int phase = 1;
    private String delimiter = ";";

    private String pos;
    private String fov;
    private String rest;

    private String nav;

    GameObject CameraRig;
    GameObject CenterEye;
    GameObject GameManager;
    GameObject PlayerController;
    GameObject GlobalControlObj;

    GameObject Enemy;
    GameManagerScript GMS;
    GameObject RightController;

    [HideInInspector]
    public bool isTraining = false;


    private void Awake()
    {
        CameraRig = GameObject.Find("OVRCameraRig");
        CenterEye = GameObject.Find("CenterEyeAnchor");
        GameManager = GameObject.Find("GameManager");
        PlayerController = GameObject.Find("OVRPlayerController");
        Enemy = GameObject.Find("Enemy");
        GlobalControlObj = GameObject.Find("GlobalControl");
        GMS = GameManager.GetComponent<GameManagerScript>();
        RightController = GameObject.Find("RightController");
    }

    // Use this for initialization
    void Start () {
        
        GMS.isTraining = isTraining;
        if (!isTraining)
        {
            if (!GlobalControl.Instance.experimentAlreadyRunning)
            {
                GlobalControlObj.GetComponent<CSVSave>().enabled = false;
                if (useConfigFile)
                {
                    LoadCSVAndSetExperiment();
                } else
                {
                    //seated
                    //SetExperimentManually("seated","default", "default", "default");
                    //lying down
                    SetExperimentManually("lying", "default", "default", "default");
                }

                ApplyBlackScreen();
                DisablePlayerMovement();
                DesactivateEnemy();

                GlobalControl.Instance.experimentAlreadyRunning = true;
            } else
            {
                //new level
                SetExperiment(GlobalControl.Instance.pos, GlobalControl.Instance.fov, GlobalControl.Instance.rest, GlobalControl.Instance.nav);

                RemoveBlackScreen();
            }
        } else //training
        {
            LoadCSVAndSetExperiment();
            ApplyBlackScreen();
            DisablePlayerMovement();
            DesactivateEnemy();
        }
    }
	
	// Update is called once per frame
	void Update () {

        //Start experiment and Remove black screen
        if (Input.GetKeyDown(KeyCode.B) || (!useConfigFile && OVRInput.Get(OVRInput.Button.One)))
        {
            RemoveBlackScreen();
            EnablePlayerMovement();
            ActivateEnemy();

            if (!isTraining)
            { 
                StartGamePhase();
            }
        }
        //start calibration phase
        else if(Input.GetKeyDown(KeyCode.C))
        {
            RemoveBlackScreen();
            GlobalControlObj.GetComponent<OpenvibeEventNotifier>().StartGSR();

        }
        //end calibration phase
        else if (Input.GetKeyDown(KeyCode.V))
        {
            EnablePlayerMovement();
            ActivateEnemy();

            if (!isTraining)
            {
                StartGamePhase();
            }
        }

        if (!isTraining)
        {
            GlobalControl.Instance.UpdateGameTimer();
        }

    }

    private String getPath()
    {
        return Application.dataPath + "/CSV/" + nameFile;
    }

    private void LoadCSVAndSetExperiment()
    {
        String filePath = getPath();
        StreamReader inputStream = System.IO.File.OpenText(filePath);
        String data = inputStream.ReadToEnd();
        String[] loadedLines = data.Split("\n"[0]);

        String[] rowDataTemp = new String[6];

        int i = 1;
        do
        {
            rowDataTemp = loadedLines[i].Split(delimiter[0]);
            i++;

        } while (!(int.Parse(rowDataTemp[0]) == subjectId && int.Parse(rowDataTemp[1]) == session && int.Parse(rowDataTemp[2]) == phase));
        //now we have correct line

        pos = rowDataTemp[3];
        fov = rowDataTemp[4];
        rest = rowDataTemp[5];
        nav = rowDataTemp[6].Trim();

        GlobalControl.Instance.pos = pos;
        GlobalControl.Instance.fov = fov;
        GlobalControl.Instance.rest = rest;
        GlobalControl.Instance.nav = nav;

        SetExperiment(pos, fov, rest, nav);
    }

    private void SetExperimentManually(String posP, String fovP, String restP, String navP)
    {
        GlobalControl.Instance.pos = posP;
        GlobalControl.Instance.fov = fovP;
        GlobalControl.Instance.rest = restP;
        GlobalControl.Instance.nav = navP;

        SetExperiment(posP, fovP, restP, navP);
    }

        private void SetExperiment(String posP, String fovP, String restP, String navP)
    {
        switch (posP)
        {
            case "seated":
                CameraRig.GetComponent<OVRCameraRig>().lyingPosWithoutTransition = false;
                CameraRig.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                CameraRig.GetComponent<OVRCameraRig>().InitCameraOrientation();
                break;
            case "lying":
                CameraRig.GetComponent<OVRCameraRig>().lyingPosWithoutTransition = true;
                CameraRig.GetComponent<OVRCameraRig>().InitCameraOrientation();
                break;
        }

        switch (fovP)
        {
            case "reduced":
                CenterEye.GetComponent<CustomBlur>().enabled = false;
                CenterEye.GetComponent<Tunnelling>().enabled = true;
                break;
            case "blurred":
                CenterEye.GetComponent<CustomBlur>().enabled = true;
                CenterEye.GetComponent<Tunnelling>().enabled = false;
                break;
            case "default":
                CenterEye.GetComponent<CustomBlur>().enabled = false;
                CenterEye.GetComponent<Tunnelling>().enabled = false;
                break;
        }

        switch (restP)
        {
            case "default":
                GameManager.GetComponent<SphereMaker>().enabled = false;
                break;
            case "textured":
                GameManager.GetComponent<SphereMaker>().enabled = true;
                break;
        }

        switch (navP)
        {
            case "default":
                RightController.GetComponent<VRTK.VRTK_StraightPointerRenderer>().enabled = false;
                break;
            case "extended":
                RightController.GetComponent<VRTK.VRTK_StraightPointerRenderer>().enabled = true;
                break;
        }


        Debug.Log("Experiment Configuration done");
        Debug.Log("Subject : " + subjectId + ", Session : " + session + ", Phase : " + phase + ", Position : " + posP + ", FOV : " + fovP + ", Rest Frame : " + restP + ", Navigation : " + navP);
    }

    public String GetConfigString()
    {
        return subjectId.ToString() + "-" + session.ToString() + "-" + phase.ToString() + "-" + pos.ToString() + "-" + fov.ToString() + "-" + rest.ToString() + "-" + nav.ToString();
    }

    public void ApplyBlackScreen()
    {
        CenterEye.GetComponent<BlackScreen>().enabled = true;
    }

    private void RemoveBlackScreen()
    {
        CenterEye.GetComponent<BlackScreen>().enabled = false;
        CenterEye.GetComponent<MeshRenderer>().enabled = false;
    }

    private void DisablePlayerMovement()
    {
        PlayerController.GetComponent<OVRPlayerController>().enabled = false;
    }

    private void EnablePlayerMovement()
    {
        PlayerController.GetComponent<OVRPlayerController>().enabled = true;
    }



    public void StartGamePhase()
    {
        Debug.Log("Starting Game Phase");
        GMS.PrintText("Start Game!");
        if (useConfigFile)
        {
            GlobalControlObj.GetComponent<CSVSave>().enabled = true;
        }
        GlobalControl.Instance.StartGamePhaseTimer();
        GlobalControlObj.GetComponent<OpenvibeEventNotifier>().NotifyEvent(OpenvibeEventNotifier.EventTypes.START_EXPERIMENT);
    }
    public void EndGamePhase()
    {
        Debug.Log("End Game Phase");
        if (useConfigFile)
        {
            GlobalControlObj.GetComponent<CSVSave>().FinishRecording();
        }
        GlobalControlObj.GetComponent<OpenvibeEventNotifier>().EndGSR();
        ApplyBlackScreen();
    }

    private void ActivateEnemy()
    {
        Enemy.GetComponent<EnemyNavigation>().enabled = true;
        Enemy.GetComponent<NavMeshAgent>().enabled = true;
    }
    private void DesactivateEnemy()
    {
        Enemy.GetComponent<EnemyNavigation>().enabled = false;
        Enemy.GetComponent<NavMeshAgent>().enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class CSVSave : MonoBehaviour {

    private List<String[]> rowData = new List<String[]>();
    public Transform Player;
    public Transform PlayerHead;
    public Transform Enemy;

    public String FOVReduc;
    public String RestFrame;

    public bool replay;

    private int indexReader = 1;
    private String[] loadedLines;
    private String[] currentRow = new String[7];

    public float currentTime;
    public Vector3 currentPlayerPosition;
    public Quaternion currentPlayerRotation;
    public Quaternion currentPlayerHeadRotation;
    public Vector3 currentEnemyPosition;
    public Quaternion currentEnemyRotation;
    public int currentActivatedMinimap;

    private bool activatedMinimap;

    private String delimiter = ";";
    int deltaFrame = 0;
    private bool fileAlreadyCreated;

    String line;
    StreamReader inputStream;

    ExperimentManager em;

    public static CSVSave Instance;

    private float startingTime;


    private void Awake()
    {
        em = GameObject.Find("GameManager").GetComponent<ExperimentManager>();

        SetTransformToRecord();
        /*
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }*/
    }


    // Use this for initialization
    void Start () {
        startingTime = Time.time;

        Debug.Log("Script CSV started");
        fileAlreadyCreated = false;
        if (!replay)
        {
            InitTitles();
        } else
        {
            //LoadCSV();
            OpenCSV();
        }  
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!replay)
        {
            AddRow(CheckMinimapButton());
            if (rowData.Count >= 100)
            {
                WriteToCSV();
                rowData.Clear();
                Debug.Log("Buffer saved and reset");
            }

        } else
        {

            //ReadLine();
            Read();
            ApplyMovement();

        }
    }

    void InitTitles()
    {
        String[] rowDataTemp = new String[8];
        rowDataTemp[0] = "Time";
        rowDataTemp[1] = "PlayerPosition";
        rowDataTemp[2] = "PlayerRotation";
        rowDataTemp[3] = "PlayerHeadRotation";
        rowDataTemp[4] = "EnemyPosition";
        rowDataTemp[5] = "EnemyRotation";
        rowDataTemp[6] = "MinimapButton";
        rowDataTemp[7] = "Info";

        rowData.Add(rowDataTemp);
    }

    void AddRow(int activated)
    {
        String[] rowDataTemp = new String[8];

        rowDataTemp[0] = (Time.time - startingTime) + "";
        rowDataTemp[1] = Player.position.ToString("F4");
        rowDataTemp[2] = Player.rotation.ToString("F4");
        rowDataTemp[3] = PlayerHead.rotation.ToString("F4");
        rowDataTemp[4] = Enemy.position.ToString("F4");
        rowDataTemp[5] = Enemy.rotation.ToString("F4");
        rowDataTemp[6] = activated + "";
        rowDataTemp[7] = "test";
        rowData.Add(rowDataTemp);
    }
    /*
    private void OnApplicationQuit()
    {
        if (!replay)
        {
            WriteToCSV();
        }
        oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.END_EXPERIMENT);
    }
    */
    public void FinishRecording()
    {
        if (!replay)
        {
            WriteToCSV();
        }
        //oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.END_EXPERIMENT);
    }

    private void WriteToCSV()
    {
        String[][] output = new String[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(String.Join(delimiter, output[index]));


        String filePath = getPath();

        StreamWriter outStream;

        //check if file already created
        if (fileAlreadyCreated)
        {
            outStream = System.IO.File.AppendText(filePath);
        } else
        {
            outStream = System.IO.File.CreateText(filePath);
            fileAlreadyCreated = true;
        }
        
        outStream.WriteLine(sb);
        outStream.Close();
    }

    private String getPath()
    {
        #if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + em.GetConfigString() + "-movement.csv";
        #else
        return Application.dataPath +"/"+"Saved_data.csv";
        #endif
    }

    private void LoadCSV()
    {
        String filePath = getPath();
        StreamReader inputStream = System.IO.File.OpenText(filePath);
        String data = inputStream.ReadToEnd();
        line = inputStream.ReadLine();
        loadedLines = data.Split("\n"[0]);
    }

    private void OpenCSV()
    {
        String filePath = getPath();
        inputStream = System.IO.File.OpenText(filePath);
    }

    private void Read()
    {
        line = inputStream.ReadLine();
        String[] rowDataTemp = new String[7];
        rowDataTemp = line.Split(delimiter[0]);

        float f;
        if (rowDataTemp != null && float.TryParse(rowDataTemp[0], out f))
        {

            currentTime = float.Parse(rowDataTemp[0]);
            currentPlayerPosition = StringToVector3(rowDataTemp[1]);
            currentPlayerRotation = StringToQuaternion(rowDataTemp[2]);
            currentPlayerHeadRotation = StringToQuaternion(rowDataTemp[3]);
            currentEnemyPosition = StringToVector3(rowDataTemp[4]);
            currentEnemyRotation = StringToQuaternion(rowDataTemp[5]);
            currentActivatedMinimap = int.Parse(rowDataTemp[6]);

            Debug.Log("Real time : " + Time.time + " / recordedTime : " + currentTime);
            indexReader++;
        }
        else
        {
            Debug.Log("End of File");
        }
    }


    private void ReadLine()
    {
        String[] rowDataTemp = new String[6];
        //rowDataTemp = loadedLines[indexReader].Split(delimiter[0]);

        rowDataTemp = line.Split(delimiter[0]);

        if(rowDataTemp != null)
        {

            currentTime = float.Parse(rowDataTemp[0]);
            currentPlayerPosition = StringToVector3(rowDataTemp[1]);
            currentPlayerRotation = StringToQuaternion(rowDataTemp[2]);
            currentPlayerHeadRotation = StringToQuaternion(rowDataTemp[3]);
            currentEnemyPosition = StringToVector3(rowDataTemp[4]);
            currentEnemyRotation = StringToQuaternion(rowDataTemp[5]);

            Debug.Log("Real time : " + Time.time + " / recordedTime : " + currentTime);
            indexReader++;
        } else
        {
            Debug.Log("End of File");
        }
    }

    private static Vector3 StringToVector3(String sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        String[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    private static Quaternion StringToQuaternion(String sQuaternion)
    {
        // Remove the parentheses
        if (sQuaternion.StartsWith("(") && sQuaternion.EndsWith(")"))
        {
            sQuaternion = sQuaternion.Substring(1, sQuaternion.Length - 2);
        }

        // split the items
        String[] sArray = sQuaternion.Split(',');

        // store as a Quaternion
        Quaternion result = new Quaternion(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            float.Parse(sArray[3]));

        return result;
    }

    private void ApplyMovement()
    {
        Player.position = currentPlayerPosition;
        Player.rotation = currentPlayerRotation;
        PlayerHead.rotation = currentPlayerHeadRotation;
        Enemy.position = currentEnemyPosition;
        Enemy.rotation = currentEnemyRotation;

        if(currentActivatedMinimap == 1)
        {
            //activate minimap
            GameObject.Find("Projector").GetComponent<RayCastMinimap>().ActivateMinimapReplay();
        }

    }

    public void SetTransformToRecord()
    {
        Player = GameObject.Find("OVRPlayerController").transform;
        PlayerHead = GameObject.Find("CenterEyeAnchor").transform;
        Enemy = GameObject.Find("Enemy").transform;
    }
    private int CheckMinimapButton()
    {
        int ret = 0;

        if(activatedMinimap)
        {
            ret = 1;
            activatedMinimap = false;
        }
        return ret;
    }

    public void SetActivatedMinimap(bool b)
    {
        activatedMinimap = b;
    }

    public bool GetReplay()
    {
        return replay;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastMinimap : MonoBehaviour {

    public GameObject mapPlane1;
    private GameObject mapPlane2;
    public GameObject cooldownPlane1;
    private GameObject cooldownPlane2;

    private LayerMask layerMask;
    private Transform[] raySpots;
    private bool stickMinimap;
    private string cooldownText;
    private TextMesh textCooldown1;
    private TextMesh textCooldown2;

    MinimapScript MinimapScript;

    GameManagerScript GMS;

    OpenvibeEventNotifier oen;

    private void Awake()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        MinimapScript = GameObject.Find("MinimapCamera").GetComponent<MinimapScript>();
        oen = GameObject.Find("GlobalControl").GetComponent<OpenvibeEventNotifier>();

    }

    // Use this for initialization
    void Start () {
        mapPlane2 = Instantiate<GameObject>(mapPlane1);
        cooldownPlane2 = Instantiate<GameObject>(cooldownPlane1);

        raySpots = transform.GetComponentsInChildren<Transform>();

        layerMask = LayerMask.NameToLayer("Wall");
        stickMinimap = false;
	}
	
	// Update is called once per frame
	void Update () {
        
        MapButtonListener();
        UpdateCooldownTimerText();
    }

    void Projection(bool projectMinimap)
    {
        RaycastHit hit = new RaycastHit();
        RaycastHit hit1 = new RaycastHit();
        RaycastHit hit2 = new RaycastHit();
        RaycastHit hit3 = new RaycastHit();
        RaycastHit hit4 = new RaycastHit();

        if (Physics.Raycast(raySpots[0].position, transform.forward , out hit, Mathf.Infinity, 1 << layerMask) &&
            Physics.Raycast(raySpots[1].position, transform.forward, out hit1, Mathf.Infinity, 1 << layerMask) &&
            Physics.Raycast(raySpots[2].position, transform.forward, out hit2, Mathf.Infinity, 1 << layerMask) &&
            Physics.Raycast(raySpots[3].position, transform.forward, out hit3, Mathf.Infinity, 1 << layerMask) &&
            Physics.Raycast(raySpots[4].position, transform.forward, out hit4, Mathf.Infinity, 1 << layerMask) &&
            IsOnSameWall(hit, hit1, hit2, hit3, hit4))
        {

            if (projectMinimap)
            {
                //proejct minimap
                oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.PLAYER_MINIMAP);

                if (!GMS.isTraining)
                {
                    GameObject.Find("GlobalControl").GetComponent<CSVSave>().SetActivatedMinimap(true);
                }

                SetActiveMinimap(true);

                SetPlanePosition(hit, mapPlane1, mapPlane2);

                MinimapScript.UpdateMinimapPosition();
                GMS.StartMinimapCooldown();
            } else
            {
                SetActiveCooldown(true);
                SetPlanePosition(hit, cooldownPlane1, cooldownPlane2);

            }
        }
    }
    private bool IsOnSameWall(RaycastHit hit, RaycastHit hit1, RaycastHit hit2, RaycastHit hit3, RaycastHit hit4)
    {
        int id = hit.transform.GetInstanceID();
        Vector3 normal = hit.normal; //check if on same face
        return (hit1.transform.GetInstanceID() == id  && hit2.transform.GetInstanceID() == id && 
            hit3.transform.GetInstanceID() == id && hit4.transform.GetInstanceID() == id &&
            hit1.normal == normal && hit2.normal == normal && hit3.normal == normal && hit4.normal == normal);
    }

    private void SetActiveMinimap(bool b)
    {
        mapPlane1.SetActive(b);
        mapPlane2.SetActive(b);
    }

    private void SetActiveCooldown(bool b)
    {
        cooldownPlane1.SetActive(b);
        cooldownPlane2.SetActive(b);
    }
    private void MapButtonListener()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Test run");
            if (GMS.GetIsMinimapAvailable())
            {
                Projection(GMS.GetIsMinimapAvailable());
            } else
            {
                //show cooldown
                Projection(GMS.GetIsMinimapAvailable());
            }
        }
    }

    private void SetPlanePosition(RaycastHit hit, GameObject plane1, GameObject plane2){
        plane1.transform.position = hit.point - Vector3.one * 0.01f;
        plane2.transform.position = hit.point + Vector3.one * 0.01f;
        plane1.transform.rotation = Quaternion.Euler(new Vector3(plane1.transform.eulerAngles.x, -hit.transform.eulerAngles.y, plane1.transform.eulerAngles.z));
        plane2.transform.forward = -plane1.transform.forward;
    }
    private void UpdateCooldownTimerText()
    {
        if (GMS.GetMinimapCooldownTimer() <= 0f)
        {
            SetActiveCooldown(false);
        } else
        {
            cooldownText = (GMS.GetMinimapCooldownTimer() % 60).ToString("f0");
            textCooldown1 = cooldownPlane1.transform.GetChild(0).GetComponent<TextMesh>();
            textCooldown2 = cooldownPlane2.transform.GetChild(0).GetComponent<TextMesh>();

            textCooldown1.text = cooldownText;
            textCooldown2.text = cooldownText;
        } 
    }

    public void ActivateMinimapReplay()
    {
        RaycastHit hit = new RaycastHit();

        Physics.Raycast(raySpots[0].position, transform.forward, out hit, Mathf.Infinity, 1 << layerMask);
        SetActiveMinimap(true);

        SetPlanePosition(hit, mapPlane1, mapPlane2);

        MinimapScript.UpdateMinimapPosition();
        GMS.StartMinimapCooldown();

        
    }
}

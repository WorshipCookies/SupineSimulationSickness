using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class OpenvibeEventNotifier : MonoBehaviour {
	/* WARNING: This enum's bytes must correspond to those in OpenVIBE's Lua script */
	public enum EventTypes:byte {
		START_EXPERIMENT = 	(byte)'A',


        ENEMY_TOUCH = (byte)'O',
        PLAYER_COIN = (byte)'P',
        PLAYER_POWERUP = (byte)'Q',
        PLAYER_TOUCH = (byte)'R',
        PLAYER_PORTAL = (byte)'S',
        PLAYER_MINIMAP = (byte)'T',
        PLAYER_TELEPORT = (byte)'U',

        END_EXPERIMENT =	0x00
		};

	private UdpClient client;
	private const int remotePort = 4242;//55056;
	private const string remoteIP = "localhost";
    //public static OpenvibeEventNotifier Instance;

    /*
	void Start () {
		client = new UdpClient (remoteIP,remotePort);
		if (null != client) {
			byte[] packet = new byte[]{0x00};
			client.Send (packet, 1);
		}
	}*/
    /*
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }*/

    public void StartGSR()
    {
        Debug.Log("StartGSR");
        client = new UdpClient(remoteIP, remotePort);
        if (null != client)
        {
            byte[] packet = new byte[] { 0x00 };
            client.Send(packet, 1);
        }
    }

	public void NotifyEvent(EventTypes typeEnum) {
		if (null != client) {
			byte[] type = new byte[]{(byte)typeEnum};
			client.Send (type, 1);
		}
	}
    public void EndGSR()
    {
        Debug.Log("EndGSR");
        if (null != client)
        {
            NotifyEvent(EventTypes.END_EXPERIMENT);
            client.Close();
        }
    }
    /*
	void OnDestroy() {
        Debug.Log("EndGSR before end experiment");
        if (null != client) {
			NotifyEvent (EventTypes.END_BLOCK);
			client.Close ();
		}
	}*/

}

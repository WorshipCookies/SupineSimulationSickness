using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MicControlC : MonoBehaviour {

	public enum micActivation {
		HoldToSpeak,
		PushToSpeak,
		ConstantSpeak
	}


	public bool virual3D = true;
	public float volumeFallOff = 0.3f;

	[HideInInspector]
	public float listenerDistance { get; private set; }
	[HideInInspector]
	public bool ableToHearMic = false;

	public float sensitivity = 100;
	[Range(0,100)]
	public float sourceVolume = 100;//Between 0 and 100
	[HideInInspector]
	public bool GuiSelectDevice = true;
	public micActivation micControl;
	//
	public string selectedDevice { get; private set; }
	public float loudness { get; private set; } //dont touch
	//
	private bool micSelected = false;
	private int amountSamples = 256; //increase to get better average, but will decrease performance. Best to leave it
	private int minFreq, maxFreq;
	
	private bool focused = true;
	
	private KeyCode holdToSpeakToggle = KeyCode.Keypad1;
	private OpenvibeEventNotifier oen;
	
	void Start() {
		//oen = transform.Find ("/controller").GetComponent<OpenvibeEventNotifier> ();
		GetComponent<AudioSource>().loop = true; // Set the AudioClip to loop
		GetComponent<AudioSource>().mute = false; // Mute the sound, we don't want the player to hear it

		if (Microphone.devices.Length > 0)
			selectedDevice = Microphone.devices [0].ToString ();
		else
			selectedDevice = "";

		micSelected = true;
		GetMicCaps();
		

		if(virual3D){
			GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Logarithmic;
			GetComponent<AudioSource>().minDistance = 1.0f;
			GetComponent<AudioSource>().maxDistance = 4.0f;
		}
	}
	void OnGUI() {
		MicDeviceGUI((Screen.width/2)-150, (Screen.height/2)-75, 300, 100, 10, -300);
	}
	public void MicDeviceGUI (float left, float top, float width, float height, float buttonSpaceTop, float buttonSpaceLeft) {
		if (Microphone.devices.Length > 1 && GuiSelectDevice == true || micSelected == false)//If there is more than one device, choose one.
			for (int i = 0; i < Microphone.devices.Length; ++i)
			if (GUI.Button(new Rect(left + ((width + buttonSpaceLeft) * i), top + ((height + buttonSpaceTop) * i), width, height), Microphone.devices[i].ToString())) {
				StopMicrophone();
				selectedDevice = Microphone.devices[i].ToString();
				GetMicCaps();
				StartMicrophone();
				micSelected = true;
			}
		if (Microphone.devices.Length == 1 && micSelected == false) {//If there is only 1 decive make it default
			selectedDevice = Microphone.devices[0].ToString();
			GetMicCaps();
			micSelected = true;
		}
	}
	public void GetMicCaps () {
		Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);//Gets the frequency of the device
		if ((minFreq + maxFreq) == 0)//These 2 lines of code are mainly for windows computers
			maxFreq = 44100;
	}
	public void StartMicrophone () {
		StartCoroutine (CoroutineStartMic ());
	}

	IEnumerator CoroutineStartMic () {
		//oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.MIC_ON);
		//null -> selectedDevice
		GetComponent<AudioSource>().clip = Microphone.Start(null, true, 10, maxFreq);//Starts recording
		while (!(Microphone.GetPosition(selectedDevice) > 0)){
			yield return new WaitForEndOfFrame();
		} // Wait until the recording has started
		GetComponent<AudioSource>().Play(); // Play the audio source!
		yield return 0;
	}

	public void StopMicrophone () {
		StartCoroutine (CoroutineStopMic ());
	}

	IEnumerator CoroutineStopMic () {
		//oen.NotifyEvent(OpenvibeEventNotifier.EventTypes.MIC_OFF);
		GetComponent<AudioSource>().Stop();//Stops the audio
		//null -> selectedDevice
		Microphone.End(null);//Stops the recording of the device
		yield return 0;
	}

	void Update() {
		if (!focused)
			StopMicrophone();
		
		if (!Application.isPlaying) {
			StopMicrophone();
		}
		
		if(virual3D){
			//listenerDistance = Vector3.Distance(hpg.GetHeadPos(), audio.transform.position);
			//audio.volume = (sourceVolume / 100 / (listenerDistance * volumeFallOff));
			GetComponent<AudioSource>().volume = (sourceVolume / 100);
			loudness = GetAveragedVolume() * sensitivity * (sourceVolume / 10);
		} else {
			GetComponent<AudioSource>().volume = (sourceVolume / 100);
			loudness = GetAveragedVolume() * sensitivity * (sourceVolume / 10);
		}
		//Hold To Speak!!
		if (micControl == micActivation.HoldToSpeak) {
			if (Microphone.IsRecording(selectedDevice) && Input.GetKey(holdToSpeakToggle) == false) {
				StopMicrophone();
			}

			if (Input.GetKeyDown(holdToSpeakToggle)) {
				StartMicrophone();
			}

			if (Input.GetKeyUp(holdToSpeakToggle)) {
				StopMicrophone();
			}
		}
		
		//Push To Talk!!
		if (micControl == micActivation.PushToSpeak) {
			if (Input.GetKeyDown(KeyCode.T)) {
				if (Microphone.IsRecording(selectedDevice))
					StopMicrophone();
				
				else if (!Microphone.IsRecording(selectedDevice))
					StartMicrophone();
			}
			//
		}
		
		//Constant Speak!!
		if (micControl == micActivation.ConstantSpeak)
			if (!Microphone.IsRecording(selectedDevice))
				StartMicrophone();
		
		//Mic Slected = False!!
		if (Input.GetKeyDown(KeyCode.G))
			micSelected = false;
	}
	
	float GetAveragedVolume() {
		float[] data = new float[amountSamples];
		float a = 0;
		GetComponent<AudioSource>().GetOutputData(data,0);
		foreach (float s in data) {
			a += Mathf.Abs(s);
		}
		return a/amountSamples;
	}
	
	void OnApplicationFocus(bool focus) {
		focused = focus;
	}
	
	void OnApplicationPause(bool focus) {
		focused = focus;
	}
}
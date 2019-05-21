using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;


public class FOVLimiter : MonoBehaviour {


    /// <summary>
    /// Below this angular velocity, effect will not kick in. Degrees per second
    /// </summary>
    [Tooltip("Below this angular velocity, effect will not kick in.\nDegrees per second")]
    public float minAngVel = 0f;

    /// <summary>
    /// At/above this angular velocity, effect will be maxed out. Degrees per second
    /// </summary>
    [Tooltip("At/above this angular velocity, effect will be maxed out.\nDegrees per second")]
    public float maxAngVel = 180f;

    /// <summary>
    /// Below this speed, effect will not kick in.
    /// </summary>
    [Tooltip("Below this speed, effect will not kick in.")]
    public float minSpeed = 0f;

    /// <summary>
    /// At/above this speed, effect will be maxed out.
    /// </summary>
    [Tooltip("At/above this speed, effect will be maxed out.\nSet negative for no effect.")]
    public float maxSpeed = -1f;

    [Header("Effect Settings")]
    /// <summary>
    /// Screen coverage at max angular velocity.
    /// </summary>
    [Range(0f, 1f)]
    [Tooltip("Screen coverage at max angular velocity.\n(1-this) is radius of visible area at max effect (screen space).")]
    public float maxEffect = 1;

    /// <summary>
    /// Smooth out radius over time. 0 for no smoothing.
    /// </summary>
    [Tooltip("Smooth out radius over time. 0 for no smoothing.")]
    public float smoothTime = 0.15f;

    private float _avSlew;
    private float _av;

    //private Vector3 oldPos;

    //public float MaxSpeed = 6f;
    //public float MaxFOV = .7f;

    private Transform refTransform;
    private Vector3 _lastFwd;
    private Vector3 _lastPos;

    public PostProcessingProfile ppProfile;

    //private VignetteAndChromaticAberration fovLimiter;

    private VignetteModel.Settings vignetteSettings;

    private void Awake()
    {
        refTransform = transform;
    }

    // Use this for initialization
    void Start () {
        //oldPos = transform.position;
        vignetteSettings = ppProfile.vignette.settings;
        //fovLimiter = GetComponent<VignetteAndChromaticAberration>();
        Debug.Log("Starting intensity: " + vignetteSettings.intensity);
	}
	
	// Update is called once per frame
	void Update () {
        /*
        Vector3 velocity = (transform.position - oldPos / Time.deltaTime);
        oldPos = transform.position;

        float expectedLimit = MaxFOV;
        if(velocity.magnitude < MaxSpeed)
        {
            expectedLimit = (velocity.magnitude / MaxSpeed) * MaxFOV;
        }

        //fovLimiter.intensity = Mathf.Lerp(fovLimiter.intensity, expectedLimit, 0.01f);
        vignetteSettings.intensity = Mathf.Lerp(vignetteSettings.intensity, expectedLimit, 0.01f);

        ppProfile.vignette.settings = vignetteSettings;
        Debug.Log("UpdatingFov : " + vignetteSettings.intensity);
        */

        Vector3 fwd = refTransform.forward;
        float av = Vector3.Angle(_lastFwd, fwd) / Time.deltaTime;
        av = (av - minAngVel) / (maxAngVel - minAngVel);

        Vector3 pos = refTransform.position;

        if (maxSpeed > 0)
        {
            float speed = (pos - _lastPos).magnitude / Time.deltaTime;
            speed = (speed - minSpeed) / (maxSpeed - minSpeed);

            if (speed > av)
            {
                av = speed;
            }
        }

        av = Mathf.Clamp01(av) * maxEffect;
        _av = Mathf.SmoothDamp(_av, av, ref _avSlew, smoothTime);

        _lastFwd = fwd;
        _lastPos = pos;

        vignetteSettings.intensity = _av;
        ppProfile.vignette.settings = vignetteSettings;
        Debug.Log("UpdatingFov : " + vignetteSettings.intensity);
    }
}


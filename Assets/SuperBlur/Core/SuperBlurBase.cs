using UnityEngine;

namespace SuperBlur
{

	[ExecuteInEditMode]
	public class SuperBlurBase : MonoBehaviour
	{
		protected static class Uniforms
		{
			public static readonly int _Radius = Shader.PropertyToID("_Radius");
			public static readonly int _BackgroundTexture = Shader.PropertyToID("_SuperBlurTexture");
		}

		public RenderMode renderMode = RenderMode.Screen;

		public BlurKernelSize kernelSize = BlurKernelSize.Small;

		[Range(0f, 1f)]
		public float interpolation = 1f;

		[Range(0, 4)]
		public int downsample = 1;

		[Range(1, 8)]
		public int iterations = 1;

		public bool gammaCorrection = true;

		public Material blurMaterial;

		public Material UIMaterial;


        #region Public Fields

        [Header("Angular Velocity")]
        /// <summary>
        /// Angular velocity calculated for this Transform. DO NOT USE HMD!
        /// </summary>
        [Tooltip("Angular velocity calculated for this Transform.\nDO NOT USE HMD!")]
        public Transform refTransform;

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
        public float maxEffect = 0.75f;

        /// <summary>
        /// Feather around cut-off as fraction of screen.
        /// </summary>
        [Range(0f, 0.5f)]
        [Tooltip("Feather around cut-off as fraction of screen.")]
        public float feather = 0.1f;

        /// <summary>
        /// Smooth out radius over time. 0 for no smoothing.
        /// </summary>
        [Tooltip("Smooth out radius over time. 0 for no smoothing.")]
        public float smoothTime = 0.15f;
        #endregion

        #region Smoothing
        private float _avSlew;
        private float _av;
        #endregion

        #region Shader property IDs
        private int _propAV;
        private int _propFeather;
        #endregion

        #region Eye matrices
        Matrix4x4[] _eyeToWorld = new Matrix4x4[2];
        Matrix4x4[] _eyeProjection = new Matrix4x4[2];
        #endregion

        #region Misc Fields
        private Vector3 _lastFwd;
        private Vector3 _lastPos;
        private Material _m;
        private Material _blur;
        private Camera _cam;
        #endregion

        //added
        void Awake()
        {
            _m = new Material(Shader.Find("Hidden/Tunnelling"));
            //_blur = new Material(Shader.Find("Custom/SuperBlurPostEffect"));

            if (refTransform == null)
            {
                refTransform = transform;
            }

            _propAV = Shader.PropertyToID("_AV");
            _propFeather = Shader.PropertyToID("_Feather");

            _cam = GetComponent<Camera>();
        }

        void Update()
        {
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

            _m.SetFloat(_propAV, _av);
            _m.SetFloat(_propFeather, feather);

            //test
            blurMaterial.SetFloat(_propAV, _av);
            blurMaterial.SetFloat(_propFeather, feather);

            _lastFwd = fwd;
            _lastPos = pos;
        }

        void OnPreRender()
        {
            // Update eye matrices
            Matrix4x4 local;
#if UNITY_2017_2_OR_NEWER
            if (UnityEngine.XR.XRSettings.enabled)
            {
#else
			if (UnityEngine.VR.VRSettings.enabled) {
#endif
                local = _cam.transform.parent.worldToLocalMatrix;
            }
            else
            {
                local = Matrix4x4.identity;
            }

            _eyeProjection[0] = _cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            _eyeProjection[1] = _cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
            _eyeProjection[0] = GL.GetGPUProjectionMatrix(_eyeProjection[0], true).inverse;
            _eyeProjection[1] = GL.GetGPUProjectionMatrix(_eyeProjection[1], true).inverse;

            _eyeProjection[0][1, 1] *= -1f;
            _eyeProjection[1][1, 1] *= -1f;

            // Hard-code far clip
            _eyeProjection[0][3, 3] = 0.001f;
            _eyeProjection[1][3, 3] = 0.001f;

            _eyeToWorld[0] = _cam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
            _eyeToWorld[1] = _cam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);

            _eyeToWorld[0] = local * _eyeToWorld[0].inverse;
            _eyeToWorld[1] = local * _eyeToWorld[1].inverse;

            _m.SetMatrixArray("_EyeProjection", _eyeProjection);
            _m.SetMatrixArray("_EyeToWorld", _eyeToWorld);

            //test
            blurMaterial.SetMatrixArray("_EyeProjection", _eyeProjection);
            blurMaterial.SetMatrixArray("_EyeToWorld", _eyeToWorld);

            //_m.DisableKeyword("TUNNEL_SKYBOX");
        }

        protected void Blur (RenderTexture source, RenderTexture destination)
		{

            RenderTexture originSource = RenderTexture.Instantiate(source);

			if (gammaCorrection)
			{
				Shader.EnableKeyword("GAMMA_CORRECTION");
			}
			else
			{
				Shader.DisableKeyword("GAMMA_CORRECTION");
			}

			int kernel = 0;

			switch (kernelSize)
			{
			case BlurKernelSize.Small:
				kernel = 0;
				break;
			case BlurKernelSize.Medium:
				kernel = 2;
				break;
			case BlurKernelSize.Big:
				kernel = 4;
				break;
			}

            RenderTexture rt2 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

            //RenderTexture rt3 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

            for (int i = 0; i < iterations; i++)
			{
				// helps to achieve a larger blur
				float radius = (float)i * interpolation + interpolation;
				blurMaterial.SetFloat(Uniforms._Radius, radius);

				Graphics.Blit(source, rt2, blurMaterial, 1 + kernel);
				source.DiscardContents();

				// is it a last iteration? If so, then blit to destination
				if (i == iterations - 1)
				{
                    //Graphics.Blit(rt2, destination, blurMaterial, 2 + kernel);
                    Graphics.Blit(rt2, destination, blurMaterial, 2 + kernel);
                    //FOV part
                    //Graphics.Blit(rt3, destination, _m);
                }
                else
				{
					Graphics.Blit(rt2, source, blurMaterial, 2 + kernel);
					rt2.DiscardContents();
				}
			}

			RenderTexture.ReleaseTemporary(rt2);
            //RenderTexture.ReleaseTemporary(rt3);
        }

        void OnDestroy()
        {
            DestroyImmediate(_m);
        }
    }
	
	public enum BlurKernelSize
	{
		Small,
		Medium,
		Big
	}

	public enum RenderMode
	{
		Screen,
		UI,
		OnlyUI
	}

}

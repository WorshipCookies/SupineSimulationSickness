// Pointer Direction Indicator|Prefabs|0057
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    public delegate void PointerDirectionIndicatorEventHandler(object sender);

    /// <summary>
    /// The Pointer Direction Indicator is used to determine a given world rotation that can be used by a Destiantion Marker.
    /// </summary>
    /// <remarks>
    /// The Pointer Direction Indicator can be attached to a VRTK_Pointer in the `Direction Indicator` parameter and will the be used to send rotation data when the destination marker events are emitted.
    ///
    /// This can be useful for rotating the play area upon teleporting to face the user in a new direction without expecting them to physically turn in the play space.
    /// </remarks>
    public class VRTK_PointerDirectionIndicator : MonoBehaviour
    {
        [Header("Appearance Settings")]

        [Tooltip("If this is checked then the reported rotation will include the offset of the headset rotation in relation to the play area.")]
        public bool includeHeadsetOffset = true;
        [Tooltip("If this is checked then the direction indicator will be displayed when the location is invalid.")]
        public bool displayOnInvalidLocation = true;
        [Tooltip("If this is checked then the pointer valid/invalid colours will also be used to change the colour of the direction indicator.")]
        public bool usePointerColor = false;

        [HideInInspector]
        public bool isActive = true;

        /// <summary>
        /// Emitted when the object tooltip is reset.
        /// </summary>
        public event PointerDirectionIndicatorEventHandler PointerDirectionIndicatorPositionSet;

        protected VRTK_ControllerEvents controllerEvents;
        protected Transform playArea;
        protected Transform headset;
        protected GameObject validLocation;
        protected GameObject invalidLocation;

        private Vector3 eulerAngle;
        private Vector3 storedRotation;
        private Vector3 initialRotation;

        private Vector3 transformedAngleMatrix;
        private Vector3 transformedAngleM;

        public Transform rightHandAnchor;

        public float sensibilityFactor = 3f;

        public virtual void OnPointerDirectionIndicatorPositionSet()
        {
            if (PointerDirectionIndicatorPositionSet != null)
            {
                PointerDirectionIndicatorPositionSet(this);
            }
        }

        /// <summary>
        /// The Initialize method is used to set up the direction indicator.
        /// </summary>
        /// <param name="events">The Controller Events script that is used to control the direction indicator's rotation.</param>
        public virtual void Initialize(VRTK_ControllerEvents events)
        {
            controllerEvents = events;
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            headset = VRTK_DeviceFinder.HeadsetTransform();

        }

        /// <summary>
        /// The SetPosition method is used to set the world position of the direction indicator.
        /// </summary>
        /// <param name="active">Determines if the direction indicator GameObject should be active or not.</param>
        /// <param name="position">The position to set the direction indicator to.</param>
        public virtual void SetPosition(bool active, Vector3 position)
        {
            transform.position = position;
            gameObject.SetActive((isActive && active));
            OnPointerDirectionIndicatorPositionSet();
        }
        
        /// <summary>
        /// The GetRotation method returns the current reported rotation of the direction indicator.
        /// </summary>
        /// <returns>The reported rotation of the direction indicator.</returns>
        public virtual Quaternion GetRotation()
        {
            float offset = (includeHeadsetOffset ? playArea.eulerAngles.y - headset.eulerAngles.y : 0f);
            Quaternion ret = Quaternion.Euler(0f, transform.localEulerAngles.y + offset, 0f);
            //transform.localEulerAngles = storedRotation;
            return ret; //transform.localEulerAngles.y +  it was removed in order to have correct direction avec TP
        }
        

        /// <summary>
        /// The SetMaterialColor method sets the current material colour on the direction indicator.
        /// </summary>
        /// <param name="color">The colour to update the direction indicatormaterial to.</param>
        /// <param name="validity">Determines if the colour being set is based from a valid location or invalid location.</param>
        public virtual void SetMaterialColor(Color color, bool validity)
        {
            validLocation.SetActive(validity);
            invalidLocation.SetActive((displayOnInvalidLocation ? !validity : validity));

            if (usePointerColor)
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.color = color;
                }
            }
        }

        protected virtual void Awake()
        {
            validLocation = transform.Find("ValidLocation").gameObject;
            invalidLocation = transform.Find("InvalidLocation").gameObject;
            gameObject.SetActive(false);
            initialRotation = transform.rotation.eulerAngles;
            Debug.Log("Cursor initial rotation y : " + initialRotation.y);
        }


        protected virtual void Update()
        {
            //transform.localEulerAngles = playArea.transform.localEulerAngles;

            if (controllerEvents != null)
            {

                eulerAngle = rightHandAnchor.rotation.eulerAngles;

                //eulerAngle = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles; //+ new Vector3(90, 0, 0);

                //Vector3 position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                

                //Quaternion rotation = Quaternion.Euler(90, 0, 0);
                //Matrix4x4 m = Matrix4x4.Rotate(rotation);
                //transformedAngleMatrix = m.MultiplyVector(eulerAngle);

                transformedAngleM = Quaternion.Euler(90, 0, 0) * eulerAngle;

                //transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));


                Vector2 forward2D = new Vector2(playArea.transform.forward.x, playArea.transform.forward.z).normalized;

                Vector2 destVect2D = (new Vector2(transform.position.x, transform.position.z) - new Vector2(playArea.transform.position.x, playArea.transform.position.z)).normalized;

                float baseAngle = Vector2.SignedAngle(destVect2D, forward2D);

                float correctAngle = transform.localEulerAngles.y;
                //Debug.Log(baseAngle);

                float touchpadAngle = controllerEvents.GetTouchpadAxisAngle();
                float angle = ((touchpadAngle > 180) ? touchpadAngle -= 360 : touchpadAngle) + headset.eulerAngles.y;

                transform.eulerAngles = new Vector3(0f, (eulerAngle.z * (-sensibilityFactor)) + playArea.transform.eulerAngles.y + baseAngle, 0f);//(360 - eulerAngle.z * 3f) + playArea.transform.eulerAngles.y +

                //Debug.Log("angle of z : " + (eulerAngle.z * (-1f)) + " Transformed angle Matrix : " + (transformedAngleM.z * (-1f)) + " angle of cursor : " + transform.eulerAngles.y);

                //storedRotation = transform.localEulerAngles;
                //Debug.Log("eulerAngle of Player : " + playArea.transform.eulerAngles.y);
                //Debug.Log("eulerAngle of indicator : " + transform.eulerAngles.y);
            }
        }
        void ComputeAngleBetweenSrcDest()
        {
            Vector3.Angle(playArea.transform.forward, transform.position - playArea.transform.position);
            
        }
    }
}
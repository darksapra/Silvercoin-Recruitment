using UnityEngine;
using UnityEngine.UI;

namespace sapra.silvercoin_project
{
    [RequireComponent(typeof(ShipInput))]
    [RequireComponent(typeof(Rigidbody))]
    public class ShipController : MonoBehaviour
    {
        [Header("Component")]
        [Tooltip("Ship Model")]
        [SerializeField] private Transform model;
        [Tooltip("Ship Camera Offset")]
        [SerializeField] private Transform shipCameraOffset;
        private ShipInput _input;
        private Rigidbody rb;

        [Header("Parameters")]
        [Tooltip("Maximum and minimum acceleration")]
        [SerializeField] private Vector2 maxAcceleration = new Vector2(10, 50);
        [Tooltip("Maximum distance ship from center")]
        [SerializeField] private float maxDistance = 50;
        [SerializeField] private float verticalOffset;

        private float currentAcceleration = 30;
        private Vector3 upVector;

        [Header("UI Components")]
        [Tooltip("Acceleration slider")]
        [SerializeField] private Slider slider;
        [Tooltip("Mouse position visualizer")]
        [SerializeField] private LineRenderer mouseLine;

        #region Monobehaviour Methods
        // Start is called before the first frame update
        void Start()
        {
            _input = GetComponent<ShipInput>();
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        void FixedUpdate() {
            float mouseSens = (currentAcceleration/maxAcceleration.y);
            Vector3 desiredDirection = transform.forward+_input.getMouseInputWorld*(1.2f-mouseSens)/20f;
            ChangeVelocity();
            Accelerate(desiredDirection.normalized*currentAcceleration);
            RotateModel(mouseSens);
        }
        void Update()
        {
            UpdateUI();
        }
        #endregion

        private void ChangeVelocity()
        {
            currentAcceleration += _input.getAcceleration;
            currentAcceleration = Mathf.Clamp(currentAcceleration, maxAcceleration.x, maxAcceleration.y);
        }
        private void Accelerate(Vector3 value)
        {
            rb.velocity = value;
            transform.rotation = Quaternion.FromToRotation(transform.forward, rb.velocity)*transform.rotation;
        }
        private void RotateModel(float speedFactor)
        {
            upVector = Vector3.Slerp(upVector,-_input.getMouseInputWorld, Time.deltaTime*(3f-speedFactor));
            upVector = Vector3.ProjectOnPlane(upVector, transform.forward);

            Vector3 offset = upVector*maxDistance+upVector.normalized*verticalOffset;
            if(offset.magnitude > maxDistance)
                offset = offset.normalized*maxDistance;

            shipCameraOffset.transform.position = transform.position - offset;
            model.rotation = Quaternion.LookRotation(transform.forward, -upVector.normalized);
        }
        private void UpdateUI()
        {
            mouseLine.SetPosition(1, _input.getMouseInputRaw);
            slider.normalizedValue = (currentAcceleration-maxAcceleration.x)/(maxAcceleration.y-maxAcceleration.x);
        }
    }
}
using UnityEngine;

namespace sapra.silvercoin_project
{
    public class ShipInput : MonoBehaviour
    {
        [SerializeField] private float _accelerationValues;
        [SerializeField] private KeyCode LockTarget = KeyCode.Mouse1;
        
        [SerializeField] private Vector3 _mouseInputRaw;
        [SerializeField] private Vector3 _mouseInputWorld;


        public float verticalSens = 5;
        public float horizontalSens = 3;
        public delegate void keyPressed();
        public event keyPressed lockTarget;

        public float getAcceleration{
            get{
                return _accelerationValues;
            }
        }
        public Vector3 getMouseInputWorld{
            get{
                return _mouseInputWorld;
            }
        }
        public Vector3 getMouseInputRaw{
            get{
                return _mouseInputRaw;
            }
        }
        // Update is called once per frame
        void Update()
        {
            GetValues();
        }
        
        void Start()
        {
            if(MenuManager.instance != null)
            {
                horizontalSens = MenuManager.instance.getHori();
                verticalSens = MenuManager.instance.getVert();
            }
        }
        private void GetValues()
        {
            _accelerationValues = Input.GetAxis("Vertical");
            GetMouseValues();
            if(Input.GetKeyDown(LockTarget) && lockTarget != null)
                lockTarget.Invoke();

        }
        private void GetMouseValues()
        {
/*             float MaxValue = Mathf.Max(Screen.width, Screen.height);
            Vector3 screenSize = new Vector2(Screen.width/2, Screen.height/2); //Updated in case of changes in resolution
            //_mouseInputRaw = (Input.mousePosition-screenSize); //Mouse position, with center on middle of the screen
            _mouseInput = (Input.mousePosition-screenSize)/MaxValue; //Mouse input from 0, 1 in reference to the center */
            float MaxValue = Mathf.Max(Screen.width, Screen.height);
            _mouseInputRaw += new Vector3(Input.GetAxis("Mouse X")*horizontalSens, Input.GetAxis("Mouse Y")*verticalSens, 0);
            _mouseInputWorld = transform.TransformDirection(_mouseInputRaw/MaxValue);
        }
    }
}
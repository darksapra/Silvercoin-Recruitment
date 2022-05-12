using System.Collections.Generic;
using UnityEngine;

namespace sapra.silvercoin_project
{
    [RequireComponent(typeof(BulletSharedData))]
    [RequireComponent(typeof(ShipInput))]
    public class TargetDetector : MonoBehaviour
    {
        [Header("Parameters")]
        [Tooltip("Animation image scaling curve")]
        public AnimationCurve targetScaleAnimation = new AnimationCurve(new Keyframe[]{new Keyframe(0,1), new Keyframe(1,1)});
        [Tooltip("Animation Length in seconds")]
        [Range(0.1f, 3)] public float animationLength = 2;

        public List<AsteroidDistance> asteroids;
        [SerializeField] private AsteroidDistance currentTarget;
        private AsteroidDistance dynamicTarget;
        private ShipInput shipInput;
        private Vector2 screenRes;
        private float currentTime;

        [Header("Components")]
        private BulletSharedData bulletsController;
        private Camera cam;

        [Header("UI Components")]
        [Tooltip("Image to visualize the dynamic Target")]
        [SerializeReference] private GameObject TargetableImage;
        [Tooltip("Image to visualize the targeted object")]
        [SerializeReference] private GameObject TargetedImage;

        #region Monobehaviour Methods
        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;
            bulletsController = this.GetComponent<BulletSharedData>();
            shipInput = this.GetComponent<ShipInput>();
            shipInput.lockTarget += UpdateTarget;
            screenRes = new Vector2(Screen.width/2, Screen.height/2);
            GetCurrentAsteroids();
        }

        // Update is called once per frame
        void Update()
        {
            GetDynamicTarget();
            float amount = UpdateAnimation();
            UpdateTargetIcon(amount, TargetableImage.transform, dynamicTarget);
            UpdateTargetIcon(-amount, TargetedImage.transform, currentTarget);
        }
        #endregion

        void GetCurrentAsteroids()
        {
            asteroids = new List<AsteroidDistance>();
            GameObject[] initial = GameObject.FindGameObjectsWithTag("Asteroid");
            foreach(GameObject gameObject in initial)            
                asteroids.Add(new AsteroidDistance(gameObject, 0));
        }
        void GetDynamicTarget()
        {
            for(int i = asteroids.Count-1; i >= 0;  i--)
            {
                if(asteroids[i].GetAsteroid == null)                
                    asteroids.RemoveAt(i);                
                else
                {
                    float dotProduct = Vector3.Dot((asteroids[i].GetAsteroid.transform.position-cam.transform.position).normalized, cam.transform.forward);
                    asteroids[i].distance = dotProduct;
                }
            }
            asteroids.Sort((a, b) => b.distance.CompareTo(a.distance));
            dynamicTarget = asteroids.Count > 0 ? asteroids[0].distance > 0.2f ? asteroids[0] : null : null;
        }

        void UpdateTarget()
        {
            if(currentTarget == dynamicTarget)
                currentTarget = null;
            else
                currentTarget = dynamicTarget;

            Transform currentTargetTransform = currentTarget != null ? currentTarget.GetAsteroid.transform : null;
            bulletsController.SetTarget(currentTargetTransform);
        }
        #region  UI
        float UpdateAnimation()
        {
            currentTime += Time.deltaTime;
            if(currentTime > animationLength)
                currentTime = 0;
            return targetScaleAnimation.Evaluate(currentTime/animationLength);
        }
        void UpdateTargetIcon(float amount, Transform image, AsteroidDistance Target)
        {
            if(Target == null || Target.GetAsteroid == null)
            {
                image.gameObject.SetActive(false);
                return;
            }
            image.gameObject.SetActive(true);
            Vector3 position = cam.WorldToScreenPoint(Target.GetAsteroid.transform.position);
            image.position = position;
            image.localScale = Vector3.one*amount;
            image.Rotate(image.forward, amount/2);
        }
        #endregion
    }
        [System.Serializable]
        public class AsteroidDistance
        {
            [SerializeField] private GameObject Asteroid;
            public float distance{get; set;}
            public AsteroidDistance(GameObject asteroid, float distance)
            {
                this.Asteroid = asteroid;
                this.distance = distance;
            }
            public GameObject GetAsteroid{
                get{
                    return Asteroid;
                }
            }
        }

}

using System.Collections;
using UnityEngine;

namespace sapra.silvercoin_project
{
    [RequireComponent(typeof(Rigidbody))]
    public class BulletTarget : MonoBehaviour
    {
        [Header("Parameters")]
        public float bulletVelocity = 400;
        public float damage = 100;
        public AnimationCurve bulletAnimation;
        public float animationLength = 2;

        private Camera cam;
        private LayerMask mask;
        private Rigidbody rb;

        public delegate void destroyed(BulletTarget bullet, Vector3 position, Quaternion rotation);
        public event destroyed bulletOut;

        #region Monobehaviour Methods
        void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
            mask = 1 << 7; //Getting the asteroid Mask
            cam = Camera.main;
        }
        void FixedUpdate()
        {
            HitCheck();
            RotateBullet();
        }
        #endregion

        public void ActivateBullet(Transform target, GameObject shooter)
        {
            StartCoroutine(ShootBullet(target, shooter));
        }

        private IEnumerator ShootBullet(Transform target, GameObject shooter)
        {
            Rigidbody shooterRB = shooter.GetComponent<Rigidbody>();
            if(target == null) //Without a target bullets will go straigth
            {
                Vector3 direction = getShootDirection();
                rb.velocity = bulletVelocity*direction+shooterRB.velocity;
                StartCoroutine(WaitToDie(shooter.transform));
            }
            else
            {
                Vector3 initial = transform.up;
                for(float i = 0; i< animationLength; i+=Time.deltaTime) //Bullet Curve
                {
                    if(target == null)
                        break;
                    float result = bulletAnimation.Evaluate(i/animationLength);
                    Vector3 targetDirection = (target.position-transform.position).normalized;
                    Vector3 direction = Vector3.Lerp(initial, targetDirection, result);
                    rb.velocity = direction*(bulletVelocity+shooterRB.velocity.magnitude);
                    yield return null;
                }
                while(target != null) //Go to target
                {
                    Vector3 direction = (target.position-transform.position).normalized;
                    rb.velocity = direction*(bulletVelocity+shooterRB.velocity.magnitude);
                    yield return null;
                }
                StartCoroutine(WaitToDie(shooter.transform));
            }
        }
        private IEnumerator WaitToDie(Transform shooter)
        {
            while((shooter.position-transform.position).magnitude < 500)
                yield return null;
            if(bulletOut != null)
                bulletOut.Invoke(this, Vector3.zero, Quaternion.identity);
        }
        private void RotateBullet()
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, rb.velocity)*transform.rotation;
        }
        private void HitCheck()
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.up, out hit, rb.velocity.magnitude*Time.deltaTime, mask))
            {
                Health touchedHealth = hit.collider.gameObject.GetComponent<Health>();
                if(touchedHealth != null)
                    touchedHealth.Damage(damage);
                if(bulletOut != null)
                    bulletOut.Invoke(this, hit.point, Quaternion.LookRotation(hit.normal, transform.up));
            }
        }
        private Vector3 getShootDirection() //Aim assist when rotating
        {
            RaycastHit hit;
            Vector3 position = cam.transform.position+cam.transform.forward*100;
            if(Physics.Raycast(position, cam.transform.forward, out hit,1000 ,mask))            
                return (hit.point-transform.position).normalized;            
            return transform.up;
        }
    }
}
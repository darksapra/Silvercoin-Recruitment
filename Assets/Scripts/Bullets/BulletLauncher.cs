using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace sapra.silvercoin_project
{
    [RequireComponent(typeof(BulletSharedData))]
    public class BulletLauncher : MonoBehaviour
    {
        [Header("Parameters")]
        public float reloadTime = 0.2f;
        public BulletTarget bulletPrefab;
        public ParticleSystem particleHitPrefab;
        [Tooltip("Can bullets follow a target?")]
        public bool targetable;
        public KeyCode shootKey;

        [Header("UI Components")]
        public Slider reloadVisualizer;

        private float currentReloadTime = 0;
        private List<BulletTarget> bulletsDisabled;
        private List<ParticleSystem> hitParticlesDisabled;
        private BulletSharedData bc;

        #region Monobehaviour Methods
        // Start is called before the first frame update
        void Start()
        {
            bc = GetComponent<BulletSharedData>();
            bulletsDisabled = new List<BulletTarget>();
            hitParticlesDisabled = new List<ParticleSystem>();
        }
        void Update() {
            InputCheck();          
            UpdateUI();
        }
        #endregion

        private void InputCheck()
        {
            if(currentReloadTime <= reloadTime+1)
                currentReloadTime += Time.deltaTime;
            if(Input.GetKey(shootKey))            
                GenerateBullet();            
        }

        #region Bullet
        private void GenerateBullet()
        {
            if(currentReloadTime>= reloadTime)
            {
                currentReloadTime = 0;
                if(bulletsDisabled.Count > 0)
                {
                    BulletTarget bullet = bulletsDisabled[0];
                    bulletsDisabled.RemoveAt(0);
                    StartCoroutine(ShootBullet(bullet));
                }
                else
                {
                    BulletTarget newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bc.bulletsParent);
                    StartCoroutine(ShootBullet(newBullet));
                }
            }
        }
        IEnumerator ShootBullet(BulletTarget bullet)
        {
            bullet.transform.position = bc.model.position-bc.model.up*2;
            bullet.transform.rotation = Quaternion.FromToRotation(bullet.transform.up, bc.model.forward)*bullet.transform.rotation;
            bullet.gameObject.SetActive(true);
            bullet.bulletOut += GenerateParticles;
            Transform target = targetable ? bc.getTarget : null;
            bullet.ActivateBullet(target, this.gameObject);       
            yield return null;
        }
        #endregion
        #region Particles
        private void GenerateParticles(BulletTarget bullet, Vector3 position, Quaternion rotation)
        {
            if(position != Vector3.zero)
            {
                if(hitParticlesDisabled.Count > 0)
                {
                    ParticleSystem hitParticle = hitParticlesDisabled[0];
                    hitParticle.transform.position = position;
                    hitParticle.transform.rotation = rotation;
                    hitParticlesDisabled.RemoveAt(0);
                    StartCoroutine(EnableHitParticle(hitParticle));
                }
                else if(particleHitPrefab != null)
                {                        
                    ParticleSystem newParticles = Instantiate(particleHitPrefab, position, rotation, bc.bulletsParent);
                    StartCoroutine(EnableHitParticle(newParticles));
                }
            }

            bullet.bulletOut -= GenerateParticles;
            bullet.gameObject.SetActive(false);
            bulletsDisabled.Add(bullet);
        }
        IEnumerator EnableHitParticle(ParticleSystem particle)
        {
            particle.gameObject.SetActive(true);
            while(particle.isPlaying)
                yield return null;
            hitParticlesDisabled.Add(particle);
            particle.gameObject.SetActive(false);
            yield return null;
        }
        #endregion

        void UpdateUI(){
            if(reloadVisualizer != null)
                reloadVisualizer.value = currentReloadTime/reloadTime;
        }
    }
}

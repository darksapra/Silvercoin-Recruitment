using UnityEngine;

namespace sapra.silvercoin_project
{
    public class Health : MonoBehaviour
    {
        public delegate void Destroyed();
        public event Destroyed onDestroy;
        public float maxHealth;
        public float currentHealth;
        public float points;
        void Start() {
            currentHealth = maxHealth;
        }
        public void Damage(float amount)
        {
            currentHealth -= amount;
            if(currentHealth <= 0)
            {
                Destroy(this.transform.parent.gameObject);
                if(onDestroy != null)
                    onDestroy.Invoke();
            }
        }
    }
}

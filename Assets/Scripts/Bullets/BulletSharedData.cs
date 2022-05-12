using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.silvercoin_project
{
    public class BulletSharedData : MonoBehaviour
    {
        public Transform bulletsParent;
        private Transform target;
        public Transform model;
        public void SetTarget(Transform target)
        {
            this.target = target;
        }
        public Transform getTarget
        {
            get{
                return target;
            }
        }
    }
}

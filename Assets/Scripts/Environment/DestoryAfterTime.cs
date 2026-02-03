using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace HT
{
    public class DestoryAfterTime : MonoBehaviour
    {
        public float timeToDestory = 2f;
        void OnEnable()
        {
            Invoke("PushInPool", timeToDestory);
        }

        void OnDisable()
        {
            CancelInvoke();
        }
        private void PushInPool()
        {
            PoolMgr.Instance.PushObj(this.gameObject);
        }
    }
}


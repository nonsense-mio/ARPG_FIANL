using System.Collections;
using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;
namespace ARPG
{
    public class DestoryAfterTime : ARPGController
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
            this.GetSystem<IPoolSystem>().Recycle(this.gameObject);
        }
    }
}


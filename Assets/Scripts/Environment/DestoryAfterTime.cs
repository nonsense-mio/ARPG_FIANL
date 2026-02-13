using System.Collections;
using System.Collections.Generic;
using ARPG;
using Unity.VisualScripting;
using UnityEngine;
namespace ARPG
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
            GameArchitecture.Interface.GetSystem<IPoolSystem>().Recycle(this.gameObject);
        }
    }
}


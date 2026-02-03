using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class FogWall : MonoBehaviour
    {
        private void Awake()
        {
            ActivateFogWall(false);
        }
        public void ActivateFogWall(bool activate)
        {
            gameObject.SetActive(activate);
        } 
    }

}

using Framework;
using UnityEngine;

namespace ARPG
{
    public abstract class ARPGController : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameArchitecture.Interface;
        }
    }
}
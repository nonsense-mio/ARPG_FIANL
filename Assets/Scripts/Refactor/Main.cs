using ARPG;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        GameArchitecture.Interface.GetSystem<IUISystem>().ShowPanel<BeginPanel>();
        GameArchitecture.Interface.GetSystem<IMusicSystem>().PlayBGM("BeginScene");
    }
}

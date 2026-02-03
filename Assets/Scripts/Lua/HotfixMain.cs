using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class HotfixMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LuaMgr.Instance.Init();
        LuaMgr.Instance.DoLuaFile("Main");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

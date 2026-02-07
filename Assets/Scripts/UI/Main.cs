using System.Collections;
using System.Collections.Generic;
using HT;
using UnityEngine;
using UnityEngine.U2D;

public class Main : MonoBehaviour
{
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;

    // Start is called before the first frame update
    void Start()
    {
        UIMgr.Instance.ShowPanel<BeginPanel>();
        MusicMgr.Instance.PlayBKMusic("BeginScene");
        // ABUpdateMgr.Instance.CheckUpdate((isOver) =>
        // {
        //     if (isOver)
        //     {
        //         // UIMgr.Instance.GetPanel<TipPanel>((panel) =>
        //         // {
        //         //     panel.SetTipInfo("更新完毕");
        //         // });
        //         LuaMgr.Instance.Init();
        //         LuaMgr.Instance.DoLuaFile("Main");
        //     }
        //     else
        //     {
        //         UIMgr.Instance.GetPanel<TipPanel>((panel) =>
        //         {
        //             panel.SetTipInfo("更新失败,请检查网络连接后重试");
        //         });
        //     }

        // }, (updateInfo) =>
        // {
        //     Debug.Log(updateInfo);
        //     UIMgr.Instance.ShowPanel<TipPanel>(E_UILayer.Top, (panel) =>
        //     {
        //         panel.SetTipInfo("检测到资源更新：" + updateInfo);
        //     });
        // });
    }

}

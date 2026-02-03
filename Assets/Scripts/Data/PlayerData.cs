using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    //人物状态
    public string playerName;
    public int playerLevel = 1;
    public int healthLevel = 10;
    public int staminaLevel = 10;
    public int focusLevel = 10;
    public int poiseLevel = 10;
    public int strengthLevel = 10;
    public int dexterityLevel = 10;
    public int intelligenceLevel = 10;
    public int faithLevel = 10;
    public int currentSoulCount;

    //人物坐标
    public float xPos;
    public float yPos;
    public float zPos;


    //用json存储宝箱开启状态 LitJson字典的键必须是string类型
    public Dictionary<string, bool> chestDic = new Dictionary<string, bool>();
    //篝火激活状态
    public Dictionary<string, bool> bonfireDic = new Dictionary<string, bool>();

    //当前激活的复活点篝火ID
    public string lastRestedBonfireID = "";

    //复活点坐标（用于死亡复活）
    public float respawnX;
    public float respawnY;
    public float respawnZ;

}

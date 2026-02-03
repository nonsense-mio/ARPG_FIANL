using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HT
{
    public class Item_SO :ScriptableObject
    {
        [Header("物品信息")]
        public Sprite itemIcon;
        public string itemName;
        public int itemID;
    }
}


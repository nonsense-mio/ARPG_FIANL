using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    [CreateAssetMenu( menuName = "Items/Ammo Item")]
    public class RangedAmmoItem_SO : Item_SO
    {
        [Header("弹药类型")]
        public E_AmmoType ammoType;
        [Header("弹药速度")]
        public float forwardVelocity = 550;
        public float upwardVelocity = 0;
        public float ammoMass = 0;
        public bool useGravity = false;

        [Header("弹药容量")]
        public int carryLimit = 99;
        public int carryAmount = 99;

        [Header("弹药伤害")]
        public int physicalDamage = 50;

        [Header("弹药预制体")]
        public string loadedItemModelName = "Projectiles/Arrow_Loaded_Model";//装载的箭矢模型
        public string liveAmmoModelName = "Projectiles/Arrow_Live_Model"; //发射出去的箭矢模型
        public string penetratedModelName = "Projectiles/Arrow_Penetrated_Model";//射到碰撞体上的箭矢模型
    }
}


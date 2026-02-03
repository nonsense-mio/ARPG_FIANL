using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class EqiupmentItem_SO : Item_SO
    {
        [Header("防御属性")]
        public float physicalDefense;
        public float magicDefense;

        [Header("抗性")]
        public float poisonResistance;
        public float fireResistance;
    }

}

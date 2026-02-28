using System;
using UnityEngine;

namespace ARPG
{
    public class PlayerEquipmentManager : MonoBehaviour
    {
        PlayerManager player;
        [Header("装备模型切换器")]
        HelmetModelChanger helmetModelChanger;
        TorsoModelChanger torsoModelChanger;
        UpperLeftArmModelChanger upperLeftArmModelChanger;
        UpperRightArmModelChanger upperRightArmModelChanger;
        HipModelChanger hipModelChanger;
        LeftLegModelChanger leftLegModelChanger;
        RightLegModelChanger rightLegModelChanger;
        LowerLeftArmModelChanger lowerLeftArmModelChanger;
        LowerRightArmModelChanger lowerRightArmModelChanger;
        LeftHandModelChanger leftHandModelChanger;
        RightHandModelChanger rightHandModelChanger;
        [Header("默认模型")]
        public GameObject nakedHeadModel;
        public string nakedTorsoModel;
        public string nakedUpperLeftArmModel;
        public string nakedUpperRightArmModel;
        public string nakedLowerLeftArmModel;
        public string nakedLowerRightArmModel;
        public string nakedLeftHandModel;
        public string nakedRightHandModel;
        public string nakedHipModel;
        public string nakedLeftLegModel;
        public string nakedRightLegModel;

        public void Init(PlayerManager playerManager)
        {
            player = playerManager;
            helmetModelChanger = GetComponentInChildren<HelmetModelChanger>();
            torsoModelChanger = GetComponentInChildren<TorsoModelChanger>();
            hipModelChanger = GetComponentInChildren<HipModelChanger>();
            leftLegModelChanger = GetComponentInChildren<LeftLegModelChanger>();
            rightLegModelChanger = GetComponentInChildren<RightLegModelChanger>();
            upperLeftArmModelChanger = GetComponentInChildren<UpperLeftArmModelChanger>();
            upperRightArmModelChanger = GetComponentInChildren<UpperRightArmModelChanger>();
            lowerLeftArmModelChanger = GetComponentInChildren<LowerLeftArmModelChanger>();
            lowerRightArmModelChanger = GetComponentInChildren<LowerRightArmModelChanger>();
            leftHandModelChanger = GetComponentInChildren<LeftHandModelChanger>();
            rightHandModelChanger = GetComponentInChildren<RightHandModelChanger>();
        }

        private void Start()
        {
            EquipAllEquipmentModels();
        }

        /// <summary>
        /// 卸载 changers → 根据装备是否存在加载对应模型 → 设置防御值
        /// nakedToggle: 头盔专用，无装备时显示裸头模型
        /// nakedModelNames 中的 null 项表示该 changer 无裸装模型
        /// </summary>
        private void EquipSlot(
            ModelChanger[] changers,
            EqiupmentItem_SO equipment,
            string[] equippedModelNames,
            string[] nakedModelNames,
            Action<float> setDefense,
            GameObject nakedToggle = null)
        {
            foreach (var changer in changers)
                changer.UnEqiupmentAllChildrenModels();

            if (equipment != null)
            {
                if (nakedToggle != null) nakedToggle.SetActive(false);
                for (int i = 0; i < changers.Length; i++)
                    changers[i].EqiupmentModelByName(equippedModelNames[i]);
                setDefense(equipment.physicalDefense);
            }
            else
            {
                if (nakedToggle != null) nakedToggle.SetActive(true);
                for (int i = 0; i < changers.Length; i++)
                    if (nakedModelNames[i] != null)
                        changers[i].EqiupmentModelByName(nakedModelNames[i]);
                setDefense(0);
            }
        }

        public void EquipAllEquipmentModels()
        {
            var inv = player.playerInventoryManager;
            var stats = player.playerStatsManager;

            // 头盔
            var helmet = inv.currentHelmet;
            EquipSlot(
                new ModelChanger[] { helmetModelChanger },
                helmet,
                helmet != null ? new[] { helmet.helmetModelName } : null,
                new string[] { null },
                val => stats.physicalDamageAbsorptionHead = val,
                nakedHeadModel);

            // 躯干
            var body = inv.currentBody;
            EquipSlot(
                new ModelChanger[] { torsoModelChanger, upperLeftArmModelChanger, upperRightArmModelChanger },
                body,
                body != null ? new[] { body.torsoModelName, body.upperLeftArmModelName, body.upperRightArmModelName } : null,
                new[] { nakedTorsoModel, nakedUpperLeftArmModel, nakedUpperRightArmModel },
                val => stats.physicalDamageAbsorptionBody = val);

            // 手部
            var hands = inv.currentHands;
            EquipSlot(
                new ModelChanger[] { lowerLeftArmModelChanger, lowerRightArmModelChanger, leftHandModelChanger, rightHandModelChanger },
                hands,
                hands != null ? new[] { hands.lowerLeftArmModelName, hands.lowerRightArmModelName, hands.leftHandModelName, hands.rightHandModelName } : null,
                new[] { nakedLowerLeftArmModel, nakedLowerRightArmModel, nakedLeftHandModel, nakedRightHandModel },
                val => stats.physicalDamageAbsorptionHands = val);

            // 腿部
            var legs = inv.currentLegs;
            EquipSlot(
                new ModelChanger[] { hipModelChanger, leftLegModelChanger, rightLegModelChanger },
                legs,
                legs != null ? new[] { legs.hipModelName, legs.leftLegModelName, legs.rightLegModelName } : null,
                new[] { nakedHipModel, nakedLeftLegModel, nakedRightLegModel },
                val => stats.physicalDamageAbsorptionLegs = val);
        }
    }
}


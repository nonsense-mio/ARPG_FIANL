using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
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
        /// 在开始时装备所有装备模型
        /// </summary>
        public void EquipAllEquipmentModels()
        {
            //头盔装备
            helmetModelChanger.UnEqiupmentAllChildrenModels();
            if(player.playerInventoryManager.currentHelmet != null)
            {
                nakedHeadModel.SetActive(false);
                helmetModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentHelmet.helmetModelName);
                player.playerStatsManager.physicalDamageAbsorptionHead = player.playerInventoryManager.currentHelmet.physicalDefense;
                Debug.Log("Head Absorption: " + player.playerStatsManager.physicalDamageAbsorptionHead + "%");
            }
            else
            {
                nakedHeadModel.SetActive(true);
                player.playerStatsManager.physicalDamageAbsorptionHead = 0;
            }
            //躯干装备
            torsoModelChanger.UnEqiupmentAllChildrenModels();
            upperLeftArmModelChanger.UnEqiupmentAllChildrenModels();
            upperRightArmModelChanger.UnEqiupmentAllChildrenModels();
            if(player.playerInventoryManager.currentBody != null)
            {
                torsoModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentBody.torsoModelName);
                upperLeftArmModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentBody.upperLeftArmModelName);
                upperRightArmModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentBody.upperRightArmModelName);
                player.playerStatsManager.physicalDamageAbsorptionBody = player.playerInventoryManager.currentBody.physicalDefense;
                Debug.Log("Body Absorption: " + player.playerStatsManager.physicalDamageAbsorptionBody + "%");
            }
            else
            {
                torsoModelChanger.EqiupmentModelByName(nakedTorsoModel);
                upperLeftArmModelChanger.EqiupmentModelByName(nakedUpperLeftArmModel);
                upperRightArmModelChanger.EqiupmentModelByName(nakedUpperRightArmModel);
                player.playerStatsManager.physicalDamageAbsorptionBody = 0;
            }
            //手部装备
            lowerLeftArmModelChanger.UnEqiupmentAllChildrenModels();
            lowerRightArmModelChanger.UnEqiupmentAllChildrenModels();
            leftHandModelChanger.UnEqiupmentAllChildrenModels();
            rightHandModelChanger.UnEqiupmentAllChildrenModels();
            if(player.playerInventoryManager.currentHands != null)
            {
                lowerLeftArmModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentHands.lowerLeftArmModelName);
                lowerRightArmModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentHands.lowerRightArmModelName);
                leftHandModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentHands.leftHandModelName);
                rightHandModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentHands.rightHandModelName);
                player.playerStatsManager.physicalDamageAbsorptionHands = player.playerInventoryManager.currentHands.physicalDefense;
                Debug.Log("Hand Absorption: " + player.playerStatsManager.physicalDamageAbsorptionHands + "%");
            }
            else
            {
                lowerLeftArmModelChanger.EqiupmentModelByName(nakedLowerLeftArmModel);
                lowerRightArmModelChanger.EqiupmentModelByName(nakedLowerRightArmModel);
                leftHandModelChanger.EqiupmentModelByName(nakedLeftHandModel);
                rightHandModelChanger.EqiupmentModelByName(nakedRightHandModel);
                player.playerStatsManager.physicalDamageAbsorptionHands = 0;
            }
            //腿部装备
            hipModelChanger.UnEqiupmentAllChildrenModels();
            leftLegModelChanger.UnEqiupmentAllChildrenModels();
            rightLegModelChanger.UnEqiupmentAllChildrenModels();
            if(player.playerInventoryManager.currentLegs != null)
            {
                hipModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentLegs.hipModelName);
                leftLegModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentLegs.leftLegModelName);
                rightLegModelChanger.EqiupmentModelByName(player.playerInventoryManager.currentLegs.rightLegModelName);
                player.playerStatsManager.physicalDamageAbsorptionLegs = player.playerInventoryManager.currentLegs.physicalDefense;
                Debug.Log("Leg Absorption: " + player.playerStatsManager.physicalDamageAbsorptionLegs + "%");
            }
            else
            {
                hipModelChanger.EqiupmentModelByName(nakedHipModel);
                leftLegModelChanger.EqiupmentModelByName(nakedLeftLegModel);
                rightLegModelChanger.EqiupmentModelByName(nakedRightLegModel);
                player.playerStatsManager.physicalDamageAbsorptionLegs = 0;
            }

        }

    }
}



using UnityEngine;

namespace ARPG
{
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;
        public WeaponItem_SO currentWeapon;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;
        public bool isBackSlot;

        public GameObject currentWeaponModel;

        /// <summary>
        /// 卸载武器的方法
        /// </summary>
        public void UnloadWeapon()
        {
            if(currentWeaponModel != null)
            {
                currentWeaponModel.SetActive(false);
            }
        }
        /// <summary>
        /// 销毁当前武器
        /// </summary>
        public void UnloadWeaponAndDestory()
        {
            if(currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }

        /// <summary>
        /// 加载武器
        /// </summary>
        /// <param name="weaponItem"></param>
        public void LoadWeaponModel(WeaponItem_SO weaponItem)
        {
            //销毁 当前武器
            UnloadWeaponAndDestory();
            //传入null时 意味着要取消当前武器
            if (weaponItem == null)
            {
                //卸载武器
                UnloadWeapon();
                return;
            }
            //实例化武器
            GameObject model = Instantiate(weaponItem.modelPrefab);
            if(model != null )
            {
                if(parentOverride != null)
                {
                    model.transform.parent = parentOverride.transform;
                }
                else
                {
                    model.transform.parent = transform;
                }
                //设置武器的位置、旋转、缩放
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }
            //设置当前武器
            currentWeaponModel = model;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ARPG;
using HT;
using UnityEngine;
using UnityEngine.UI;

public class BagItem : MonoBehaviour, IItemBase<Item_SO>, IPoolable
{
    public Button btnSlot;
    public Image bkImage;
    public Image iconImage;

    public void InitInfo(Item_SO item)
    {
        iconImage.sprite = item.itemIcon;
    }

    public void OnSpawn() { }

    public void OnRecycle()
    {
        iconImage.sprite = null;
        iconImage.enabled = true;

        // 重置Transform状态，让LayoutGroup能正确排列
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        // 重置RectTransform的锚点位置
        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}

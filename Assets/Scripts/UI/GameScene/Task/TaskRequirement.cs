using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskRequirement : MonoBehaviour, IPoolObject
{
    private Text requireName;
    private Text requireAmount;

    private void Awake()
    {
        requireName = GetComponent<Text>();
        requireAmount = transform.GetChild(0).GetComponent<Text>();
    }

    public void InitRequirement(string name, int amount, int currentAmount)
    {
        requireName.text = name;
        requireAmount.text = currentAmount + "/" + amount;
    }
    public void InitRequirement(string name, bool isFinished)
    {
        if (isFinished)
        {
            requireName.text = name;
            requireAmount.text = "已完成";
            requireName.color = Color.green;
            requireAmount.color = Color.green;
        }
    }

    public void ResetInfo()
    {
        requireName.text = "";
        requireAmount.text = "0/0";
        requireName.color = Color.white;
        requireAmount.color = Color.white;
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

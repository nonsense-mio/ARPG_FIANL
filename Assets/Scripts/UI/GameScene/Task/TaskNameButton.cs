using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskNameButton : MonoBehaviour, IPoolObject
{
    public Button btnTask;
    public Text txtTaskName;
    public TaskData_SO currentData;

    public string txtTaskContent;

    // 初始化任务按钮
    public void Init(TaskData_SO data)
    {
        currentData = data;
        if(data.isCompleted)
            txtTaskName.text = "<color=green>" + data.taskName + "</color>";
        else
            txtTaskName.text = data.taskName;
        txtTaskContent = data.description;
    }

    // 重置任务按钮信息
    public void ResetInfo()
    {
        btnTask.onClick.RemoveAllListeners();
        currentData = null;
        txtTaskName.text = "";
        txtTaskContent = "";

        // 重置Transform状态
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

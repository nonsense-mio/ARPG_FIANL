
using System.Collections.Generic;
using UnityEngine;
namespace ARPG
{
    /// <summary>
    /// 模型切换器
    /// </summary>
    public abstract class ModelChanger : MonoBehaviour
    {
        //模型列表
        public List<GameObject> modelList = new List<GameObject>();

        protected virtual void Awake()
        {
            GetAllChildrenModels();
        }
        private void GetAllChildrenModels()
        {
            int childrenGameObjects = transform.childCount;
            //遍历所有子物体并添加到列表中
            for (int i = 0; i < childrenGameObjects; i++)
            {
                modelList.Add(transform.GetChild(i).gameObject);
            }
        }
        /// <summary>
        /// 卸下该装备类所有模型
        /// </summary>
        public virtual void UnEqiupmentAllChildrenModels()
        {
            foreach (GameObject model in modelList)
            {
                model.SetActive(false);
            }
        }
        /// <summary>
        /// 装备该类指定模型
        /// </summary>
        /// <param name="modelName">模型名</param>
        public virtual void EqiupmentModelByName(string modelName)
        {
            for(int i = 0; i < modelList.Count; i++)
            {
                if(modelList[i].name == modelName)
                {
                    modelList[i].SetActive(true);
                    break;
                }
            }
        }
    }

}
   
using System.IO;
using LitJson;
using QFramework;
using UnityEngine;

namespace ARPG
{
    /// <summary>
    /// JSON序列化方式
    /// </summary>
    public enum JsonType
    {
        JsonUtility,
        LitJson,
    }

    /// <summary>
    /// JSON工具接口
    /// </summary>
    public interface IStorage : IUtility
    {
        void SaveData(object data, string fileName, JsonType type = JsonType.LitJson);
        T LoadData<T>(string fileName, JsonType type = JsonType.LitJson) where T : new();
    }

    /// <summary>
    /// JSON工具实现 - 用于序列化和反序列化JSON数据
    /// </summary>
    public class JsonStorage : IStorage
    {
        public void SaveData(object data, string fileName, JsonType type = JsonType.LitJson)
        {
            string path = Application.persistentDataPath + "/" + fileName + ".json";

            //序列化 得到json字符串
            string jsonStr = "";
            switch (type)
            {
                case JsonType.JsonUtility:
                    jsonStr = JsonUtility.ToJson(data);
                    break;
                case JsonType.LitJson:
                    jsonStr = JsonMapper.ToJson(data);
                    break;
            }
            //把序列化的json字符串 存储到指定路径的文件中
            File.WriteAllText(path, jsonStr);
        }

        public T LoadData<T>(string fileName, JsonType type = JsonType.LitJson) where T : new()
        {
            string path = Application.persistentDataPath + "/" + fileName + ".json";

            if (!File.Exists(path))
            {
                path = Application.streamingAssetsPath + "/" + fileName + ".json";
                if (!File.Exists(path))
                {
                    return new T();
                }
            }

            string jsonStr = File.ReadAllText(path);

            T data = default(T);
            switch (type)
            {
                case JsonType.JsonUtility:
                    data = JsonUtility.FromJson<T>(jsonStr);
                    break;
                case JsonType.LitJson:
                    data = JsonMapper.ToObject<T>(jsonStr);
                    break;
            }

            return data;
        }
    }
}

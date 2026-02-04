using System.Collections;
using System.Collections.Generic;
using ARPG;
using QFramework;
using UnityEngine;

public class JsonStorageTest : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture()
    {
        return GameArchitecture.Interface;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 测试保存
        var testData = new TestData { name = "Test", value = 100 };
        this.GetUtility<IStorage>().SaveData(testData, "test");

        // 测试加载
        var loaded = this.GetUtility<IStorage>().LoadData<TestData>("test");
        Debug.Log($"Loaded: {loaded.name}, {loaded.value}");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
[System.Serializable]
public class TestData
{
    public string name;
    public int value;
}

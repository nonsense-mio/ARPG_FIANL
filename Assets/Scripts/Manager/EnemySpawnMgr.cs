using System.Collections;
using System.Collections.Generic;
using HT;
using UnityEngine;

public class EnemySpawnMgr : SingletonMono<EnemySpawnMgr>
{
    public List<EnemyManager> enemyList = new List<EnemyManager>();
    public EnemyManager boss;
    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
    public void SpawnEnemy()
    {
        foreach (var enemy in enemyList)
        {
            enemy.gameObject.SetActive(true);
        }
    }

    public void SpawnBoss()
    {
        boss.gameObject.SetActive(true);
    }
}

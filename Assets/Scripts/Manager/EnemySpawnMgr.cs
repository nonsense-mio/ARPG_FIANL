using System.Collections.Generic;
using ARPG;
using Framework;
using UnityEngine;

public class EnemySpawnMgr : MonoBehaviour
{
    public List<EnemyManager> enemyList = new List<EnemyManager>();
    public EnemyManager boss;

    void Start()
    {
        GameArchitecture.Interface.RegisterEvent<SpawnEnemyEvent>(_ => SpawnEnemy())
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        GameArchitecture.Interface.RegisterEvent<SpawnBossEvent>(_ => SpawnBoss())
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void SpawnEnemy()
    {
        foreach (var enemy in enemyList)
        {
            enemy.gameObject.SetActive(true);
        }
    }

    private void SpawnBoss()
    {
        boss.gameObject.SetActive(true);
    }
}

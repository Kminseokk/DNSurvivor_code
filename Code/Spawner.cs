using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public float levelTime;

    int level;
    float timer;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length -1); //난이도 조절

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }

        //if (timer > (level == 0 ? 0.5f : 0.2f))
        //{            
        //    timer = 0;
        //    Spawn();
        //}

    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0); //프리펩 종류 선택하는곳 level < 넣으면 시간에 따라서 결정
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

[System.Serializable] //직렬화
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int mon_health;
    public float mon_speed;
}
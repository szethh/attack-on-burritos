using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Singleton;
    
    public Enemy enemyPrefab;
    public float SpawnSpeed => 1/spawnBaseSpeed+3*spawnBaseSpeed - 344*spawnBaseSpeed*spawnBaseSpeed/(count+24*spawnBaseSpeed*spawnBaseSpeed);
    public float spawnBaseSpeed;
    public List<ObjectPooling.Pool> pools = new List<ObjectPooling.Pool>();

    private ObjectPooling _pooling;
    private float _nextSpawn;
    public int count;
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;

    private void Awake()
    {
        Singleton = this;
    }
    
    private void Start()
    {
        _pooling = ObjectPooling.Singleton;
        foreach (var p in pools)
        {
            ObjectPooling.Singleton.GeneratePool(p);
        }
    }

    private void Update()
    {
        if (Time.time > _nextSpawn)
        {
            _nextSpawn = Time.time + 6 * spawnBaseSpeed / (7 * SpawnSpeed);
            var amt = Random.Range(0, 5);
            count += amt;
            for (int i = 0; i < amt; i++)
            {
                Vector2 pos = new Vector2();
                // por si acaso
                while (pos.sqrMagnitude < minRadius)
                {
                    pos = (Vector2.one * float.Epsilon + Random.insideUnitCircle).normalized *
                          Random.Range(minRadius, maxRadius);
                }
                SpawnEnemy(pos);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Vector3.zero, minRadius);
        Gizmos.DrawWireSphere(Vector3.zero, maxRadius);
    }

    private void SpawnEnemy(Vector3 pos)
    {
        var en = _pooling.GetFromPool(enemyPrefab.gameObject).GetComponent<Enemy>();
        en.transform.position = pos;
        var health = Random.Range(1, 5);
        var size = Random.Range(0.6f, 2f);
        var moveSpeed = Random.Range(0.5f, 1.5f);
        // level can vary from 1.08 to 4.21
        en.Init(health, moveSpeed, size, GameManager.Singleton.players);
    }
}
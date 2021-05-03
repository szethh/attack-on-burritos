using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public Enemy enemyPrefab;
    public float spawnSpeed;
    public List<ObjectPooling.Pool> pools = new List<ObjectPooling.Pool>();

    private ObjectPooling _pooling;
    private float _nextSpawn;

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
            _nextSpawn = Time.time + 1f / spawnSpeed;
            var pos = Random.insideUnitCircle * 5 + Vector2.one * 5;
            SpawnEnemy(pos);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnEnemy(pos);
        }
    }

    private void SpawnEnemy(Vector3 pos)
    {
        var en = _pooling.GetFromPool(enemyPrefab.gameObject).GetComponent<Enemy>();
        en.transform.position = pos;
        en.Init(3, GameManager.Singleton.players);
    }
}
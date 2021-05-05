using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Singleton;
    
    public Enemy enemyPrefab;
    public float SpawnSpeed => Mathf.RoundToInt(Mathf.Log10(spawnBaseSpeed + count)+Mathf.Sqrt(count)/spawnBaseSpeed);
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
            _nextSpawn = Time.time + spawnBaseSpeed / SpawnSpeed;
            var pos = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
            SpawnEnemy(pos);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnEnemy(pos);
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
        en.Init(3, GameManager.Singleton.players);
    }
}
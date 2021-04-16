using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public List<Player> players;
    public Enemy enemyPrefab;
    
    public List<ObjectPooling.Pool> pools = new List<ObjectPooling.Pool>();

    private ObjectPooling _pooling;
    
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
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            var en = _pooling.GetFromPool(enemyPrefab.gameObject).GetComponent<Enemy>();
            en.transform.position = pos;
            en.Init(3, players);
        }
    }
}
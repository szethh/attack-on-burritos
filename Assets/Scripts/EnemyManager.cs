using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public List<Player> players;
    public List<Enemy> enemies;
    public Enemy enemyPrefab;
    public Transform enemyParent;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var en = Instantiate(enemyPrefab, transform);
            en.transform.position = pos;
            en.Init(3, players);
        }
    }
}
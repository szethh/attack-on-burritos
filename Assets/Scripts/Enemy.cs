using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour
{
    // CONST
    public float sepDst = 0.2f;
    public float moveSpeed = 1f;
    public float rotSpeed = 3f;
    
    public int maxHealth = 3;
    public float size;

    private List<Player> _players;
    
    private int _health;
    
    public void Init(int health, List<Player> players)
    {
        maxHealth = health;
        _health = maxHealth;

        _players = players;
        
        transform.localScale *= size;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var total = Vector3.zero;
        var ply = TargetPlayer();
        //print(ply);
        total += ply;

        var sep = Separate();
        total += sep;

        transform.up += total * (rotSpeed * Time.fixedDeltaTime);
        transform.position += transform.up * (moveSpeed * Time.fixedDeltaTime);
    }

    private Vector3 TargetPlayer()
    {
        var ply = Vector2.zero;
        if (_players.Count > 0)
        {
            var closest = _players.Aggregate(
                (curMin, x) =>
                    curMin == null ||
                    (x.transform.position-transform.position).sqrMagnitude <
                    (curMin.transform.position-transform.position).sqrMagnitude ? x : curMin);
            ply = (closest.transform.position - transform.position).normalized;
        }

        return ply;
    }

    private Vector3 Separate()
    {
        // var count = Physics2D.OverlapCircleNonAlloc(transform.position, sepDst, others);
        var others = Physics2D.OverlapCircleAll(
            transform.position, sepDst)
            .Select(x => x.GetComponent<Enemy>())
            .Where(x => x != null && x != this).ToArray();
        var count = others.Length;
        
        var avg = Vector3.zero;
        foreach (var o in others)
        {
            var dir = (transform.position - o.transform.position).normalized;
            avg += dir;
        }

        if (count > 0)
        {
            avg /= count;
        }
        return avg;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<Player>(out var p))
        {
            p.Hit(this);
        } else if (other.gameObject.TryGetComponent<Bullet>(out var b))
        {
            b.Hit();
            Hit(b);
        }
    }

    private void Hit(Bullet b)
    {
        _health--;
        if (_health <= 0)
        {
            // die
            print("died");
            ObjectPooling.Singleton.AddToPool(gameObject);
        }
    }
}

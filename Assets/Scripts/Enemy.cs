using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    // CONST
    public float sepDst = 20f;
    
    public int maxHealth = 3;
    public float size;

    private List<Player> _players;
    
    private Rigidbody2D _rb;
    private int _health;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Init(int health, List<Player> players)
    {
        maxHealth = health;
        _health = maxHealth;

        _players = players;
        
        transform.localScale = Vector3.one * size;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var ply = TargetPlayer();
        _rb.AddForce(ply);

        var sep = Separate();
        _rb.AddForce(sep);
    }

    private Vector2 TargetPlayer()
    {
        var ply = Vector2.zero;
        if (_players.Count > 0)
        {
            var closest = _players.Aggregate(
                (curMin, x) =>
                    curMin == null ||
                    (x.transform.position-transform.position).sqrMagnitude <
                    (curMin.transform.position-transform.position).sqrMagnitude ? x : curMin);
            print("targeting " + closest);
            ply = (closest.transform.position - transform.position).normalized;
        }

        return ply;
    }

    private Vector2 Separate()
    {
        var others = Physics2D.OverlapCircleAll(
            transform.position, sepDst).Select(x => x.GetComponent<Enemy>()).ToArray();

        var avg = Vector2.zero;
        foreach (var o in others)
        {
            if (o == this)
                break;
            var dir = (transform.position - o.transform.position).normalized;
            avg += (Vector2)dir;
        }

        if (others.Length > 0)
        {
            avg /= others.Length;
        }
        return avg;
    }
}

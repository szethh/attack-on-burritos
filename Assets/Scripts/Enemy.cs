using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    // CONST
    public float moveSpeed = 1f;
    public float rotSpeed = 3f;
    public Transform model;
    public Weapon weaponPrefab;

    public int maxHealth = 3;
    public float size;
    public int Level => Mathf.RoundToInt(maxHealth - size * 0.7f + moveSpeed * rotSpeed/60f);
    
    private List<Player> _players;
    
    private int _health;
    private SpriteRenderer _renderer;
    private ParticleSystem _particleSystem;
    private bool _invincible;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _renderer = model.GetComponent<SpriteRenderer>();
        _particleSystem = GetComponent<ParticleSystem>();
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Init(int health, List<Player> players)
    {
        _players = players;
        maxHealth = health;
        _health = maxHealth;
        transform.localScale *= size;
        if (_agent == null)
            _agent = GetComponent<NavMeshAgent>();

        _agent.speed = moveSpeed;
        _agent.updateUpAxis = false;
        _agent.updateRotation = false;
        _agent.angularSpeed = 0f;
    }

    private void FixedUpdate()
    {
        var ply = Seek();
        _agent.SetDestination(ply);

        try
        {
            model.up = (_agent.path.corners[1] - transform.position).normalized;
            var oldRot = model.rotation.eulerAngles;
            oldRot.x = 0;
            oldRot.y = 0;
            model.rotation = Quaternion.Euler(oldRot);
        }
        catch
        {
            // ignored
        }
    }

    private Vector3 Seek()
    {
        var ply = transform.position;
        if (_players.Count > 0)
        {
            var closest = _players.Where(x => x.gameObject.activeInHierarchy).Aggregate(
                (curMin, x) =>
                    curMin == null ||
                    (x.transform.position-transform.position).sqrMagnitude <
                    (curMin.transform.position-transform.position).sqrMagnitude ? x : curMin);
            return closest.transform.position;
        }
        return ply;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Player>(out var p))
        {
            if (p.HitByEnemy(this))
                Die(true);
        } else if (other.gameObject.TryGetComponent<Bullet>(out var b) && !_invincible)
        {
            b.Hit(this);
            Hit(b);
        }
    }

    private void Hit(Bullet b)
    {
        _invincible = true;
        _renderer.DOColor(Color.red, 0.15f).SetEase(Ease.Flash, 2).OnComplete(() => _invincible = _health <= 0);
        _health -= b.weapon.weaponDamage;
        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die(bool disableDrop = false)
    {
        //GetComponent<Collider2D>().enabled = false;
        _particleSystem.Play();
        _renderer.DOFade(0f, 0.1f).SetDelay(0.1f).OnComplete(() =>
        {
            if (Random.value < 0.1f * Mathf.Log10(Level) && !disableDrop || true)
            {
                var drop = Instantiate(weaponPrefab);
                drop.transform.SetParent(GameManager.Singleton.itemParent);
                drop.transform.position = transform.position;
                // all weapons have the same chance (for now)
                drop.stats = GameManager.Singleton.weaponStatsList[Random.Range(0, GameManager.Singleton.weaponStatsList.Count)];
            }
            gameObject.SetActive(false);
        });
    }
}

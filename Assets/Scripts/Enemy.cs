using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    // CONST
    public float sepDst = 0.2f;
    public float moveSpeed = 1f;
    public float rotSpeed = 3f;
    public Weapon weaponPrefab;

    public int maxHealth = 3;
    public float size;
    public int Level => Mathf.RoundToInt(maxHealth - size * 0.7f + moveSpeed * rotSpeed);

    private List<Player> _players;
    
    private int _health;
    private SpriteRenderer _renderer;
    private ParticleSystem _particleSystem;

    public void Init(int health, List<Player> players)
    {
        maxHealth = health;
        _health = maxHealth;

        _players = players;
        _renderer = GetComponent<SpriteRenderer>();
        _particleSystem = GetComponent<ParticleSystem>();
        
        GetComponent<Collider2D>().enabled = true;
        
        transform.localScale *= size;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var total = Vector3.zero;
        var ply = Seek();
        //print(ply);
        total += ply;

        var sep = Separate();
        total += sep;

        transform.up += total * (rotSpeed * Time.fixedDeltaTime);
        var oldRot = transform.rotation.eulerAngles;
        oldRot.x = 0;
        transform.rotation = Quaternion.Euler(oldRot);
        transform.position += transform.up * (moveSpeed * Time.fixedDeltaTime);
    }

    private Vector3 Seek()
    {
        var ply = Vector2.zero;
        if (_players.Count > 0)
        {
            var closest = _players.Where(x => x.gameObject.activeInHierarchy).Aggregate(
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Player>(out var p))
        {
            p.HitByEnemy(this);
            Die();
        } else if (other.gameObject.TryGetComponent<Bullet>(out var b))
        {
            b.Hit(this);
            Hit(b);
        }
    }

    private void Hit(Bullet b)
    {
        _renderer.DOColor(Color.red, 0.15f).SetEase(Ease.Flash, 2);
        _health -= b.sender.weapon.weaponDamage;
        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // particles, sounds, etc
        GetComponent<Collider2D>().enabled = false;
        _particleSystem.Play();
        _renderer.DOFade(0f, 0.1f).SetDelay(0.1f).OnComplete(() =>
        {
            if (Random.value < 0.1f * Mathf.Log10(Level) || true)
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

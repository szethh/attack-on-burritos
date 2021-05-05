using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public WeaponStats weapon;
    private Player _sender;

    private Vector3 _initialPos;
    private float _initialTime;

    private Rigidbody2D _rb;
    private Collider2D _col;
    private ParticleSystem _particleSystem;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _particleSystem = GetComponent<ParticleSystem>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Player sender, WeaponStats weapon)
    {
        _sender = sender;
        this.weapon = weapon;
        _initialPos = transform.position;
        _initialTime = Time.time;
        _col.enabled = true;
        _renderer.color = Color.white;
    }

    private void FixedUpdate()
    {
        //transform.position += transform.up * (weapon.bulletSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(transform.position + transform.up * (weapon.bulletSpeed * Time.fixedDeltaTime));

        if ((transform.position - _initialPos).sqrMagnitude > weapon.range*weapon.range || Time.time > _initialTime+3)
            Die();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("wall"))
        {
            Die(other.GetContact(0).normal);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            Die(other.GetContact(0).normal);
        }
    }

    public void Hit(Enemy e)
    {
        _sender.HitEnemy(e);
        if (weapon.piercing)
            return;
        Die(transform.position - e.transform.position);
    }

    private void Die(Vector3 normal = new Vector3())
    {
        _col.enabled = false;
        _particleSystem.Play();
        transform.up = normal;
        _renderer.color = Color.clear;
        _renderer.DOFade(0f, 0.2f).OnComplete(() => { gameObject.SetActive(false); });
    }
}

using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Weapon : MonoBehaviour
{
    public WeaponStats stats;
    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;
    private BoxCollider2D _col;
    private float _initialTime;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _col = GetComponent<BoxCollider2D>();

        _rb.AddTorque(0.5f, ForceMode2D.Force);
        _rb.AddForce(Random.insideUnitCircle*10f, ForceMode2D.Force);
        _renderer.sprite = stats.sprite;
        // adjust collider to sprite
        _col.offset = _renderer.bounds.center - transform.localPosition;
        // adjust for pivot on the left side
        _col.offset += new Vector2(_renderer.bounds.extents.x, 0);
        _col.size = _renderer.bounds.size*2;
        _initialTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > _initialTime + 7)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Player>(out var p))
        {
            p.PickUp(this);
            Die();
        }
    }

    private void Die()
    {
        // particles, sound, etc
        Destroy(gameObject);
    }
}
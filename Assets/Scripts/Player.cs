using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public WeaponStats weapon, defaultWeapon;
    public GameObject bulletPrefab;

    public int playerIdx = 0;

    private int _score;
    public int Score
    {
        get => _score;
        set => _score = Mathf.Max(0, value);
    }

    public List<float> hits;
    public int hitsByEnemy;
    
    private KeyCode _shootKey;
    private KeyCode _strafeKey;
    
    private float _nextShot;
    private ObjectPooling _pooling;

    private Vector2 _lastDir;
    private Rigidbody2D _rb;


    private void Start()
    {
        _pooling = ObjectPooling.Singleton;
        
        _shootKey = playerIdx == 0 ? KeyCode.C : KeyCode.M;
        _strafeKey = playerIdx == 0 ? KeyCode.LeftShift : KeyCode.RightShift;
        
        _lastDir = transform.up;

        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Movement
        var input = new Vector2(
            Input.GetAxisRaw("Horizontal " + playerIdx),
            Input.GetAxisRaw("Vertical " + playerIdx));
        _rb.MovePosition((Vector2)transform.position + input * (moveSpeed * Time.fixedDeltaTime));

        if (input != _lastDir && input != Vector2.zero && !Input.GetKey(_strafeKey))
            _lastDir = input;

        if (weapon == null)
            weapon = defaultWeapon;

        // Shooting
        if (Input.GetKey(_shootKey) && Time.time > _nextShot)
        {
            _nextShot = Time.time + 1f / weapon.fireRate;
            var b = _pooling.GetFromPool(weapon.bulletPrefab).GetComponent<Bullet>();
            Physics2D.IgnoreCollision(b.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            b.transform.position = transform.position;
            var spread = Vector2.one * (Random.Range(weapon.spread.x, weapon.spread.y) * (Random.value * 2 - 1));
            b.transform.up = _lastDir + spread;
            b.sender = this;
            b.speed = weapon.bulletSpeed;
            b.piercing = weapon.piercing;
        }
    }

    public void HitByEnemy(Enemy e)
    {
        GameManager.Singleton.HitByEnemy(this, e);
    }

    public void HitEnemy(Enemy e)
    {
        GameManager.Singleton.HitEnemy(this, e);
    }

    public void PickUp(Weapon item)  // refactor to PickUp type
    {
        weapon = item.stats;
    }
}

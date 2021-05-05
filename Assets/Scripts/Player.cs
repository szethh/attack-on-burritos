using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public WeaponStats weapon, defaultWeapon;
    public SpriteRenderer gunHolder;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private LineRenderer laser;

    public int playerIdx = 0;

    private int _score;
    public int Score
    {
        get => _score;
        set => _score = Mathf.Max(0, value);
    }
    public int bullets;

    public List<float> hits;
    public int hitsByEnemy;
    
    private KeyCode _shootKey;
    private KeyCode _strafeKey;
    
    private float _nextShot;
    private ObjectPooling _pooling;

    private Vector2 _lastDir;
    private Rigidbody2D _rb;
    private const int _laserLayer = ~(1 << 8 | 1 << 2 | 1 << 6);

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

        if (bullets <= 0)
        {
            weapon = null;
        }
        
        if (weapon == null)
        {
            Equip(defaultWeapon);
        }

        gunHolder.transform.right = _lastDir;
        laser.SetPositions(new[]
        {
            firingPoint.position,
            firingPoint.position+firingPoint.right*
            Physics2D.Raycast(firingPoint.position, firingPoint.right, weapon.range, _laserLayer).distance
        });
        laser.startWidth = laser.endWidth = 0.015f;
        laser.startColor = laser.endColor = weapon.laser;

        // Shooting
        if (Input.GetKey(_shootKey) && Time.time > _nextShot)
        {
            _nextShot = Time.time + 1f / weapon.fireRate;
            bullets--;
            for (int i = 0; i < weapon.bulletsPerShot; i++)
            {
                var b = _pooling.GetFromPool(weapon.bulletPrefab).GetComponent<Bullet>();
                Physics2D.IgnoreCollision(b.GetComponent<Collider2D>(), GetComponent<Collider2D>());

                // TODO: help
                b.transform.position = firingPoint.position;
                
                var spread = Vector2.one.Random(weapon.spread.x, weapon.spread.y).RandomFlip();
                // Si estamos disparando en diagonal no podemos añadir spread en ambos ejes. Dropeamos uno aleatoriamente
                if (_lastDir.x != 0f && _lastDir.y != 0f)
                    spread = spread.RandomSide();

                b.transform.up = _lastDir + spread;
                b.Init(this, weapon);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(firingPoint.position, 0.1f);
    }

    public void HitByEnemy(Enemy e)
    {
        GameManager.Singleton.HitByEnemy(this, e);
    }

    public void HitEnemy(Enemy e)
    {
        GameManager.Singleton.HitEnemy(this, e);
    }

    private void Equip(WeaponStats w)
    {
        weapon = w;
        gunHolder.sprite = weapon.sprite;
        bullets = weapon.ammo;

        var oldRot = transform.eulerAngles;
        transform.rotation = Quaternion.identity;
        firingPoint.localPosition = gunHolder.transform.localPosition + weapon.firingPoint;
        transform.eulerAngles = oldRot;
    }

    public void PickUp(Weapon item)  // refactor to PickUp type
    {
        Equip(item.stats);
    }
}

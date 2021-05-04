using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public WeaponStats weapon, defaultWeapon;
    public GameObject bulletPrefab;
    public SpriteRenderer gunHolder;

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

        gunHolder.transform.right = _lastDir;

        // Shooting
        if (Input.GetKey(_shootKey) && Time.time > _nextShot)
        {
            _nextShot = Time.time + 1f / weapon.fireRate;
            for (int i = 0; i < weapon.bulletsPerShot; i++)
            {
                var b = _pooling.GetFromPool(weapon.bulletPrefab).GetComponent<Bullet>();
                Physics2D.IgnoreCollision(b.GetComponent<Collider2D>(), GetComponent<Collider2D>());

                //print(gunHolder.transform.InverseTransformPoint(gunHolder.transform.localPosition));
                // TODO: help
                b.transform.position = gunHolder.transform.TransformPoint(gunHolder.transform.InverseTransformPoint(gunHolder.transform.position) + weapon.firingPoint);
                
                var spread = Vector2.one.Random(weapon.spread.x, weapon.spread.y).RandomFlip();
                // Si estamos disparando en diagonal no podemos añadir spread en ambos ejes. Dropeamos uno aleatoriamente
                if (_lastDir.x != 0f && _lastDir.y != 0f)
                    spread = spread.RandomSide();

                b.transform.up = _lastDir + spread;
                b.Init(this, weapon);
            }
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
        gunHolder.sprite = weapon.sprite;
    }
}

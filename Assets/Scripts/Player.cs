using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

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

    public List<int> hits;
    public int hitsByEnemy;
    public AudioClip[] playerHurtClips;
    
    private KeyCode _shootKey;
    private KeyCode _strafeKey;
    
    private float _nextShot;
    private ObjectPooling _pooling;

    private Vector2 _lastDir;
    private Rigidbody2D _rb;
    private const int _laserLayer = ~(1 << 8 | 1 << 2 | 1 << 6 | 1 << 9);
    private SpriteRenderer _renderer;
    private bool _invincible;
    private AudioSource _sound;
    private AudioSource _weaponSound;
    [SerializeField] private Sprite normalSprite, angrySprite;

    private void Start()
    {
        _pooling = ObjectPooling.Singleton;
        
        _shootKey = playerIdx == 0 ? KeyCode.Space : KeyCode.Return;
        _strafeKey = playerIdx == 0 ? KeyCode.LeftShift : KeyCode.RightShift;
        
        _lastDir = transform.up;

        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _sound = GetComponent<AudioSource>();
        _weaponSound = GetComponentInChildren<AudioSource>();
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

        if (bullets <= 0 && weapon != defaultWeapon)
        {
            weapon = null;
        }
        
        if (weapon == null)
        {
            Equip(defaultWeapon);
        }

        _renderer.transform.eulerAngles = new Vector3(0, 180*Mathf.Min(0, Mathf.Sign(_lastDir.x)));
        gunHolder.transform.right = _lastDir;
        gunHolder.flipY = _lastDir.x < 0 && Math.Abs(_lastDir.y) > 0.01f;
        
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
            
            if (weapon.shootingClips.Length > 0 && (!_weaponSound.isPlaying || weapon.fireRate < 10))
            {
                _weaponSound.clip = weapon.shootingClips[Random.Range(0, weapon.shootingClips.Length)];
                _weaponSound.Play();
            }

            _renderer.sprite = angrySprite;
            int x;
            DOTween.To(() => 0, value => x = value, 3, 0.2f).OnComplete(() =>
            {
                if (!Input.GetKey(_shootKey) || !(Time.time > _nextShot))
                    _renderer.sprite = normalSprite;
            });
            
            for (int i = 0; i < weapon.bulletsPerShot; i++)
            {
                var b = _pooling.GetFromPool(weapon.bulletPrefab).GetComponent<Bullet>();
                Physics2D.IgnoreCollision(b.GetComponent<Collider2D>(), GetComponent<Collider2D>());

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

    public bool HitByEnemy(Enemy e)
    {
        if (_invincible)
            return false;
        _invincible = true;
        if (playerHurtClips.Length > 0)
        {
            _sound.clip = playerHurtClips[Random.Range(0, playerHurtClips.Length)];
            _sound.Play();
        }
        _renderer.sprite = angrySprite;
        _renderer.DOColor(Color.red, 0.45f).SetEase(Ease.Flash, 6).OnComplete(() =>
        {
            _invincible = false;
            _renderer.sprite = normalSprite;
        });
        GameManager.Singleton.HitByEnemy(this, e);
        return true;
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
        
        _nextShot = Time.time + 1f / weapon.fireRate;

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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float shootSpeed = 1f;
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
    private float _nextShot;
    private ObjectPooling _pooling;

    private Vector2 _lastDir;


    private void Start()
    {
        _pooling = ObjectPooling.Singleton;
        
        if (playerIdx == 0)
        {
            _shootKey = KeyCode.C;
        }
        else if (playerIdx == 1)
        {
            _shootKey = KeyCode.M;
        }
    }

    private void Update()
    {
        // Movement
        var input = new Vector2(
            Input.GetAxisRaw("Horizontal " + playerIdx),
            Input.GetAxisRaw("Vertical " + playerIdx));
        transform.position += (Vector3)input * (moveSpeed * Time.deltaTime);

        if (input != _lastDir && input != Vector2.zero)
            _lastDir = input;

        // Shooting
        if (Input.GetKey(_shootKey) && Time.time > _nextShot)
        {
            _nextShot = Time.time + 1f / shootSpeed;
            var b = _pooling.GetFromPool(bulletPrefab).GetComponent<Bullet>();
            b.transform.position = transform.position;
            b.transform.up = _lastDir;
            b.sender = this;
            // b.piercing = piercing;
            
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Power ups
        /*
        if (other.gameObject.TryGetComponent<PowerUp>(out var p))
        {
            p.Hit(this);
        }
        */
    }

    public void HitByEnemy(Enemy e)
    {
        GameManager.Singleton.HitByEnemy(this, e);
    }

    public void HitEnemy(Enemy e)
    {
        GameManager.Singleton.HitEnemy(this, e);
    }
}

using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float shootSpeed = 1f;
    public GameObject bulletPrefab;
    public int maxHealth = 3;
    private int _health;

    public int playerIdx = 0;
    
    private KeyCode _shootKey;
    private float nextShot;
    private ObjectPooling _pooling;


    private void Start()
    {
        _pooling = ObjectPooling.Singleton;
        
        if (playerIdx == 0)
        {
            _shootKey = KeyCode.Z;
        }
        else if (playerIdx == 1)
        {
            _shootKey = KeyCode.M;
        }

        _health = maxHealth;
    }

    private void Update()
    {
        // Movement
        var input = new Vector2(
            Input.GetAxisRaw("Horizontal " + playerIdx),
            Input.GetAxisRaw("Vertical " + playerIdx));
        transform.position += (Vector3)input * (moveSpeed * Time.deltaTime);
        
        // Shooting
        if (Input.GetKey(_shootKey) && Time.time > nextShot)
        {
            nextShot = Time.time + 1f / shootSpeed;
            print("pew");
            var b = _pooling.GetFromPool(bulletPrefab).GetComponent<Bullet>();
            b.transform.position = transform.position;
            b.transform.up = input.normalized;
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

    public void Hit(Enemy e)
    {
        _health--;
        // die if 0 left
    }
}

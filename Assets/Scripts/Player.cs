using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float shootSpeed = 1f;
    public int maxHealth = 3;
    private int _health;

    public int playerIdx = 0;
    
    private KeyCode _shootKey;
    private Rigidbody2D _rb;
    
    // Start is called before the first frame update
    void Start()
    {
        if (playerIdx == 0)
        {
            _shootKey = KeyCode.Z;
        }
        else if (playerIdx == 1)
        {
            _shootKey = KeyCode.M;
        }

        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        var input = new Vector2(
            Input.GetAxisRaw("Horizontal " + playerIdx),
            Input.GetAxisRaw("Vertical " + playerIdx));
        _rb.MovePosition(transform.position + (Vector3)input * (moveSpeed * Time.deltaTime));
        
        // Shooting
        if (Input.GetKeyDown(_shootKey))
        {
            print("pew");
        }
    }
}

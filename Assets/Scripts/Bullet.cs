using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1f;
    public bool piercing;
    public Player sender;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.up * (speed * Time.fixedDeltaTime);
        if (Mathf.Abs(transform.position.x) > 50 || Mathf.Abs(transform.position.y) > 50)
            ObjectPooling.Singleton.AddToPool(gameObject);
    }

    public void Hit(Enemy e)
    {
        sender.HitEnemy(e);
        // maybe add check for piercing bullets
        if (piercing)
            return;
        gameObject.SetActive(false);
    }
}

using UnityEngine;

public class Projectile : MonoBehaviour
{
    public PlayerController owner;

    private Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        transform.right = -rb2D.velocity.normalized;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject != owner.gameObject)
        {
            owner.DealDamage(collision.gameObject.GetComponent<PlayerController>());
            Destroy(gameObject);
        }
    }
}

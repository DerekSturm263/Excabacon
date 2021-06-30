using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb2D;

    [HideInInspector] public float damage;
    private bool isColliding;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isColliding)
        {
            transform.right = -rb2D.velocity.normalized;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isColliding = true;

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

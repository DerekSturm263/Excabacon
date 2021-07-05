using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public PlayerController owner;

    private Rigidbody2D rb2D;
    private BoxCollider2D col2D;

    private float timeSinceTimer;
    private float timerTime = 0.25f;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        col2D = GetComponent<BoxCollider2D>();

        col2D.enabled = false;
    }

    private void Update()
    {
        transform.right = -rb2D.velocity.normalized;

        if (timeSinceTimer > 0f)
        {
            timeSinceTimer += Time.deltaTime;
        }

        if (timeSinceTimer > timerTime)
        {
            col2D.enabled = true;
        }
    }

    public void Shoot(Vector2 direction)
    {
        rb2D.AddForce(direction, ForceMode2D.Impulse);
        StartTimer();
    }

    private void StartTimer()
    {
        timeSinceTimer = 0.01f;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        HitEffect(col.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        HitEffect(col.gameObject);
    }

    private void HitEffect(GameObject col)
    {
        if (col.CompareTag("Player") && col != owner.gameObject)
        {
            owner.DealDamage(col.GetComponent<PlayerController>());
        }
        else if (col.CompareTag("Ground"))
        {
            GameController.TerrainInterface.DestroyTerrain(transform.position, owner.weaponClass.mineRadius, out bool hitBlock);
        }

        Destroy(gameObject);
    }
}

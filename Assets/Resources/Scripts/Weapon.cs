using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PlayerController owner;
    private LayerMask player = 1 << 8;

    private void Awake()
    {
        owner = transform.GetComponentInParent<PlayerController>();
    }

    public void CheckForImpact()
    {
        RaycastHit2D[] results = Physics2D.BoxCastAll(transform.position, transform.localScale, 0f, Vector2.zero, 0f, player);

        foreach (RaycastHit2D hit in results)
        {
            if (hit.transform.gameObject != owner.gameObject)
            {
                owner.DealDamage(hit.transform.GetComponent<PlayerController>());
                return;
            }
        }
    }
}

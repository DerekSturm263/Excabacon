using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    private LayerMask ground = 1 << 6;
    private bool used;
    public GameObject carrier;

    [HideInInspector] public Rigidbody2D rb2D;
    [HideInInspector] public SpriteRenderer sprtRndr;

    public Action<Item> useAction; // Action for when the player uses the item.
    public Action<Item> hitAction; // Action for when the item hits the ground or another player.

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (carrier != null)
        {
            transform.position = carrier.transform.position;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Currently only works when hitting the ground. Update in future to work whenever it hits something that isn't the player who threw or used it.
        if (used && collision.gameObject.layer << ground != 0)
        {
            hitAction.Invoke(this);
        }
    }

    public void Throw()
    {
        rb2D.AddForce(new Vector2(5f * (carrier.GetComponent<SpriteRenderer>().flipX ? -1f : 1f), 5f), ForceMode2D.Impulse);

        carrier = null;
        used = true;
    }

    public void Explode()
    {
        GameController.TerrainInterface.DestroyTerrain(transform.position, 10, out bool hasMined);
        Destroy(gameObject);
    }

    public void CopyFromItemType(ItemType baseItem)
    {
        useAction = baseItem.useAction;
        hitAction = baseItem.hitAction;

        sprtRndr.sprite = baseItem.icon;
    }
}

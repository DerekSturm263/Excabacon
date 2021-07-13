using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject item;
    private float lastTimeSinceItemSpawn;
    private float nextTimeTillItemSpawn;

    public TileTerrainTestScript terrain;

    private void Awake()
    {
        if (GameController.gameSettings.itemSettings.spawnRate == 0f)
        {
            gameObject.SetActive(false);
        }

        nextTimeTillItemSpawn = (-20f * GameController.gameSettings.itemSettings.spawnRate * Random.Range(0.8f, 1.2f)) + 70f;
    }

    private void Update()
    {
        lastTimeSinceItemSpawn += Time.deltaTime;

        if (lastTimeSinceItemSpawn >= nextTimeTillItemSpawn)
        {
            SpawnItem(Random.Range(0, GameController.gameSettings.itemSettings.items.Count));
        }
    }

    public void SpawnItem(int itemNum)
    {
        Item newItem = Instantiate(item).GetComponent<Item>();
        newItem.CopyFromItemType(GameController.gameSettings.itemSettings.items[itemNum]);

        Vector2 spawnLoc = Vector2.zero;
        spawnLoc.x = Random.Range(terrain.Tiles.MinTileBounds.x + 5, terrain.Tiles.MaxTileBounds.x - 4);
        spawnLoc.y = Random.Range(terrain.Tiles.MinTileBounds.y + 5, terrain.Tiles.MaxTileBounds.y - 4);

        // Change this to be a random point on the map later on.
        newItem.transform.position = spawnLoc;

        lastTimeSinceItemSpawn = 0f;
        nextTimeTillItemSpawn = (-20f * GameController.gameSettings.itemSettings.spawnRate * Random.Range(0.8f, 1.2f)) + 70f;

        Debug.Log(newItem.item.name + " spawned at " + spawnLoc);
    }
}

using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject item;
    private float lastTimeSinceItemSpawn;
    private float nextTimeTillItemSpawn;

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

        // Change this to be a random point on the map later on.
        newItem.transform.position = Vector2.zero;

        lastTimeSinceItemSpawn = 0f;
        nextTimeTillItemSpawn = (-20f * GameController.gameSettings.itemSettings.spawnRate * Random.Range(0.8f, 1.2f)) + 70f;
    }
}

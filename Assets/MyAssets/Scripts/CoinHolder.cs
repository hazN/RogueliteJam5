using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinHolder : MonoBehaviour
{
    public float rotationSpeed = 100f;

    // Prefabs
    public GameObject[] basicCoins;
    public GameObject[] healthCoins;
    public GameObject[] goldCoins;
    public GameObject[] lootCoins;
    public GameObject[] bossCoins;
    public void SetCoins(Door.RoomType coin1, int level1, Door.RoomType coin2, int level2)
    {
        // Destroy any existing coins in the Coin1 and Coin2 objects
        foreach (Transform child in transform.Find("Coin1"))
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in transform.Find("Coin2"))
        {
            Destroy(child.gameObject);
        }

        // Instantiate the new coins based on the specified room type and level
        GameObject coin1Prefab = null;
        GameObject coin2Prefab = null;
        switch (coin1)
        {
            case Door.RoomType.Basic:
                coin1Prefab = basicCoins[level1 - 1];
                break;
            case Door.RoomType.Health:
                coin1Prefab = healthCoins[level1 - 1];
                break;
            case Door.RoomType.Gold:
                coin1Prefab = goldCoins[level1 - 1];
                break;
            case Door.RoomType.Loot:
                coin1Prefab = lootCoins[level1 - 1];
                break;
            case Door.RoomType.Boss:
                coin1Prefab = bossCoins[level1 - 1];
                break;
        }
        switch (coin2)
        {
            case Door.RoomType.Basic:
                coin2Prefab = basicCoins[level2 - 1];
                break;
            case Door.RoomType.Health:
                coin2Prefab = healthCoins[level2 - 1];
                break;
            case Door.RoomType.Gold:
                coin2Prefab = goldCoins[level2 - 1];
                break;
            case Door.RoomType.Loot:
                coin2Prefab = lootCoins[level2 - 1];
                break;
            case Door.RoomType.Boss:
                coin2Prefab = bossCoins[level2 - 1];
                break;
        }
        Instantiate(coin1Prefab, transform.Find("Coin1"));
        Instantiate(coin2Prefab, transform.Find("Coin2"));
    }
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}

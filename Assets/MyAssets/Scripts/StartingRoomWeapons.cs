using System.Collections.Generic;
using UnityEngine;

public class StartingRoomWeapons : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Transform weaponsParent;

    private void Start()
    {
        InstantiateWeapons();
    }
    public void ResetWeapons()
    {
        // Destroy old weapons
        Transform weaponsToDestroy = weaponsParent.transform;
        foreach (Transform weapon in weaponsToDestroy)
        {
            Destroy(weapon.gameObject);
        }
        // Place new weapons
        InstantiateWeapons();
    }
    private void InstantiateWeapons()
    {
        // Place new weapons
        int numWeapons = weapons.Length;
        Bounds tableBounds = weaponsParent.gameObject.GetComponent<MeshRenderer>().bounds;
        float spacing = tableBounds.size.x / (numWeapons + 1); // equally spaced along the x-axis
        Vector3 spawnPos = new Vector3(tableBounds.min.x + spacing, tableBounds.max.y, tableBounds.center.z - 1f);
        for (int i = 0; i < numWeapons; i++)
        {
            Quaternion rotation = Quaternion.identity;
            switch (i)
            {
                case 0:
                    rotation = Quaternion.Euler(90f, 0f, 0f);
                    break;
                case 1:
                    rotation = Quaternion.Euler(0f, 90f, 90f);
                    break;
                case 2:
                    rotation = Quaternion.Euler(0f, 90f, 83f);
                    break;
            }
            GameObject weapon = Instantiate(weapons[i], spawnPos, rotation, weaponsParent);
            weapon.name = weapons[i].name;
            spawnPos += new Vector3(spacing, 0f, 0f);
        }
    }
}

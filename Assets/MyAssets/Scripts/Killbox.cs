using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.transform.root.CompareTag("Player");
        if (isPlayer)
        {
            other.GetComponentInChildren<Health>().TakeDamage(999999999999f);
        }
        else Destroy(other);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    [SerializeField] private float healthAmount;
    public void Consume()
    {
        Health playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        if (playerHealth.Heal(healthAmount)) 
            Destroy(gameObject);
    }
}

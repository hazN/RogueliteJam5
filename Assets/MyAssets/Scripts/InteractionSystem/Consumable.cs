using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    [SerializeField] private float healthAmount;
    public void Consume()
    {
        Health playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        if (playerHealth.currentHealth < playerHealth.maxHealth ) 
        {
            playerHealth.currentHealth = Mathf.Min(playerHealth.currentHealth + healthAmount, playerHealth.maxHealth);
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public float maxHealth = 100;
    [SerializeField] public float currentHealth;
    [SerializeField] private Slider healthBar;
    Animator anim;
    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }
    public void SetHealth(float health)
    {
        maxHealth = health;
        if (maxHealth < 100) maxHealth = 100;
        currentHealth = health;
        if (healthBar)
            healthBar.value = currentHealth / maxHealth;
    }
    public bool Heal(float hp)
    {
        // Return false if already max hp
        if (currentHealth == maxHealth) 
            return false;
        // Otherwise heal
        currentHealth = Mathf.Min(currentHealth + hp, maxHealth);
        if (healthBar)
            healthBar.value = currentHealth / maxHealth;
        return true;
    }
    public void TakeDamage(float damage)
    {
        anim.SetTrigger("takeDamage");
        Enemy enemy = GetComponent<Enemy>();
        if (enemy)
        {
            GetComponent<Enemy>().TakeDamage();
            // Make enemy face player
            Vector3 direction = (GameObject.Find("PlayerArmature").transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            // Push enemy back
            transform.position -= direction * 0.25f;
        }
        currentHealth -= damage;
        if (healthBar)
        {
            healthBar.value = currentHealth / maxHealth;
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        // Check if its a player 
        if (TryGetComponent(out Player player))
        {
            // Check if player has combat component so we can save the stats
            if (TryGetComponent(out PlayerCombat playerCombat))
                playerCombat.SavePlayerStats();
            // Reset player position
            if (TryGetComponent(out CharacterController cc))
            {
                cc.enabled = false;
                player.transform.position = new Vector3(4.9f, 2.37f, -48f);
                player.transform.rotation = Quaternion.identity;
                cc.enabled = true;
            }
            SetHealth(maxHealth);
            GameObject.Find("DungeonManager").GetComponent<RoomGenerator>().roomNumber = 0;
        } // Otherwise destroy
        else Destroy(gameObject);
    }
}
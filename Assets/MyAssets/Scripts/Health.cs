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
        Destroy(gameObject);
    }
}